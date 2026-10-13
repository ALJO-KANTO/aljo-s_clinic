
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
