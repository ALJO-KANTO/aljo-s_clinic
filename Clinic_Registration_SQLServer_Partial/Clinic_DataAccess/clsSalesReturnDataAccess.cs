using Clinic_DataAccess;
using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsSalesReturnDataAccess
    {



        public static DataTable GetBillCompleteInfo(
     int billID,
     ref string billNumber,
     ref DateTime billDate,
     ref string patientName,
     ref bool isFound)
        {
            DataTable dtDetails = new DataTable();
            isFound = false;

            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();

                // STEP 1: Load bill and patient information
                string billQuery = @"
            SELECT
                CONCAT(
                    'INV-',
                    YEAR(B.BillDate),
                    '-',
                    LPAD(B.BillID, 4, '0')
                ) AS BillNumber,
                B.BillDate,
                COALESCE(PE.FullName, 'Pharmacy Sales')
                    AS PatientName
            FROM bills B
            LEFT JOIN visits V
                ON B.VisitID = V.VisitID
            LEFT JOIN patients PA
                ON V.PatientID = PA.PatientID
            LEFT JOIN persons PE
                ON PA.PersonID = PE.PersonID
            WHERE B.BillID = @BillID;";

                using (MySqlCommand command =
                    new MySqlCommand(billQuery, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            billNumber = reader["BillNumber"].ToString();

                            billDate = Convert.ToDateTime(
                                reader["BillDate"]);

                            patientName = reader["PatientName"].ToString();

                            isFound = true;
                        }
                    }
                }

                if (!isFound)
                    return dtDetails;

                // STEP 2: Load medicine details
                // This query loads prescription details.
                // It does NOT establish that medicine was dispensed.
                string detailsQuery = @"
    SELECT
        PD.PrescriptionDetailID AS PrescriptionDetailsID,
        PD.MedicineID,

        COALESCE(
            PD.SavedMedicineName,
            ''
        ) AS SavedMedicineName,

        PD.Quantity AS QtyBought,

        COALESCE(
            (
                SELECT SUM(SRD.QtyReturned)
                FROM salesreturndetails SRD
                INNER JOIN salesreturns SR
                    ON SR.SalesReturnID = SRD.ReturnID
                WHERE SRD.PrescriptionDetailsID =
                    PD.PrescriptionDetailID
                  AND SR.BillID = B.BillID
            ),
            0
        ) AS QtyReturnedBefore,

        PD.SavedMedicinePrice AS UnitPrice,

        CAST(0 AS DECIMAL(10,2)) AS TaxRate,

        P.PrescriptionStatus

    FROM bills B

    INNER JOIN prescriptions P
        ON P.VisitID = B.VisitID

    INNER JOIN prescriptiondetails PD
        ON PD.PrescriptionID = P.PrescriptionID

    WHERE B.BillID = @BillID;";
                using (MySqlCommand command =
                    new MySqlCommand(detailsQuery, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dtDetails);
                    }
                }
            }

            return dtDetails;
        }
        public static int SaveSalesReturn(
            int billID,
            decimal totalRefund,
            int cashierID,
            DataTable dtReturnItems)
        {
            if (dtReturnItems == null || dtReturnItems.Rows.Count == 0)
                return -1;

            int newReturnID = -1;

            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        // STEP 1: Check that the bill is eligible
                        string checkBillQuery = @"
                    SELECT COUNT(*)
                    FROM bills
                    WHERE BillID = @BillID
                      AND LOWER(TRIM(PaymentStatus))
                          IN ('paid', 'partial');";

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                checkBillQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);

                            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                                throw new Exception(
                                    "Bill is not paid or partially paid.");
                        }

                        DataRow[] returnedRows =
                            dtReturnItems.Select("QtyReturned > 0");

                        if (returnedRows.Length == 0)
                            throw new Exception("No return quantity selected.");

                        // STEP 2: Insert return master
                        string insertMasterQuery = @"
                    INSERT INTO salesreturns
                    (
                        BillID,
                        ReturnDate,
                        TotalRefund,
                        CashierID
                    )
                    VALUES
                    (
                        @BillID,
                        NOW(),
                        @TotalRefund,
                        @CashierID
                    );";

                        using (MySqlCommand cmd =
                            new MySqlCommand(
                                insertMasterQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);
                            cmd.Parameters.AddWithValue("@TotalRefund", totalRefund);
                            cmd.Parameters.AddWithValue("@CashierID", cashierID);

                            cmd.ExecuteNonQuery();

                            newReturnID = Convert.ToInt32(cmd.LastInsertedId);
                        }

                        // STEP 3: Process each returned medicine
                        foreach (DataRow row in returnedRows)
                        {
                            int prescriptionDetailID =
                                Convert.ToInt32(
                                    row["PrescriptionDetailsID"]);

                            decimal qtyReturned =
                                Convert.ToDecimal(row["QtyReturned"]);

                            decimal unitPrice =
                                Convert.ToDecimal(row["UnitPrice"]);

                            int medicineID;

                            // Verify medicine belongs to this bill
                            string medicineQuery = @"
                        SELECT PD.MedicineID
                        FROM prescriptiondetails PD
                        INNER JOIN prescriptions P
                            ON PD.PrescriptionID = P.PrescriptionID
                        INNER JOIN bills B
                            ON P.VisitID = B.VisitID
                        WHERE B.BillID = @BillID
                          AND PD.PrescriptionDetailID = @DetailID
                        LIMIT 1;";

                            using (MySqlCommand cmd =
                                new MySqlCommand(
                                    medicineQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BillID", billID);
                                cmd.Parameters.AddWithValue(
                                    "@DetailID", prescriptionDetailID);

                                object result = cmd.ExecuteScalar();

                                if (result == null || result == DBNull.Value)
                                    throw new Exception(
                                        "Prescription medicine was not found.");

                                medicineID = Convert.ToInt32(result);
                            }

                            // Prevent returning more than purchased
                            string quantityQuery = @"
                        SELECT
                            PD.Quantity -
                            COALESCE(
                                (
                                    SELECT SUM(SRD.QtyReturned)
                                    FROM salesreturndetails SRD
                                    INNER JOIN salesreturns SR
                                        ON SR.SalesReturnID = SRD.ReturnID
                                    WHERE SRD.PrescriptionDetailsID =
                                          PD.PrescriptionDetailID
                                ), 0
                            )
                        FROM prescriptiondetails PD
                        WHERE PD.PrescriptionDetailID = @DetailID;";

                            using (MySqlCommand cmd =
                                new MySqlCommand(
                                    quantityQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@DetailID", prescriptionDetailID);

                                decimal availableQty =
                                    Convert.ToDecimal(cmd.ExecuteScalar());

                                if (qtyReturned <= 0 ||
                                    qtyReturned > availableQty)
                                {
                                    throw new Exception(
                                        "Invalid return quantity.");
                                }
                            }

                            // Insert return detail
                            string insertDetailQuery = @"
                        INSERT INTO salesreturndetails
                        (
                            ReturnID,
                            PrescriptionDetailsID,
                            QtyReturned,
                            UnitPrice
                        )
                        VALUES
                        (
                            @ReturnID,
                            @DetailID,
                            @QtyReturned,
                            @UnitPrice
                        );";

                            using (MySqlCommand cmd =
                                new MySqlCommand(
                                    insertDetailQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@ReturnID", newReturnID);

                                cmd.Parameters.AddWithValue(
                                    "@DetailID", prescriptionDetailID);

                                cmd.Parameters.AddWithValue(
                                    "@QtyReturned", qtyReturned);

                                cmd.Parameters.AddWithValue(
                                    "@UnitPrice", unitPrice);

                                cmd.ExecuteNonQuery();
                            }

                            // Restore medicine stock
                            string updateStockQuery = @"
                        UPDATE medicines
                        SET CurrentStock = CurrentStock + @Quantity
                        WHERE MedicineID = @MedicineID;";

                            using (MySqlCommand cmd =
                                new MySqlCommand(
                                    updateStockQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@Quantity", qtyReturned);

                                cmd.Parameters.AddWithValue(
                                    "@MedicineID", medicineID);

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception(
                                        "Failed to update medicine stock.");
                            }

                            // Record inventory movement
                            string inventoryQuery = @"
                        INSERT INTO inventorytransactions
                        (
                            MedicineID,
                            QuantityChange,
                            ReferenceID,
                            TransactionDate,
                            UserID
                        )
                        VALUES
                        (
                            @MedicineID,
                            @Quantity,
                            @ReturnID,
                            NOW(),
                            @UserID
                        );";

                            using (MySqlCommand cmd =
                                new MySqlCommand(
                                    inventoryQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@MedicineID", medicineID);

                                cmd.Parameters.AddWithValue(
                                    "@Quantity", qtyReturned);

                                cmd.Parameters.AddWithValue(
                                    "@ReturnID", newReturnID);

                                cmd.Parameters.AddWithValue(
                                    "@UserID", cashierID);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        return newReturnID;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        System.Diagnostics.Debug.WriteLine(
                            "Sales Return Error: " + ex);

                        throw new InvalidOperationException(
                            "Failed to save sales return for BillID: " +
                            billID, ex);
                    }
                }
            }
        }
        public static bool IsBillPaidOrPartiallyPaid(int billID)
        {
            const string query = @"
        SELECT COUNT(*)
        FROM bills
        WHERE BillID = @BillID
          AND (
              LOWER(TRIM(PaymentStatus)) = 'paid'
              OR LOWER(TRIM(PaymentStatus)) = 'partial'
          );";

            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BillID", billID);

                    connection.Open();

                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
    }
}