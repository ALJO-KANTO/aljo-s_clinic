using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace YamyProject
{
    public static class InventoryPutawayService
    {
        public static int ResolveDestinationLocation(
            int warehouseId,
            int sourceLocationId,
            int itemId)
        {
            using (MySqlConnection conn = ClinicDatabase.GetConnection())
            {
                conn.Open();

                return ResolveDestinationLocation(
                    conn,
                    null,
                    warehouseId,
                    sourceLocationId,
                    itemId);
            }
        }

        public static int ResolveDestinationLocation(
            MySqlConnection conn,
            MySqlTransaction trans,
            int warehouseId,
            int sourceLocationId,
            int itemId)
        {
            if (warehouseId <= 0 ||
                sourceLocationId <= 0 ||
                itemId <= 0)
            {
                return sourceLocationId;
            }

            using (MySqlCommand cmd = new MySqlCommand(@"
                SELECT destination_location_id
                FROM tbl_putaway_rule
                WHERE warehouse_id = @warehouseId
                  AND source_location_id = @sourceLocationId
                  AND item_id = @itemId
                  AND active = 1
                  AND state = 0
                ORDER BY priority ASC, id ASC
                LIMIT 1;",
                conn,
                trans))
            {
                cmd.Parameters.AddWithValue("@warehouseId", warehouseId);
                cmd.Parameters.AddWithValue("@sourceLocationId", sourceLocationId);
                cmd.Parameters.AddWithValue("@itemId", itemId);

                object result = cmd.ExecuteScalar();

                if (result != null &&
                    result != DBNull.Value &&
                    Convert.ToInt32(result) > 0)
                {
                    return Convert.ToInt32(result);
                }
            }

            int categoryId = GetItemCategoryId(
                conn,
                trans,
                itemId);

            if (categoryId > 0)
            {
                using (MySqlCommand cmd = new MySqlCommand(@"
                    SELECT destination_location_id
                    FROM tbl_putaway_rule
                    WHERE warehouse_id = @warehouseId
                      AND source_location_id = @sourceLocationId
                      AND item_id = 0
                      AND item_category_id = @categoryId
                      AND active = 1
                      AND state = 0
                    ORDER BY priority ASC, id ASC
                    LIMIT 1;",
                    conn,
                    trans))
                {
                    cmd.Parameters.AddWithValue("@warehouseId", warehouseId);
                    cmd.Parameters.AddWithValue("@sourceLocationId", sourceLocationId);
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);

                    object result = cmd.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value &&
                        Convert.ToInt32(result) > 0)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return sourceLocationId;
        }

        private static int GetItemCategoryId(
            MySqlConnection conn,
            MySqlTransaction trans,
            int itemId)
        {
            string[] possibleColumns =
            {
                "category_id",
                "cat_id",
                "item_category_id"
            };

            foreach (string columnName in possibleColumns)
            {
                bool exists;

                using (MySqlCommand cmd = new MySqlCommand(@"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'tbl_items'
                      AND COLUMN_NAME = @columnName;",
                    conn,
                    trans))
                {
                    cmd.Parameters.AddWithValue("@columnName", columnName);
                    exists = Convert.ToInt32(cmd.ExecuteScalar() ?? 0) > 0;
                }

                if (!exists)
                    continue;

                using (MySqlCommand cmd = new MySqlCommand(
                    "SELECT IFNULL(`" + columnName + "`,0) " +
                    "FROM tbl_items WHERE id = @itemId LIMIT 1;",
                    conn,
                    trans))
                {
                    cmd.Parameters.AddWithValue("@itemId", itemId);

                    object result = cmd.ExecuteScalar();

                    if (result != null &&
                        result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return 0;
        }

        public static DataTable GetRules(
            int sourceLocationId = 0)
        {
            string sql = @"
                SELECT
                    r.id,
                    r.warehouse_id,
                    r.source_location_id,
                    r.item_id,
                    r.item_category_id,
                    r.destination_location_id,
                    r.priority,
                    r.active
                FROM tbl_putaway_rule r
                WHERE r.state = 0";

            if (sourceLocationId > 0)
            {
                sql += " AND r.source_location_id = @sourceLocationId";
            }

            sql += " ORDER BY r.priority, r.id;";

            if (sourceLocationId > 0)
            {
                return ClinicDatabase.ExecuteDataTable(
                    sql,
                    ClinicDatabase.CreateParameterNew(
                        "@sourceLocationId",
                        sourceLocationId));
            }

            return ClinicDatabase.ExecuteDataTable(sql);
        }
    }
}
