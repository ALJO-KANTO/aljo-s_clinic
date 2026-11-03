using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Clinic_DataAccess
{
    public static partial class ClinicDatabase
    {

        public static void EnsureLaboratoryTables()
        {
            // Call after InitializeDatabase()/ApplyMigrations().
            ExecuteNonQuery(@"
        CREATE TABLE IF NOT EXISTS labrequests
        (
            LabRequestID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
            VisitID INT NOT NULL,
            TestID INT NOT NULL,
            BillID INT NULL,
            SavedTestPrice DECIMAL(12,2) NULL,

            RequestedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            Status VARCHAR(30) NOT NULL DEFAULT 'Ordered',
            ResultValue TEXT NULL,
            ResultNotes TEXT NULL,
            ResultedAt DATETIME NULL,

            INDEX IX_LabRequests_Visit (VisitID),
            INDEX IX_LabRequests_Test (TestID),
            INDEX IX_LabRequests_Status (Status),

            UNIQUE KEY UQ_LabRequests_BillID (BillID),

            CONSTRAINT FK_LabRequests_Visit
                FOREIGN KEY (VisitID)
                REFERENCES visits(VisitID),

            CONSTRAINT FK_LabRequests_Test
                FOREIGN KEY (TestID)
                REFERENCES laboratorytests(TestID),

            CONSTRAINT FK_LabRequests_Bill
                FOREIGN KEY (BillID)
                REFERENCES bills(BillID)

        ) ENGINE=InnoDB;
    ");
        }
        public static DataTable GetLabPatients()
        {
            return ExecuteDataTable(@"
        SELECT
            p.PatientID,
            pe.FullName AS PatientName
        FROM patients p
        INNER JOIN persons pe
            ON pe.PersonID = p.PersonID
        WHERE p.IsActive = 1
        ORDER BY pe.FullName;");
        }
        public static int BackfillLabRequestPrices()
        {
            return ExecuteNonQuery(@"
            UPDATE labrequests lr
            INNER JOIN laboratorytests lt
                ON lr.TestID = lt.TestID
            SET lr.SavedTestPrice = lt.TestPrice
            WHERE lr.SavedTestPrice IS NULL;
        ");
        }
        public static DataTable GetLabPatientVisits(int patientID)
        {
            return ExecuteDataTable(@"
        SELECT
            v.VisitID,
            CONCAT(
                'Visit #', v.VisitID,
                ' - ',
                DATE_FORMAT(v.VisitDate, '%d %b %Y'),
                ' - ',
                COALESCE(dp.FullName, 'Unknown Doctor')
            ) AS VisitDisplay
        FROM visits v
        LEFT JOIN doctors d
            ON d.DoctorID = v.DoctorID
        LEFT JOIN persons dp
            ON dp.PersonID = d.PersonID
        WHERE v.PatientID = @PatientID
        ORDER BY v.VisitDate DESC, v.VisitID DESC;",
                new MySqlParameter("@PatientID", patientID));
        }
        public static DataTable GetLabTests(string search = "")
        {
            return ExecuteDataTable(@"SELECT TestID, TestName, TestPrice, IsActive
                FROM laboratorytests
                WHERE (@Search = '' OR TestName LIKE CONCAT('%', @Search, '%'))
                ORDER BY TestName", new MySqlParameter("@Search", search ?? ""));
        }

        public static int SaveLabTest(int testID, string name, decimal price, bool active)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Test name is required.");
            if (price < 0) throw new ArgumentException("Price cannot be negative.");
            if (testID <= 0)
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(@"INSERT INTO laboratorytests (TestName, TestPrice, IsActive)
                        VALUES (@Name, @Price, @Active);", connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name.Trim());
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Active", active);
                        cmd.ExecuteNonQuery();
                        return Convert.ToInt32(cmd.LastInsertedId);
                    }
                }
            }
            ExecuteNonQuery(@"UPDATE laboratorytests SET TestName=@Name, TestPrice=@Price,
                IsActive=@Active WHERE TestID=@ID",
                new MySqlParameter("@Name", name.Trim()), new MySqlParameter("@Price", price),
                new MySqlParameter("@Active", active), new MySqlParameter("@ID", testID));
            return testID;
        }

        public static bool DeactivateLabTest(int testID)
        {
            // Retain historical requests/results; never physically delete referenced tests.
            return ExecuteNonQuery("UPDATE laboratorytests SET IsActive=0 WHERE TestID=@ID",
                new MySqlParameter("@ID", testID)) > 0;
        }

        public static int CreateLabRequest(int visitID, int testID)
        {
            if (visitID <= 0 || testID <= 0)
                throw new ArgumentException(
                    "Select a visit and a test.");

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var cmd = new MySqlCommand(@"
            INSERT INTO labrequests
            (
                VisitID,
                TestID,
                SavedTestPrice
            )
            SELECT
                @VisitID,
                TestID,
                TestPrice
            FROM laboratorytests
            WHERE TestID = @TestID
              AND IsActive = 1;",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@VisitID", visitID);
                    cmd.Parameters.AddWithValue("@TestID", testID);

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "Selected test is inactive or missing.");

                    return Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public static DataTable GetLabRequests(
            string search = "",
            string status = "All")
        {
            return ExecuteDataTable(@"
        SELECT *
        FROM
        (
            SELECT
                R.LabRequestID,
                R.VisitID,
                R.BillID,
                PE.FullName AS PatientName,
                LT.TestName,
                COALESCE(
                    R.SavedTestPrice,
                    LT.TestPrice,
                    0
                ) AS SavedTestPrice,
                COALESCE(PAY.PaidAmount, 0) AS PaidAmount,
                GREATEST(
                    COALESCE(R.SavedTestPrice, LT.TestPrice, 0)
                    - COALESCE(PAY.PaidAmount, 0),
                    0
                ) AS BalanceDue,
                R.RequestedAt,
                CASE
                    WHEN R.Status = 'Cancelled'
                        THEN 'Cancelled'
                    WHEN R.Status = 'Completed'
                        THEN 'Completed'
                    WHEN COALESCE(
                        R.SavedTestPrice,
                        LT.TestPrice,
                        0
                    ) > 0
                    AND COALESCE(PAY.PaidAmount, 0) >=
                        COALESCE(
                            R.SavedTestPrice,
                            LT.TestPrice,
                            0
                        )
                        THEN 'Paid'
                    WHEN COALESCE(PAY.PaidAmount, 0) > 0
                        THEN 'Partial Payment'
                    ELSE 'Ordered'
                END AS Status,
                R.ResultValue,
                R.ResultNotes,
                R.ResultedAt
            FROM labrequests R
            INNER JOIN visits V
                ON V.VisitID = R.VisitID
            INNER JOIN patients P
                ON P.PatientID = V.PatientID
            INNER JOIN persons PE
                ON PE.PersonID = P.PersonID
            INNER JOIN laboratorytests LT
                ON LT.TestID = R.TestID
            LEFT JOIN
            (
                SELECT
                    BillID,
                    SUM(PaymentAmount) AS PaidAmount
                FROM payments
                GROUP BY BillID
            ) PAY
                ON PAY.BillID = R.BillID
        ) X
        WHERE (@Status = 'All' OR X.Status = @Status)
          AND (
              @Search = ''
              OR X.PatientName LIKE CONCAT('%', @Search, '%')
              OR X.TestName LIKE CONCAT('%', @Search, '%')
              OR CAST(X.LabRequestID AS CHAR)
                  LIKE CONCAT('%', @Search, '%')
          )
        ORDER BY X.RequestedAt DESC,
                 X.LabRequestID DESC;",
                new MySqlParameter("@Status", status ?? "All"),
                new MySqlParameter("@Search", search ?? ""));
        }

        public static DataTable GetLabDashboardStats()
        {
            return ExecuteDataTable(@"SELECT
                (SELECT COUNT(*) FROM laboratorytests WHERE IsActive=1) AS TotalTests,
                (SELECT COUNT(*) FROM labrequests) AS TotalRequests,
                (SELECT COUNT(*) FROM labrequests WHERE Status='Ordered') AS Pending,
                (SELECT COUNT(*) FROM labrequests WHERE Status='In Progress') AS InProgress,
                (SELECT COUNT(*) FROM labrequests WHERE Status='Completed') AS Completed");
        }

        public static bool UpdateLabRequestStatus(int requestID, string status)
        {
            if (status != "Ordered" && status != "In Progress" && status != "Cancelled")
                throw new ArgumentException("Unsupported status transition.");
            return ExecuteNonQuery(@"UPDATE labrequests SET Status=@Status
                WHERE LabRequestID=@ID AND Status IN ('Ordered','In Progress')",
                new MySqlParameter("@Status", status), new MySqlParameter("@ID", requestID)) > 0;
        }
        // =====================================================
        // GET OR CREATE BILL FOR LABORATORY REQUEST
        // =====================================================



        public static int GetOrCreateLabRequestBill(int requestID)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                using (MySqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        int visitID;
                        int? existingBillID;
                        decimal testPrice;

                        using (var cmd = new MySqlCommand(@"
                    SELECT
                        lr.VisitID,
                        lr.BillID,
                        COALESCE(lr.SavedTestPrice, lt.TestPrice) AS TestPrice
                    FROM labrequests lr
                    INNER JOIN laboratorytests lt
                        ON lt.TestID = lr.TestID
                    WHERE lr.LabRequestID = @RequestID
                      AND lr.Status <> 'Cancelled'
                    FOR UPDATE;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@RequestID", requestID);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception(
                                        "Laboratory request not found or cancelled.");

                                visitID = Convert.ToInt32(reader["VisitID"]);

                                existingBillID = reader["BillID"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["BillID"]);

                                testPrice = Convert.ToDecimal(reader["TestPrice"]);
                            }
                        }

                        if (existingBillID.HasValue)
                        {
                            transaction.Commit();
                            return existingBillID.Value;
                        }

                        if (testPrice <= 0)
                            throw new Exception("Laboratory test price must be greater than zero.");

                        int billID;

                        using (var cmd = new MySqlCommand(@"
                    INSERT INTO bills
                        (VisitID, PaymentStatus, TotalAmount)
                    VALUES
                        (@VisitID, 'Unpaid', @Amount);",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@VisitID", visitID);
                            cmd.Parameters.AddWithValue("@Amount", testPrice);

                            cmd.ExecuteNonQuery();
                            billID = Convert.ToInt32(cmd.LastInsertedId);
                        }

                        using (var cmd = new MySqlCommand(@"
                    UPDATE bills
                    SET BillNumber = CONCAT(
                        'INV-', YEAR(BillDate), '-',
                        LPAD(BillID, 4, '0'))
                    WHERE BillID = @BillID;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand(@"
                    UPDATE labrequests
                    SET BillID = @BillID,
                        SavedTestPrice = @Amount
                    WHERE LabRequestID = @RequestID;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);
                            cmd.Parameters.AddWithValue("@Amount", testPrice);
                            cmd.Parameters.AddWithValue("@RequestID", requestID);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return billID;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        // =====================================================
        // CHECK WHETHER LAB REQUEST HAS BEEN PAID
        // =====================================================



        public static bool IsLabRequestPaid(int requestID)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                using (var cmd = new MySqlCommand(@"
            SELECT
                COALESCE(lr.SavedTestPrice, lt.TestPrice) AS TestAmount,
                COALESCE((
                    SELECT SUM(p.PaymentAmount)
                    FROM payments p
                    WHERE p.BillID = lr.BillID
                ), 0) AS PaidAmount
            FROM labrequests lr
            INNER JOIN laboratorytests lt
                ON lt.TestID = lr.TestID
            WHERE lr.LabRequestID = @RequestID
              AND lr.BillID IS NOT NULL
              AND lr.Status <> 'Cancelled';",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@RequestID", requestID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return false;

                        decimal amount = Convert.ToDecimal(
                            reader["TestAmount"]);

                        decimal paid = Convert.ToDecimal(
                            reader["PaidAmount"]);

                        return amount > 0 && paid >= amount;
                    }
                }
            }
        }

        public static DataTable GetLabReportDetails(int requestID)
        {
            return ExecuteDataTable(@"
        SELECT
            LR.LabRequestID,
            LR.VisitID,
            LR.BillID,
            P.PatientID,
            PE.FullName AS PatientName,
            COALESCE(DP.FullName, 'Not Assigned') AS DoctorName,
            LT.TestName,
            COALESCE(LR.SavedTestPrice, LT.TestPrice, 0) AS SavedTestPrice,
            LR.ResultValue,
            LR.ResultNotes,
            LR.RequestedAt,
            LR.ResultedAt,
            LR.Status AS RequestStatus,
            COALESCE(PAY.PaidAmount, 0) AS PaidAmount
        FROM labrequests LR
        INNER JOIN visits V
            ON V.VisitID = LR.VisitID
        INNER JOIN patients P
            ON P.PatientID = V.PatientID
        INNER JOIN persons PE
            ON PE.PersonID = P.PersonID
        INNER JOIN laboratorytests LT
            ON LT.TestID = LR.TestID
        LEFT JOIN doctors D
            ON D.DoctorID = V.DoctorID
        LEFT JOIN persons DP
            ON DP.PersonID = D.PersonID
        LEFT JOIN
        (
            SELECT
                BillID,
                SUM(PaymentAmount) AS PaidAmount
            FROM payments
            GROUP BY BillID
        ) PAY
            ON PAY.BillID = LR.BillID
        WHERE LR.LabRequestID = @RequestID;",
                new MySqlParameter("@RequestID", requestID));
        }


        public static bool SaveLabResult(
            int requestID,
            string result,
            string notes)
        {
            if (string.IsNullOrWhiteSpace(result))
                throw new ArgumentException("Result is required.");

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                using (MySqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        decimal testAmount;
                        decimal paidAmount;
                        string status;

                        using (MySqlCommand cmd = new MySqlCommand(@"
                    SELECT
                        COALESCE(
                            LR.SavedTestPrice,
                            LT.TestPrice,
                            0
                        ) AS TestAmount,
                        COALESCE(
                            (
                                SELECT SUM(PAY.PaymentAmount)
                                FROM payments PAY
                                WHERE PAY.BillID = LR.BillID
                            ), 0
                        ) AS PaidAmount,
                        LR.Status
                    FROM labrequests LR
                    INNER JOIN laboratorytests LT
                        ON LT.TestID = LR.TestID
                    WHERE LR.LabRequestID = @RequestID
                      AND LR.BillID IS NOT NULL
                    FOR UPDATE;",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@RequestID", requestID);

                            using (MySqlDataReader reader =
                                cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new InvalidOperationException(
                                        "Laboratory request or bill not found.");

                                testAmount = Convert.ToDecimal(
                                    reader["TestAmount"]);

                                paidAmount = Convert.ToDecimal(
                                    reader["PaidAmount"]);

                                status = Convert.ToString(
                                    reader["Status"]);
                            }
                        }

                        if (status == "Cancelled")
                            throw new InvalidOperationException(
                                "Cancelled laboratory requests cannot be completed.");

                        if (status == "Completed")
                            throw new InvalidOperationException(
                                "This laboratory result is already completed.");

                        if (status != "Ordered" &&
                            status != "In Progress")
                            throw new InvalidOperationException(
                                "The request is not ready for result entry.");

                        if (testAmount <= 0 ||
                            paidAmount < testAmount)
                            throw new InvalidOperationException(
                                "Full payment is required before entering " +
                                "the laboratory result. Remaining: AED " +
                                Math.Max(0, testAmount - paidAmount)
                                    .ToString("N2"));

                        using (MySqlCommand cmd = new MySqlCommand(@"
                    UPDATE labrequests
                    SET ResultValue = @Result,
                        ResultNotes = @Notes,
                        ResultedAt = NOW(),
                        Status = 'Completed'
                    WHERE LabRequestID = @RequestID
                      AND Status IN ('Ordered', 'In Progress');",
                            connection, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Result", result.Trim());

                            cmd.Parameters.AddWithValue(
                                "@Notes", notes ?? "");

                            cmd.Parameters.AddWithValue(
                                "@RequestID", requestID);

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

    }
}
