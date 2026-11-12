using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
                R.LabOrderID,
                R.VisitID,
                R.BillID,
                PE.FullName AS PatientName,
                LT.TestName,
                COALESCE(R.SavedTestPrice, LT.TestPrice, 0)
                    AS SavedTestPrice,
                COALESCE(PAY.PaidAmount, 0) AS PaidAmount,
                GREATEST(
                    COALESCE(R.SavedTestPrice, LT.TestPrice, 0)
                    - COALESCE(PAY.PaidAmount, 0),
                    0
                ) AS BalanceDue,
                R.RequestedAt,
                CASE
                    WHEN R.Status IN
                        ('Cancelled', 'Completed', 'In Progress')
                        THEN R.Status
                    WHEN R.BillID IS NOT NULL
                         AND COALESCE(PAY.PaidAmount, 0) >=
                             COALESCE(
                                 B.TotalAmount,
                                 R.SavedTestPrice,
                                 LT.TestPrice,
                                 0
                             )
                         AND COALESCE(
                             B.TotalAmount,
                             R.SavedTestPrice,
                             LT.TestPrice,
                             0
                         ) > 0
                        THEN 'Paid'
                    WHEN COALESCE(PAY.PaidAmount, 0) > 0
                        THEN 'Partial Payment'
                    ELSE 'Ordered'
                END AS Status,
                R.ResultValue,
                R.ResultNotes,
                R.ResultedAt
            FROM labrequests R
            LEFT JOIN visits V
                ON V.VisitID = R.VisitID
            INNER JOIN patients P
                ON P.PatientID =
                   COALESCE(R.PatientID, V.PatientID)
            INNER JOIN persons PE
                ON PE.PersonID = P.PersonID
            INNER JOIN laboratorytests LT
                ON LT.TestID = R.TestID
            LEFT JOIN bills B
                ON B.BillID = R.BillID
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
              OR CAST(X.LabOrderID AS CHAR)
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


        public static int CreateLabOrder(
            int patientID,
            int? visitID,
            int? doctorID,
            string requestSource,
            IEnumerable<int> testIDs,
            string clinicalNotes = null,
            int? createdByUserID = null)
        {
            if (patientID <= 0)
                throw new ArgumentException("Select a valid patient.");

            if (testIDs == null)
                throw new ArgumentNullException(nameof(testIDs));

            int[] tests = testIDs.Distinct().ToArray();

            if (tests.Length == 0 || tests.Any(id => id <= 0))
                throw new ArgumentException("Select valid laboratory tests.");

            if (requestSource != "Doctor" &&
                requestSource != "WalkIn")
                throw new ArgumentException("Invalid laboratory request source.");

            if (requestSource == "Doctor" &&
                (!visitID.HasValue || visitID.Value <= 0))
                throw new ArgumentException("Doctor requests require a visit.");

            if (requestSource == "WalkIn" && visitID.HasValue)
                throw new ArgumentException("Walk-in requests cannot have a visit.");

            using (var con = GetConnection())
            {
                con.Open();

                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        // Verify patient exists and is active.
                        using (var cmd = new MySqlCommand(@"
                    SELECT COUNT(*)
                    FROM patients
                    WHERE PatientID = @PatientID
                      AND IsActive = 1;",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@PatientID", patientID);

                            if (Convert.ToInt32(cmd.ExecuteScalar()) != 1)
                                throw new InvalidOperationException(
                                    "Patient does not exist or is inactive.");
                        }

                        // Verify visit and doctor belong together.
                        if (visitID.HasValue)
                        {
                            using (var cmd = new MySqlCommand(@"
                        SELECT DoctorID
                        FROM visits
                        WHERE VisitID = @VisitID
                          AND PatientID = @PatientID;",
                                con, tx))
                            {
                                cmd.Parameters.AddWithValue("@VisitID", visitID.Value);
                                cmd.Parameters.AddWithValue("@PatientID", patientID);

                                object value = cmd.ExecuteScalar();

                                if (value == null || value == DBNull.Value)
                                    throw new InvalidOperationException(
                                        "Visit does not belong to this patient.");

                                int actualDoctorID = Convert.ToInt32(value);

                                if (doctorID.HasValue &&
                                    doctorID.Value != actualDoctorID)
                                    throw new InvalidOperationException(
                                        "Doctor does not match the selected visit.");

                                doctorID = actualDoctorID;
                            }
                        }

                        // Create order header.
                        int orderID;

                        using (var cmd = new MySqlCommand(@"
                    INSERT INTO laborders
                    (
                        PatientID,
                        VisitID,
                        DoctorID,
                        RequestSource,
                        OrderStatus,
                        ClinicalNotes,
                        CreatedByUserID
                    )
                    VALUES
                    (
                        @PatientID,
                        @VisitID,
                        @DoctorID,
                        @RequestSource,
                        'Ordered',
                        @ClinicalNotes,
                        @CreatedByUserID
                    );",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@PatientID", patientID);
                            cmd.Parameters.AddWithValue(
                                "@VisitID", (object)visitID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue(
                                "@DoctorID", (object)doctorID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue(
                                "@RequestSource", requestSource);
                            cmd.Parameters.AddWithValue(
                                "@ClinicalNotes",
                                (object)clinicalNotes ?? DBNull.Value);
                            cmd.Parameters.AddWithValue(
                                "@CreatedByUserID",
                                (object)createdByUserID ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                            orderID = Convert.ToInt32(cmd.LastInsertedId);
                        }

                        // Insert every selected test with its current price.
                        foreach (int testID in tests)
                        {
                            using (var cmd = new MySqlCommand(@"
                        INSERT INTO labrequests
                        (
                            LabOrderID,
                            PatientID,
                            VisitID,
                            TestID,
                            Status,
                            SavedTestPrice
                        )
                        SELECT
                            @LabOrderID,
                            @PatientID,
                            @VisitID,
                            TestID,
                            'Ordered',
                            TestPrice
                        FROM laboratorytests
                        WHERE TestID = @TestID
                          AND IsActive = 1;",
                                con, tx))
                            {
                                cmd.Parameters.AddWithValue("@LabOrderID", orderID);
                                cmd.Parameters.AddWithValue("@PatientID", patientID);
                                cmd.Parameters.AddWithValue(
                                    "@VisitID", (object)visitID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@TestID", testID);

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new InvalidOperationException(
                                        "Laboratory test " + testID +
                                        " is missing or inactive.");
                            }
                        }

                        tx.Commit();
                        return orderID;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }


        public static int GetOrCreateLabOrderBill(int labOrderID)
        {
            if (labOrderID <= 0)
                throw new ArgumentException("Invalid laboratory order.");

            using (var con = GetConnection())
            {
                con.Open();

                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        int patientID;
                        int? visitID;
                        int? existingBillID;

                        // Lock the order to prevent duplicate invoices.
                        using (var cmd = new MySqlCommand(@"
                    SELECT
                        PatientID,
                        VisitID
                    FROM laborders
                    WHERE LabOrderID = @OrderID
                    FOR UPDATE;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", labOrderID);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception("Lab order not found.");

                                patientID = Convert.ToInt32(reader["PatientID"]);

                                visitID = reader["VisitID"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["VisitID"]);
                            }
                        }

                        // Check for an existing invoice.
                        using (var cmd = new MySqlCommand(@"
                    SELECT BillID
                    FROM bills
                    WHERE LabOrderID = @OrderID
                    LIMIT 1;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", labOrderID);
                            object result = cmd.ExecuteScalar();

                            existingBillID = result == null ||
                                             result == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(result);
                        }

                        if (existingBillID.HasValue)
                        {
                            tx.Commit();
                            return existingBillID.Value;
                        }

                        // Prevent attaching an additional invoice to
                        // requests that have already been billed.
                        using (var cmd = new MySqlCommand(@"
                    SELECT COUNT(*)
                    FROM labrequests
                    WHERE LabOrderID = @OrderID
                      AND BillID IS NOT NULL;",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", labOrderID);

                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                throw new InvalidOperationException(
                                    "Some tests are already linked to an invoice.");
                        }

                        decimal total;

                        using (var cmd = new MySqlCommand(@"
                    SELECT
                        COUNT(*) AS TestCount,
                        COALESCE(SUM(SavedTestPrice), 0) AS Total
                    FROM labrequests
                    WHERE LabOrderID = @OrderID
                      AND Status <> 'Cancelled';",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", labOrderID);

                            using (var reader = cmd.ExecuteReader())
                            {
                                reader.Read();

                                int count = Convert.ToInt32(reader["TestCount"]);
                                total = Convert.ToDecimal(reader["Total"]);

                                if (count == 0 || total <= 0)
                                    throw new InvalidOperationException(
                                        "No billable tests found.");
                            }
                        }

                        int billID;

                        using (var cmd = new MySqlCommand(@"
                    INSERT INTO bills
                    (
                        PatientID,
                        VisitID,
                        LabOrderID,
                        PaymentStatus,
                        TotalAmount
                    )
                    VALUES
                    (
                        @PatientID,
                        @VisitID,
                        @LabOrderID,
                        'Unpaid',
                        @Total
                    );", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@PatientID", patientID);
                            cmd.Parameters.AddWithValue(
                                "@VisitID", (object)visitID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@LabOrderID", labOrderID);
                            cmd.Parameters.AddWithValue("@Total", total);

                            cmd.ExecuteNonQuery();
                            billID = Convert.ToInt32(cmd.LastInsertedId);
                        }

                        using (var cmd = new MySqlCommand(@"
                    UPDATE bills
                    SET BillNumber = CONCAT(
                        'INV-', YEAR(BillDate), '-',
                        LPAD(BillID, 4, '0'))
                    WHERE BillID = @BillID;",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand(@"
                    UPDATE labrequests
                    SET BillID = @BillID
                    WHERE LabOrderID = @OrderID
                      AND Status <> 'Cancelled'
                      AND BillID IS NULL;",
                            con, tx))
                        {
                            cmd.Parameters.AddWithValue("@BillID", billID);
                            cmd.Parameters.AddWithValue("@OrderID", labOrderID);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return billID;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

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
                b.TotalAmount,
                COALESCE(
                    (
                        SELECT SUM(p.PaymentAmount)
                        FROM payments p
                        WHERE p.BillID = b.BillID
                    ), 0
                ) AS PaidAmount
            FROM labrequests lr
            INNER JOIN bills b
                ON b.BillID = lr.BillID
            WHERE lr.LabRequestID = @RequestID
              AND lr.Status <> 'Cancelled'
            LIMIT 1;", connection))
                {
                    cmd.Parameters.AddWithValue("@RequestID", requestID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return false;

                        decimal total = Convert.ToDecimal(
                            reader["TotalAmount"]);

                        decimal paid = Convert.ToDecimal(
                            reader["PaidAmount"]);

                        return total > 0 && paid >= total;
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
