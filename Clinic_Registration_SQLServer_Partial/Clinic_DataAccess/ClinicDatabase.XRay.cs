using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Clinic_DataAccess
{
    public static partial class ClinicDatabase
    {
        // Run once at application startup (or from frmXRay_Load).
        // Does not alter existing tables and is safe to rerun.
        public static void EnsureXRayTables()
        {
            using (var c = GetConnection())
            {
                c.Open();
                string[] statements = {
                    @"CREATE TABLE IF NOT EXISTS xraytests (
                        XRayTestID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        TestName VARCHAR(200) NOT NULL,
                        TestPrice DECIMAL(12,2) NOT NULL DEFAULT 0,
                        IsActive TINYINT(1) NOT NULL DEFAULT 1,
                        UNIQUE KEY UQ_XRayTests_Name (TestName)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
                    @"CREATE TABLE IF NOT EXISTS xrayrequests (
                        XRayRequestID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        VisitID INT NOT NULL,
                        XRayTestID INT NOT NULL,
                        RequestedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        Status VARCHAR(30) NOT NULL DEFAULT 'Ordered',
                        ClinicalNotes TEXT NULL,
                        Findings TEXT NULL,
                        Impression TEXT NULL,
                        ImagePath VARCHAR(1000) NULL,
                        ResultedAt DATETIME NULL,
                        INDEX IX_XRayRequests_Visit (VisitID),
                        INDEX IX_XRayRequests_Test (XRayTestID),
                        INDEX IX_XRayRequests_Status (Status),
                        CONSTRAINT FK_XRayRequests_Visit FOREIGN KEY (VisitID) REFERENCES visits(VisitID),
                        CONSTRAINT FK_XRayRequests_Test FOREIGN KEY (XRayTestID) REFERENCES xraytests(XRayTestID)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"
                };
                foreach (string sql in statements)
                    using (var cmd = new MySqlCommand(sql, c)) cmd.ExecuteNonQuery();
            }
        }

        private static DataTable XRayTable(string sql, params MySqlParameter[] parameters)
        {
            using (var c = GetConnection())
            using (var cmd = new MySqlCommand(sql, c))
            using (var da = new MySqlDataAdapter(cmd))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private static int XRayExecute(string sql, params MySqlParameter[] parameters)
        {
            using (var c = GetConnection())
            using (var cmd = new MySqlCommand(sql, c))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                c.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private static MySqlParameter XP(string name, object value)
        {
            return new MySqlParameter(name, value ?? DBNull.Value);
        }

        public static DataTable GetXRayTests(bool activeOnly = false)
        {
            return XRayTable(@"SELECT XRayTestID, TestName, TestPrice, IsActive
                               FROM xraytests WHERE (@active = 0 OR IsActive = 1)
                               ORDER BY TestName", XP("@active", activeOnly ? 1 : 0));
        }

        public static int SaveXRayTest(int id, string name, decimal price, bool active)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Test name is required.");
            if (price < 0) throw new ArgumentException("Price cannot be negative.");
            if (id > 0)
                return XRayExecute(@"UPDATE xraytests SET TestName=@name, TestPrice=@price,
                                     IsActive=@active WHERE XRayTestID=@id",
                    XP("@name", name.Trim()), XP("@price", price), XP("@active", active ? 1 : 0), XP("@id", id));
            return XRayExecute(@"INSERT INTO xraytests(TestName,TestPrice,IsActive)
                                 VALUES(@name,@price,@active)",
                XP("@name", name.Trim()), XP("@price", price), XP("@active", active ? 1 : 0));
        }

        // Soft delete protects historical requests and reports.
        public static bool DeleteXRayTest(int id)
        {
            return XRayExecute("UPDATE xraytests SET IsActive=0 WHERE XRayTestID=@id", XP("@id", id)) > 0;
        }

        public static DataTable GetXRayVisits()
        {
            return XRayTable(@"SELECT v.VisitID, v.PatientID, pe.FullName AS PatientName,
                                     v.VisitDate, CONCAT('#',v.VisitID,' - ',pe.FullName,
                                     ' (',DATE_FORMAT(v.VisitDate,'%d %b %Y'),')') AS VisitDisplay
                               FROM visits v
                               INNER JOIN patients p ON p.PatientID=v.PatientID
                               INNER JOIN persons pe ON pe.PersonID=p.PersonID
                               ORDER BY v.VisitDate DESC, v.VisitID DESC");
        }

        public static DataTable GetXRayRequests(string search, string status,
                                                 DateTime? from = null, DateTime? to = null)
        {
            return XRayTable(@"SELECT r.XRayRequestID, r.VisitID, pe.FullName AS PatientName,
                                     t.TestName, r.RequestedAt, r.Status, r.ImagePath,
                                     r.XRayTestID, r.ClinicalNotes, r.Findings, r.Impression,
                                     r.ResultedAt
                               FROM xrayrequests r
                               JOIN visits v ON v.VisitID=r.VisitID
                               JOIN patients p ON p.PatientID=v.PatientID
                               JOIN persons pe ON pe.PersonID=p.PersonID
                               JOIN xraytests t ON t.XRayTestID=r.XRayTestID
                               WHERE (@search='' OR pe.FullName LIKE CONCAT('%',@search,'%')
                                  OR t.TestName LIKE CONCAT('%',@search,'%')
                                  OR CAST(r.VisitID AS CHAR) LIKE CONCAT('%',@search,'%'))
                                 AND (@status='All' OR r.Status=@status)
                                 AND (@from IS NULL OR r.RequestedAt>=@from)
                                 AND (@to IS NULL OR r.RequestedAt<DATE_ADD(@to, INTERVAL 1 DAY))
                               ORDER BY r.RequestedAt DESC,r.XRayRequestID DESC",
                XP("@search", (search ?? "").Trim()), XP("@status", string.IsNullOrWhiteSpace(status) ? "All" : status),
                XP("@from", from.HasValue ? (object)from.Value.Date : DBNull.Value),
                XP("@to", to.HasValue ? (object)to.Value.Date : DBNull.Value));
        }

        public static DataRow GetXRayRequest(int id)
        {
            var dt = XRayTable("SELECT * FROM xrayrequests WHERE XRayRequestID=@id", XP("@id", id));
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static DataTable GetXRayDashboardStats()
        {
            return XRayTable(@"SELECT
                (SELECT COUNT(*) FROM xraytests WHERE IsActive=1) AS TotalTests,
                COUNT(*) AS TotalRequests,
                COALESCE(SUM(Status='Ordered'),0) AS Pending,
                COALESCE(SUM(Status='In Progress'),0) AS InProgress,
                COALESCE(SUM(Status='Completed'),0) AS Completed
                FROM xrayrequests");
        }

        public static int CreateXRayRequest(int visitId, int testId, string clinicalNotes)
        {
            if (visitId < 1 || testId < 1) throw new ArgumentException("Choose a visit and X-ray test.");
            using (var c = GetConnection())
            {
                c.Open();
                using (var cmd = new MySqlCommand(@"INSERT INTO xrayrequests(VisitID,XRayTestID,ClinicalNotes)
                        SELECT @visit,@test,@notes FROM visits v
                        JOIN xraytests t ON t.XRayTestID=@test AND t.IsActive=1
                        WHERE v.VisitID=@visit", c))
                {
                    cmd.Parameters.AddRange(new[] { XP("@visit", visitId), XP("@test", testId), XP("@notes", clinicalNotes) });
                    if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Visit or active X-ray test not found.");
                    return Convert.ToInt32(cmd.LastInsertedId);
                }
            }
        }

        public static bool UpdateXRayRequest(int id, int visitId, int testId, string notes)
        {
            return XRayExecute(@"UPDATE xrayrequests r SET VisitID=@visit,XRayTestID=@test,ClinicalNotes=@notes
                                  WHERE r.XRayRequestID=@id AND r.Status='Ordered'
                                  AND EXISTS(SELECT 1 FROM visits v WHERE v.VisitID=@visit)
                                  AND EXISTS(SELECT 1 FROM xraytests t WHERE t.XRayTestID=@test AND t.IsActive=1)",
                XP("@id", id), XP("@visit", visitId), XP("@test", testId), XP("@notes", notes)) > 0;
        }

        public static bool SetXRayInProgress(int id)
        {
            return XRayExecute(@"UPDATE xrayrequests SET Status='In Progress'
                                 WHERE XRayRequestID=@id AND Status='Ordered'", XP("@id", id)) > 0;
        }

        public static bool SaveXRayResult(int id, string findings, string impression, string imagePath)
        {
            if (string.IsNullOrWhiteSpace(findings) || string.IsNullOrWhiteSpace(impression))
                throw new ArgumentException("Findings and impression are required.");
            return XRayExecute(@"UPDATE xrayrequests SET Findings=@findings, Impression=@impression,
                                ImagePath=@image,Status='Completed',ResultedAt=NOW()
                                WHERE XRayRequestID=@id AND Status IN ('Ordered','In Progress')",
                XP("@id", id), XP("@findings", findings.Trim()), XP("@impression", impression.Trim()),
                XP("@image", imagePath)) > 0;
        }

        // Delete means cancel for medical traceability; no physical record is removed.
        public static bool DeleteXRayRequest(int id)
        {
            return XRayExecute(@"UPDATE xrayrequests SET Status='Cancelled'
                                 WHERE XRayRequestID=@id AND Status='Ordered'", XP("@id", id)) > 0;
        }
    }
}
