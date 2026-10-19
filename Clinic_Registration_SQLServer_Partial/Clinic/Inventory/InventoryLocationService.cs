using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace YamyProject
{
    internal static class InventoryLocationService
    {
        // ============================================================
        // INITIAL DATABASE SETUP
        // ============================================================

        public static void EnsureSchema()
        {
            CreateWarehouseLocationTable();

            CreateLocationStockTable();

            CreateLocationHistoryTable();

            CreateDefaultLocations();

            SeedExistingItemStock();
        }


        // ============================================================
        // WAREHOUSE LOCATIONS
        // ============================================================

        private static void CreateWarehouseLocationTable()
        {
            ClinicDatabase.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS tbl_warehouse_location
                (
                    id INT NOT NULL AUTO_INCREMENT,

                    warehouse_id INT NOT NULL DEFAULT 0,

                    code VARCHAR(50)
                        NOT NULL DEFAULT '',

                    name VARCHAR(150)
                        NOT NULL DEFAULT '',

                    active TINYINT(1)
                        NOT NULL DEFAULT 1,

                    state INT
                        NOT NULL DEFAULT 0,

                    PRIMARY KEY (id),

                    KEY ix_warehouse_location_warehouse
                        (warehouse_id)
                )
                ENGINE=InnoDB
                DEFAULT CHARSET=utf8mb4;
            ");
        }


        // ============================================================
        // CURRENT STOCK BY LOCATION
        // ============================================================

        private static void CreateLocationStockTable()
        {
            ClinicDatabase.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS tbl_inventory_location_stock
                (
                    id INT NOT NULL AUTO_INCREMENT,

                    warehouse_id INT
                        NOT NULL DEFAULT 0,

                    location_id INT
                        NOT NULL DEFAULT 0,

                    item_id INT
                        NOT NULL DEFAULT 0,

                    on_hand DECIMAL(20,4)
                        NOT NULL DEFAULT 0,

                    reserved_qty DECIMAL(20,4)
                        NOT NULL DEFAULT 0,

                    last_modified DATETIME
                        NOT NULL DEFAULT CURRENT_TIMESTAMP,

                    PRIMARY KEY (id),

                    UNIQUE KEY uq_inventory_location_stock
                    (
                        warehouse_id,
                        location_id,
                        item_id
                    ),

                    KEY ix_inventory_location_stock_item
                        (item_id),

                    KEY ix_inventory_location_stock_location
                        (location_id)
                )
                ENGINE=InnoDB
                DEFAULT CHARSET=utf8mb4;
            ");
        }


        // ============================================================
        // MOVEMENT HISTORY
        // ============================================================

        private static void CreateLocationHistoryTable()
        {
            ClinicDatabase.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS tbl_inventory_location_history
                (
                    id INT NOT NULL AUTO_INCREMENT,

                    movement_date DATETIME
                        NOT NULL DEFAULT CURRENT_TIMESTAMP,

                    reference_no VARCHAR(100)
                        NOT NULL DEFAULT '',

                    reference_type VARCHAR(50)
                        NOT NULL DEFAULT '',

                    item_id INT
                        NOT NULL DEFAULT 0,

                    from_warehouse_id INT
                        NOT NULL DEFAULT 0,

                    from_location_id INT
                        NOT NULL DEFAULT 0,

                    to_warehouse_id INT
                        NOT NULL DEFAULT 0,

                    to_location_id INT
                        NOT NULL DEFAULT 0,

                    quantity DECIMAL(20,4)
                        NOT NULL DEFAULT 0,

                    status VARCHAR(30)
                        NOT NULL DEFAULT 'Done',

                    created_by INT
                        NOT NULL DEFAULT 0,

                    note VARCHAR(500)
                        NOT NULL DEFAULT '',

                    PRIMARY KEY (id),

                    KEY ix_inventory_location_history_item
                        (item_id),

                    KEY ix_inventory_location_history_date
                        (movement_date)
                )
                ENGINE=InnoDB
                DEFAULT CHARSET=utf8mb4;
            ");
        }


        // ============================================================
        // CREATE STOCK LOCATION FOR EVERY WAREHOUSE
        // ============================================================

        private static void CreateDefaultLocations()
        {
            ClinicDatabase.ExecuteNonQuery(@"
                INSERT INTO tbl_warehouse_location
                (
                    warehouse_id,
                    code,
                    name,
                    active,
                    state
                )

                SELECT
                    w.id,
                    'STOCK',
                    'Stock',
                    1,
                    0

                FROM tbl_warehouse w

                WHERE NOT EXISTS
                (
                    SELECT 1

                    FROM tbl_warehouse_location l

                    WHERE l.warehouse_id = w.id

                      AND l.code = 'STOCK'

                      AND l.state = 0
                );
            ");
        }


        // ============================================================
        // COPY EXISTING tbl_items.on_hand INTO LOCATIONS
        // ============================================================

        private static void SeedExistingItemStock()
        {
            /*
             * IMPORTANT:
             *
             * Existing items already have an on_hand value.
             *
             * Before Locations was enabled that stock had no
             * physical location.
             *
             * This places the old quantity into that warehouse's
             * STOCK location.
             *
             * It only runs for items that DO NOT already have
             * location rows, therefore reopening Locations does
             * not duplicate stock.
             */

            ClinicDatabase.ExecuteNonQuery(@"
                INSERT INTO tbl_inventory_location_stock
                (
                    warehouse_id,
                    location_id,
                    item_id,
                    on_hand,
                    reserved_qty,
                    last_modified
                )

                SELECT
                    selectedWarehouse.warehouse_id,

                    selectedLocation.location_id,

                    i.id,

                    IFNULL(i.on_hand, 0),

                    0,

                    NOW()

                FROM tbl_items i


                INNER JOIN
                (
                    SELECT
                        i2.id AS item_id,

                        CASE
                            WHEN IFNULL(i2.warehouse_id, 0) > 0
                                THEN i2.warehouse_id

                            ELSE
                                (
                                    SELECT MIN(w2.id)
                                    FROM tbl_warehouse w2
                                )
                        END AS warehouse_id

                    FROM tbl_items i2

                ) selectedWarehouse

                    ON selectedWarehouse.item_id =
                       i.id


                INNER JOIN
                (
                    SELECT
                        i3.id AS item_id,

                        (
                            SELECT MIN(l.id)

                            FROM tbl_warehouse_location l

                            WHERE l.warehouse_id =
                            CASE
                                WHEN IFNULL(i3.warehouse_id, 0) > 0
                                    THEN i3.warehouse_id

                                ELSE
                                    (
                                        SELECT MIN(w3.id)
                                        FROM tbl_warehouse w3
                                    )
                            END

                            AND l.state = 0
                        ) AS location_id

                    FROM tbl_items i3

                ) selectedLocation

                    ON selectedLocation.item_id =
                       i.id


                WHERE selectedWarehouse.warehouse_id > 0

                  AND selectedLocation.location_id > 0

                  AND NOT EXISTS
                  (
                      SELECT 1

                      FROM tbl_inventory_location_stock s

                      WHERE s.item_id = i.id
                  );
            ");
        }


        // ============================================================
        // MAIN LOCATIONS PAGE
        // ============================================================

        public static DataTable GetLocationStock(
            string searchText = "")
        {
            string sql = @"
                SELECT
                    s.id,

                    s.warehouse_id,

                    s.location_id,

                    s.item_id,


                    CONCAT(
                        IFNULL(w.name, ''),
                        '/',
                        IFNULL(
                            NULLIF(l.code, ''),
                            l.name
                        )
                    ) AS location_name,


                    CONCAT(
                        '[',
                        IFNULL(i.code, ''),
                        '] ',
                        IFNULL(i.name, '')
                    ) AS product_name,


                    IFNULL(
                        s.on_hand,
                        0
                    ) AS on_hand,


                    IFNULL(
                        s.reserved_qty,
                        0
                    ) AS reserved_qty,


                    'Units' AS unit_name


                FROM tbl_inventory_location_stock s


                INNER JOIN tbl_items i
                    ON i.id =
                       s.item_id


                LEFT JOIN tbl_warehouse w
                    ON w.id =
                       s.warehouse_id


                LEFT JOIN tbl_warehouse_location l
                    ON l.id =
                       s.location_id


                WHERE 1 = 1
            ";


            if (!string.IsNullOrWhiteSpace(
                    searchText))
            {
                sql += @"

                    AND
                    (
                        i.code LIKE @search

                        OR i.name LIKE @search

                        OR w.name LIKE @search

                        OR l.name LIKE @search

                        OR l.code LIKE @search
                    )
                ";
            }


            sql += @"

                ORDER BY
                    w.name,
                    l.name,
                    i.name;
            ";


            if (string.IsNullOrWhiteSpace(
                    searchText))
            {
                return ClinicDatabase.ExecuteDataTable(
                    sql);
            }


            return ClinicDatabase.ExecuteDataTable(
                sql,

                ClinicDatabase.CreateParameterNew(
                    "@search",
                    "%" +
                    searchText.Trim() +
                    "%"));
        }


        // ============================================================
        // HISTORY
        // ============================================================

        public static DataTable GetHistory(
            int itemId = 0,
            int warehouseId = 0,
            int locationId = 0)
        {
            string sql = @"
                SELECT
                    h.id,

                    h.movement_date,

                    h.reference_no,

                    h.reference_type,


                    CONCAT(
                        '[',
                        IFNULL(i.code, ''),
                        '] ',
                        IFNULL(i.name, '')
                    ) AS product_name,


                    CASE

                        WHEN h.from_location_id = 0

                        THEN
                            CASE
                                WHEN h.reference_type =
                                     'Purchase Receipt'
                                THEN 'Vendor'

                                ELSE
                                    'Virtual Locations/Inventory adjustment'
                            END

                        ELSE

                            CONCAT(
                                IFNULL(fw.name, ''),
                                '/',
                                IFNULL(
                                    NULLIF(fl.code, ''),
                                    fl.name
                                )
                            )

                    END AS from_location,


                    CONCAT(
                        IFNULL(tw.name, ''),
                        '/',
                        IFNULL(
                            NULLIF(tl.code, ''),
                            tl.name
                        )
                    ) AS to_location,


                    h.quantity,

                    'Units' AS unit_name,

                    h.status


                FROM tbl_inventory_location_history h


                INNER JOIN tbl_items i
                    ON i.id =
                       h.item_id


                LEFT JOIN tbl_warehouse fw
                    ON fw.id =
                       h.from_warehouse_id


                LEFT JOIN tbl_warehouse_location fl
                    ON fl.id =
                       h.from_location_id


                LEFT JOIN tbl_warehouse tw
                    ON tw.id =
                       h.to_warehouse_id


                LEFT JOIN tbl_warehouse_location tl
                    ON tl.id =
                       h.to_location_id


                WHERE 1 = 1
            ";


            List<MySqlParameter> parameters =
                new List<MySqlParameter>();


            if (itemId > 0)
            {
                sql += @"
                    AND h.item_id =
                        @itemId
                ";

                parameters.Add(
                    ClinicDatabase.CreateParameterNew(
                        "@itemId",
                        itemId));
            }


            if (warehouseId > 0)
            {
                sql += @"

                    AND
                    (
                        h.from_warehouse_id =
                            @warehouseId

                        OR

                        h.to_warehouse_id =
                            @warehouseId
                    )
                ";

                parameters.Add(
                    ClinicDatabase.CreateParameterNew(
                        "@warehouseId",
                        warehouseId));
            }


            if (locationId > 0)
            {
                sql += @"

                    AND
                    (
                        h.from_location_id =
                            @locationId

                        OR

                        h.to_location_id =
                            @locationId
                    )
                ";

                parameters.Add(
                    ClinicDatabase.CreateParameterNew(
                        "@locationId",
                        locationId));
            }


            sql += @"

                ORDER BY
                    h.movement_date DESC,
                    h.id DESC;
            ";


            return ClinicDatabase.ExecuteDataTable(
                sql,
                parameters.ToArray());
        }


        // ============================================================
        // APPLY INVENTORY COUNT
        // ============================================================

        public static void ApplyInventoryAdjustment(
            int warehouseId,
            int locationId,
            int itemId,
            decimal countedQty,
            int userId)
        {
            using (
                MySqlConnection conn =
                ClinicDatabase.GetConnection())
            {
                conn.Open();


                using (
                    MySqlTransaction trans =
                    conn.BeginTransaction())
                {
                    try
                    {
                        decimal oldQty =
                            0;


                        // --------------------------------------------
                        // CURRENT LOCATION QUANTITY
                        // --------------------------------------------

                        using (
                            MySqlCommand cmd =
                            new MySqlCommand(
                                @"
                                SELECT
                                    IFNULL(
                                        on_hand,
                                        0
                                    )

                                FROM
                                    tbl_inventory_location_stock

                                WHERE
                                    warehouse_id =
                                        @warehouseId

                                AND
                                    location_id =
                                        @locationId

                                AND
                                    item_id =
                                        @itemId

                                FOR UPDATE;
                                ",

                                conn,
                                trans))
                        {
                            cmd.Parameters
                                .AddWithValue(
                                    "@warehouseId",
                                    warehouseId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@locationId",
                                    locationId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@itemId",
                                    itemId);


                            object result =
                                cmd.ExecuteScalar();


                            if (result != null &&
                                result != DBNull.Value)
                            {
                                oldQty =
                                    Convert.ToDecimal(
                                        result);
                            }
                        }


                        decimal difference =
                            countedQty -
                            oldQty;


                        // --------------------------------------------
                        // UPDATE LOCATION STOCK
                        // --------------------------------------------

                        using (
                            MySqlCommand cmd =
                            new MySqlCommand(
                                @"
                                INSERT INTO
                                    tbl_inventory_location_stock
                                (
                                    warehouse_id,
                                    location_id,
                                    item_id,
                                    on_hand,
                                    reserved_qty,
                                    last_modified
                                )
                                VALUES
                                (
                                    @warehouseId,
                                    @locationId,
                                    @itemId,
                                    @countedQty,
                                    0,
                                    NOW()
                                )

                                ON DUPLICATE KEY UPDATE

                                    on_hand =
                                        @countedQty,

                                    last_modified =
                                        NOW();
                                ",

                                conn,
                                trans))
                        {
                            cmd.Parameters
                                .AddWithValue(
                                    "@warehouseId",
                                    warehouseId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@locationId",
                                    locationId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@itemId",
                                    itemId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@countedQty",
                                    countedQty);


                            cmd.ExecuteNonQuery();
                        }


                        // --------------------------------------------
                        // UPDATE MASTER ITEM ON HAND
                        // --------------------------------------------

                        using (
                            MySqlCommand cmd =
                            new MySqlCommand(
                                @"
                                UPDATE tbl_items i

                                SET
                                    i.on_hand =
                                    (
                                        SELECT
                                            IFNULL(
                                                SUM(s.on_hand),
                                                0
                                            )

                                        FROM
                                            tbl_inventory_location_stock s

                                        WHERE
                                            s.item_id =
                                            @itemId
                                    ),

                                    i.total_value =
                                    (
                                        SELECT
                                            IFNULL(
                                                SUM(s.on_hand),
                                                0
                                            )

                                        FROM
                                            tbl_inventory_location_stock s

                                        WHERE
                                            s.item_id =
                                            @itemId
                                    )
                                    *
                                    IFNULL(
                                        i.cost_price,
                                        0
                                    )

                                WHERE
                                    i.id =
                                    @itemId;
                                ",

                                conn,
                                trans))
                        {
                            cmd.Parameters
                                .AddWithValue(
                                    "@itemId",
                                    itemId);


                            cmd.ExecuteNonQuery();
                        }


                        // --------------------------------------------
                        // HISTORY
                        // --------------------------------------------

                        string referenceNo =
                            "INV/ADJ/" +
                            DateTime.Now.ToString(
                                "yyyyMMddHHmmss");


                        using (
                            MySqlCommand cmd =
                            new MySqlCommand(
                                @"
                                INSERT INTO
                                    tbl_inventory_location_history
                                (
                                    movement_date,
                                    reference_no,
                                    reference_type,
                                    item_id,

                                    from_warehouse_id,
                                    from_location_id,

                                    to_warehouse_id,
                                    to_location_id,

                                    quantity,

                                    status,

                                    created_by,

                                    note
                                )
                                VALUES
                                (
                                    NOW(),

                                    @referenceNo,

                                    'Inventory Adjustment',

                                    @itemId,

                                    0,

                                    0,

                                    @warehouseId,

                                    @locationId,

                                    @difference,

                                    'Done',

                                    @createdBy,

                                    @note
                                );
                                ",

                                conn,
                                trans))
                        {
                            cmd.Parameters
                                .AddWithValue(
                                    "@referenceNo",
                                    referenceNo);


                            cmd.Parameters
                                .AddWithValue(
                                    "@itemId",
                                    itemId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@warehouseId",
                                    warehouseId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@locationId",
                                    locationId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@difference",
                                    difference);


                            cmd.Parameters
                                .AddWithValue(
                                    "@createdBy",
                                    userId);


                            cmd.Parameters
                                .AddWithValue(
                                    "@note",

                                    "Product Quantity Updated. " +

                                    "Old Quantity: " +

                                    oldQty.ToString(
                                        "0.####") +

                                    ", Counted Quantity: " +

                                    countedQty.ToString(
                                        "0.####") +

                                    ", Difference: " +

                                    difference.ToString(
                                        "0.####"));


                            cmd.ExecuteNonQuery();
                        }


                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();

                        throw;
                    }
                }
            }
        }


        // ============================================================
        // PURCHASE RECEIPT -> LOCATION STOCK
        // ============================================================

        public static void AddReceiptStock(
            MySqlConnection conn,
            MySqlTransaction trans,
            int warehouseId,
            int locationId,
            int itemId,
            decimal quantity,
            string referenceNo,
            int userId)
        {
            if (quantity <= 0 ||
                warehouseId <= 0 ||
                locationId <= 0)
            {
                return;
            }

            locationId =
    InventoryPutawayService.ResolveDestinationLocation(
        conn,
        trans,
        warehouseId,
        locationId,
        itemId);

            // --------------------------------------------
            // Increase location stock
            // --------------------------------------------

            using (
                MySqlCommand cmd =
                new MySqlCommand(
                    @"
                    INSERT INTO
                        tbl_inventory_location_stock
                    (
                        warehouse_id,
                        location_id,
                        item_id,
                        on_hand,
                        reserved_qty,
                        last_modified
                    )
                    VALUES
                    (
                        @warehouseId,
                        @locationId,
                        @itemId,
                        @quantity,
                        0,
                        NOW()
                    )

                    ON DUPLICATE KEY UPDATE

                        on_hand =
                            on_hand +
                            @quantity,

                        last_modified =
                            NOW();
                    ",

                    conn,
                    trans))
            {
                cmd.Parameters.AddWithValue(
                    "@warehouseId",
                    warehouseId);


                cmd.Parameters.AddWithValue(
                    "@locationId",
                    locationId);


                cmd.Parameters.AddWithValue(
                    "@itemId",
                    itemId);


                cmd.Parameters.AddWithValue(
                    "@quantity",
                    quantity);


                cmd.ExecuteNonQuery();
            }


            // --------------------------------------------
            // History
            // --------------------------------------------

            using (
                MySqlCommand cmd =
                new MySqlCommand(
                    @"
                    INSERT INTO
                        tbl_inventory_location_history
                    (
                        movement_date,
                        reference_no,
                        reference_type,
                        item_id,

                        from_warehouse_id,
                        from_location_id,

                        to_warehouse_id,
                        to_location_id,

                        quantity,
                        status,
                        created_by,
                        note
                    )
                    VALUES
                    (
                        NOW(),

                        @referenceNo,

                        'Purchase Receipt',

                        @itemId,

                        0,
                        0,

                        @warehouseId,
                        @locationId,

                        @quantity,

                        'Done',

                        @createdBy,

                        'Purchase Receipt'
                    );
                    ",

                    conn,
                    trans))
            {
                cmd.Parameters.AddWithValue(
                    "@referenceNo",
                    referenceNo ?? "");


                cmd.Parameters.AddWithValue(
                    "@itemId",
                    itemId);


                cmd.Parameters.AddWithValue(
                    "@warehouseId",
                    warehouseId);


                cmd.Parameters.AddWithValue(
                    "@locationId",
                    locationId);


                cmd.Parameters.AddWithValue(
                    "@quantity",
                    quantity);


                cmd.Parameters.AddWithValue(
                    "@createdBy",
                    userId);


                cmd.ExecuteNonQuery();
            }
        }
    }
}