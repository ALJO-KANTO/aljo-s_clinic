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
            ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS labrequests (
                LabRequestID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                VisitID INT NOT NULL,
                TestID INT NOT NULL,
                RequestedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                Status VARCHAR(30) NOT NULL DEFAULT 'Ordered',
                ResultValue TEXT NULL,
                ResultNotes TEXT NULL,
                ResultedAt DATETIME NULL,
                INDEX IX_LabRequests_Visit (VisitID),
                INDEX IX_LabRequests_Test (TestID),
                INDEX IX_LabRequests_Status (Status),
                CONSTRAINT FK_LabRequests_Visit FOREIGN KEY (VisitID) REFERENCES visits(VisitID),
                CONSTRAINT FK_LabRequests_Test FOREIGN KEY (TestID) REFERENCES laboratorytests(TestID)
            ) ENGINE=InnoDB;");
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
            if (visitID <= 0 || testID <= 0) throw new ArgumentException("Select a visit and a test.");
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var cmd = new MySqlCommand(@"INSERT INTO labrequests (VisitID, TestID)
                    SELECT @VisitID, TestID FROM laboratorytests
                    WHERE TestID=@TestID AND IsActive=1", connection))
                {
                    cmd.Parameters.AddWithValue("@VisitID", visitID);
                    cmd.Parameters.AddWithValue("@TestID", testID);
                    if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Selected test is inactive or missing.");
                    return Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public static DataTable GetLabRequests(string search = "", string status = "All")
        {
            return ExecuteDataTable(@"SELECT r.LabRequestID, r.VisitID,
                pe.FullName AS PatientName, lt.TestName,
                r.RequestedAt, r.Status, r.ResultValue, r.ResultNotes, r.ResultedAt
                FROM labrequests r
                INNER JOIN visits v ON v.VisitID=r.VisitID
                INNER JOIN patients p ON p.PatientID=v.PatientID
                INNER JOIN persons pe ON pe.PersonID=p.PersonID
                INNER JOIN laboratorytests lt ON lt.TestID=r.TestID
                WHERE (@Status='All' OR r.Status=@Status)
                  AND (@Search='' OR pe.FullName LIKE CONCAT('%',@Search,'%')
                    OR lt.TestName LIKE CONCAT('%',@Search,'%')
                    OR CAST(r.LabRequestID AS CHAR) LIKE CONCAT('%',@Search,'%'))
                ORDER BY r.RequestedAt DESC, r.LabRequestID DESC",
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

        public static bool SaveLabResult(int requestID, string result, string notes)
        {
            if (string.IsNullOrWhiteSpace(result)) throw new ArgumentException("Result is required.");
            return ExecuteNonQuery(@"UPDATE labrequests
                SET ResultValue=@Result, ResultNotes=@Notes,
                    ResultedAt=NOW(), Status='Completed'
                WHERE LabRequestID=@ID AND Status IN ('Ordered','In Progress')",
                new MySqlParameter("@Result", result.Trim()),
                new MySqlParameter("@Notes", (object)(notes ?? "") ),
                new MySqlParameter("@ID", requestID)) > 0;
        }
    }
}
