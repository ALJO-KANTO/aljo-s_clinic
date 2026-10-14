
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Clinic_DataAccess
{
    public static partial class ClinicDatabase
    {
        private class DatabaseMigration
        {
            public string Version { get; set; }
            public string Description { get; set; }
            public string Sql { get; set; }
        }

        public static void ApplyMigrations()
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                Debug.WriteLine(
                    "Migration connected to: " +
                    connection.DataSource + " / " +
                    connection.Database);

                // STEP 1: Create migration tracking table
                string createMigrationTable = @"
                    CREATE TABLE IF NOT EXISTS schema_migrations
                    (
                        version VARCHAR(100) NOT NULL PRIMARY KEY,
                        description VARCHAR(255),
                        applied_at DATETIME NOT NULL
                            DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB;";

                using (MySqlCommand command =
                    new MySqlCommand(createMigrationTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // STEP 2: Verify tracking table was created
                using (MySqlCommand command = new MySqlCommand(@"
                    SELECT COUNT(*)
                    FROM information_schema.tables
                    WHERE table_schema = DATABASE()
                      AND table_name = 'schema_migrations';",
                    connection))
                {
                    long count = Convert.ToInt64(
                        command.ExecuteScalar());

                    if (count == 0)
                    {
                        throw new Exception(
                            "schema_migrations table was not created.");
                    }

                    Debug.WriteLine(
                        "schema_migrations exists: True");
                }

                // STEP 3: Define database migrations
                // Add new migrations to this list.
                List<DatabaseMigration> migrations =
      new List<DatabaseMigration>
      {
        new DatabaseMigration
        {
            Version = "20261013_001",
            Description = "Create laboratory tests table",
            Sql = @"
                CREATE TABLE IF NOT EXISTS laboratorytests
                (
                    TestID INT AUTO_INCREMENT PRIMARY KEY,
                    TestName VARCHAR(200) NOT NULL,
                    TestPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
                    IsActive TINYINT(1) NOT NULL DEFAULT 1
                ) ENGINE=InnoDB;"
        },
        new DatabaseMigration
{
    Version = "20261013_002",
    Description = "Create visit services table",
    Sql = @"
        CREATE TABLE IF NOT EXISTS visitservices
        (
            VisitServiceID INT AUTO_INCREMENT PRIMARY KEY,
            VisitID INT NOT NULL,
            ServiceID INT NOT NULL,
            SavedServicePrice DECIMAL(18,2) NOT NULL DEFAULT 0,
            Quantity DECIMAL(18,3) NOT NULL DEFAULT 1,
            Discount DECIMAL(18,2) NOT NULL DEFAULT 0,

            INDEX IX_VisitServices_VisitID (VisitID),
            INDEX IX_VisitServices_ServiceID (ServiceID)
        ) ENGINE=InnoDB;"
},
       
        new DatabaseMigration
{
    Version = "20261014_003",
    Description = "Create billing list view",
    Sql = @"
        CREATE OR REPLACE VIEW View_Bills AS
        SELECT
            B.BillID,
            CONCAT(
                'INV-',
                YEAR(B.BillDate),
                '-',
                LPAD(B.BillID, 4, '0')
            ) AS BillNumber,
            B.VisitID,
            V.PatientID,
            P.PersonID,
            PE.FullName AS PatientName,
            V.DoctorID,
            V.AppointmentID,
            B.BillDate,
            B.TotalAmount,
            B.PaymentStatus,
            B.CreatedByUserID,
            COALESCE(PAY.PaymentAmount, 0) AS PaidAmount,
            GREATEST(
                B.TotalAmount - COALESCE(PAY.PaymentAmount, 0),
                0
            ) AS BalanceDue
        FROM bills B
        INNER JOIN visits V
            ON B.VisitID = V.VisitID
        INNER JOIN patients P
            ON V.PatientID = P.PatientID
        INNER JOIN persons PE
            ON P.PersonID = PE.PersonID
        LEFT JOIN
        (
            SELECT
                BillID,
                SUM(PaymentAmount) AS PaymentAmount
            FROM payments
            GROUP BY BillID
        ) PAY
            ON B.BillID = PAY.BillID;"
},
new DatabaseMigration
{
    Version = "20261014_004",
    Description = "Synchronize fully paid bills with visit status",
    Sql = @"
        UPDATE visits V
        INNER JOIN bills B
            ON B.VisitID = V.VisitID
        LEFT JOIN
        (
            SELECT
                BillID,
                SUM(PaymentAmount) AS PaidAmount
            FROM payments
            GROUP BY BillID
        ) PAY
            ON PAY.BillID = B.BillID
        SET V.VisitStatus = 'Completed'
        WHERE V.VisitStatus = 'Pending_Billing'
          AND B.PaymentStatus = 'Paid'
          AND COALESCE(PAY.PaidAmount, 0)
              >= B.TotalAmount;"
}

      };

                // STEP 4: Apply pending migrations
                foreach (DatabaseMigration migration in migrations)
                {
                    // Check if this migration was already applied
                    using (MySqlCommand command = new MySqlCommand(@"
                        SELECT COUNT(*)
                        FROM schema_migrations
                        WHERE version = @version;",
                        connection))
                    {
                        command.Parameters.AddWithValue(
                            "@version", migration.Version);

                        long count = Convert.ToInt64(
                            command.ExecuteScalar());

                        if (count > 0)
                        {
                            Debug.WriteLine(
                                "Already applied: " +
                                migration.Version);

                            continue;
                        }
                    }

                    // Execute migration SQL
                    using (MySqlCommand command =
                        new MySqlCommand(migration.Sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Save migration history after successful execution
                    using (MySqlCommand command = new MySqlCommand(@"
                        INSERT INTO schema_migrations
                            (version, description)
                        VALUES
                            (@version, @description);",
                        connection))
                    {
                        command.Parameters.AddWithValue(
                            "@version", migration.Version);

                        command.Parameters.AddWithValue(
                            "@description", migration.Description);

                        command.ExecuteNonQuery();
                    }

                    Debug.WriteLine(
                        "Migration applied: " +
                        migration.Version);
                }

                Debug.WriteLine(
                    "All database migrations completed.");
            }
        }

    }
}
