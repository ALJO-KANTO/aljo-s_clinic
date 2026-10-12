
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Clinic_DataAccess
{
    public enum SqlBulkCopyOptions
    {
        Default = 0
    }

    public class SqlBulkCopy : IDisposable
    {
        private readonly MySqlConnection _connection;
        private readonly MySqlTransaction _transaction;

        public string DestinationTableName { get; set; }

        public List<ColumnMapping> ColumnMappings { get; }
            = new List<ColumnMapping>();

        public SqlBulkCopy(
            MySqlConnection connection,
            SqlBulkCopyOptions options,
            MySqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void WriteToServer(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(DestinationTableName))
                throw new InvalidOperationException(
                    "Destination table is required.");

            if (ColumnMappings.Count == 0)
                throw new InvalidOperationException(
                    "Column mappings are required.");

            string QuoteIdentifier(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Invalid SQL identifier.");

                return "`" + name.Replace("`", "``") + "`";
            }

            string columns = "";
            string values = "";

            for (int i = 0; i < ColumnMappings.Count; i++)
            {
                if (i > 0)
                {
                    columns += ",";
                    values += ",";
                }

                columns += QuoteIdentifier(
                    ColumnMappings[i].DestinationColumn);

                values += "@p" + i;
            }

            string sql =
                "INSERT INTO " +
                QuoteIdentifier(DestinationTableName) +
                " (" + columns + ") VALUES (" + values + ")";

            using (MySqlCommand cmd =
                new MySqlCommand(sql, _connection, _transaction))
            {
                for (int i = 0; i < ColumnMappings.Count; i++)
                    cmd.Parameters.AddWithValue(
                        "@p" + i, DBNull.Value);

                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < ColumnMappings.Count; i++)
                    {
                        string source =
                            ColumnMappings[i].SourceColumn;

                        cmd.Parameters["@p" + i].Value =
                            row.IsNull(source)
                                ? (object)DBNull.Value
                                : row[source];
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            // The caller owns the connection and transaction.
        }

        public class ColumnMapping
        {
            public string SourceColumn { get; set; }
            public string DestinationColumn { get; set; }
        }
    }

    public static class BulkCopyMappingExtensions
    {
        public static void Add(
            this List<SqlBulkCopy.ColumnMapping> mappings,
            string source,
            string destination)
        {
            mappings.Add(new SqlBulkCopy.ColumnMapping
            {
                SourceColumn = source,
                DestinationColumn = destination
            });
        }
    }
}
