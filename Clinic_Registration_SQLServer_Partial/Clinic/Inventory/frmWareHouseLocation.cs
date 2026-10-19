
using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using Clinic_DataAccess;

namespace YamyProject
{
    public partial class frmWarehouseLocation : Form
    {
        private int locationId;

        private bool IsArabic
        {
            get
            {
                string language =
                    Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

                return language.StartsWith(
                           "ar", StringComparison.OrdinalIgnoreCase) ||
                       language.IndexOf(
                           "arabic", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private string T(string english, string arabic)
        {
            return IsArabic ? arabic : english;
        }

        public frmWarehouseLocation(int locationId = 0)
        {
            InitializeComponent();

            this.locationId = locationId;

            ApplyArabicProperties();
        }

        private void ApplyArabicProperties()
        {
            bool arabic = IsArabic;

            // Translate the Windows title bar for the selected language.
            this.Text = arabic ? "الموقع" : "Location";

            this.RightToLeft = arabic
                ? RightToLeft.Yes
                : RightToLeft.No;

            this.RightToLeftLayout = arabic;

            TranslateControls(this);

            if (cmbLocationType != null)
            {
                string currentType =
                    LocationTypeToEnglish(cmbLocationType.Text);

                LoadLocationTypes();

                if (!string.IsNullOrWhiteSpace(currentType))
                {
                    cmbLocationType.Text =
                        LocationTypeToDisplay(currentType);
                }
            }

            if (lblTitle != null)
            {
                lblTitle.Text = locationId > 0
                    ? T(
                        "Locations / " +
                        lblTitle.Text.Replace("المواقع / ", "")
                                     .Replace("Locations / ", ""),
                        "المواقع / " +
                        lblTitle.Text.Replace("المواقع / ", "")
                                     .Replace("Locations / ", ""))
                    : T("Locations / New", "المواقع / جديد");
            }
        }

        private void TranslateControls(Control parent)
        {
            var translations = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                { "Location", "الموقع" },
                { "Locations", "المواقع" },
                { "Locations / New", "المواقع / جديد" },
                { "Save", "حفظ" },
                { "Discard", "تجاهل" },
                { "Location Name", "اسم الموقع" },
                { "Code", "الرمز" },
                { "Warehouse", "المستودع" },
                { "Parent Location", "الموقع الرئيسي" },
                { "ADDITIONAL INFORMATION", "معلومات إضافية" },
                { "Location Type", "نوع الموقع" },
                { "Is a Scrap Location?", "هل هو موقع خردة؟" },
                { "Is a Return Location?", "هل هو موقع مرتجعات؟" },
                { "Replenish Location", "موقع إعادة التزويد" },
                { "Active", "نشط" },
                { "External Note", "ملاحظات خارجية" },
                { "Putaway Rules", "قواعد التخزين" },
                { "New", "جديد" },
                { "Edit", "تعديل" }
            };

            foreach (Control control in parent.Controls)
            {
                string currentText = control.Text?.Trim() ?? "";

                if (translations.TryGetValue(
                    currentText, out string arabicText))
                {
                    control.Text = IsArabic
                        ? arabicText
                        : EnglishForArabic(
                            arabicText, translations, currentText);
                }

                if (control.HasChildren)
                {
                    TranslateControls(control);
                }
            }
        }

        private string EnglishForArabic(
            string arabic,
            Dictionary<string, string> translations,
            string fallback)
        {
            foreach (var pair in translations)
            {
                if (pair.Value == arabic)
                    return pair.Key;
            }

            return fallback;
        }

        private string LocationTypeToDisplay(string english)
        {
            if (!IsArabic)
                return english;

            switch (english)
            {
                case "Vendor Location":
                    return "موقع المورد";

                case "View":
                    return "عرض";

                case "Internal Location":
                    return "موقع داخلي";

                case "Customer Location":
                    return "موقع العميل";

                case "Inventory Loss":
                    return "خسارة المخزون";

                case "Production":
                    return "الإنتاج";

                case "Transit Location":
                    return "موقع عبور";

                default:
                    return english;
            }
        }

        private string LocationTypeToEnglish(string value)
        {
            switch (value)
            {
                case "موقع المورد":
                    return "Vendor Location";

                case "عرض":
                    return "View";

                case "موقع داخلي":
                    return "Internal Location";

                case "موقع العميل":
                    return "Customer Location";

                case "خسارة المخزون":
                    return "Inventory Loss";

                case "الإنتاج":
                    return "Production";

                case "موقع عبور":
                    return "Transit Location";

                default:
                    return value;
            }
        }

        private void frmWarehouseLocation_Load(
            object sender, EventArgs e)
        {
            try
            {
                EnsureLocationAndPutawaySchema();

                LoadWarehouses();
                LoadLocationTypes();

                ApplyArabicProperties();

                if (locationId > 0)
                {
                    LoadRecord();
                }
                else
                {
                    lblTitle.Text =
                        T("Locations / New", "المواقع / جديد");

                    chkActive.Checked = true;

                    chkScrap.Checked = false;
                    chkReturn.Checked = false;
                    chkReplenish.Checked = false;

                    cmbLocationType.SelectedItem =
                        LocationTypeToDisplay("Internal Location");

                    btnPutawayRules.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(
                        "Could not load Location.\r\n\r\n",
                        "تعذر تحميل الموقع.\r\n\r\n") + ex.Message,
                    T("Location", "الموقع"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    IsArabic
                        ? MessageBoxOptions.RightAlign |
                          MessageBoxOptions.RtlReading
                        : 0);
            }
        }

        // ============================================================
        // DATABASE SCHEMA
        // ============================================================

        private void EnsureLocationAndPutawaySchema()
        {
            ClinicDatabase.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS tbl_warehouse_location
                (
                    id INT NOT NULL AUTO_INCREMENT,
                    warehouse_id INT NOT NULL DEFAULT 0,
                    code VARCHAR(50) NOT NULL DEFAULT '',
                    name VARCHAR(150) NOT NULL DEFAULT '',
                    active TINYINT(1) NOT NULL DEFAULT 1,
                    state INT NOT NULL DEFAULT 0,
                    PRIMARY KEY (id),
                    KEY ix_warehouse_location_warehouse (warehouse_id)
                )
                ENGINE=InnoDB
                DEFAULT CHARSET=utf8mb4;
            ");

            EnsureColumn(
                "tbl_warehouse_location",
                "parent_location_id",
                "INT NOT NULL DEFAULT 0");

            EnsureColumn(
                "tbl_warehouse_location",
                "location_type",
                "VARCHAR(50) NOT NULL DEFAULT 'Internal Location'");

            EnsureColumn(
                "tbl_warehouse_location",
                "is_scrap_location",
                "TINYINT(1) NOT NULL DEFAULT 0");

            EnsureColumn(
                "tbl_warehouse_location",
                "is_return_location",
                "TINYINT(1) NOT NULL DEFAULT 0");

            EnsureColumn(
                "tbl_warehouse_location",
                "replenish_location",
                "TINYINT(1) NOT NULL DEFAULT 0");

            EnsureColumn(
                "tbl_warehouse_location",
                "external_note",
                "VARCHAR(500) NOT NULL DEFAULT ''");

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

            ClinicDatabase.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS tbl_putaway_rule
                (
                    id INT NOT NULL AUTO_INCREMENT,
                    warehouse_id INT NOT NULL DEFAULT 0,
                    source_location_id INT NOT NULL DEFAULT 0,
                    item_id INT NOT NULL DEFAULT 0,
                    item_category_id INT NOT NULL DEFAULT 0,
                    destination_location_id INT NOT NULL DEFAULT 0,
                    priority INT NOT NULL DEFAULT 10,
                    active TINYINT(1) NOT NULL DEFAULT 1,
                    state INT NOT NULL DEFAULT 0,
                    created_by INT NOT NULL DEFAULT 0,
                    created_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    modified_by INT NOT NULL DEFAULT 0,
                    modified_date DATETIME NULL,
                    PRIMARY KEY (id),
                    KEY ix_putaway_rule_warehouse (warehouse_id),
                    KEY ix_putaway_rule_source (source_location_id),
                    KEY ix_putaway_rule_item (item_id),
                    KEY ix_putaway_rule_category (item_category_id),
                    KEY ix_putaway_rule_destination (destination_location_id)
                )
                ENGINE=InnoDB
                DEFAULT CHARSET=utf8mb4;
            ");
        }

        private void EnsureColumn(
            string tableName,
            string columnName,
            string definition)
        {
            object result = ClinicDatabase.ExecuteScalar(@"
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = @tableName
                  AND COLUMN_NAME = @columnName;",
                ClinicDatabase.CreateParameterNew("@tableName", tableName),
                ClinicDatabase.CreateParameterNew("@columnName", columnName));

            int exists = Convert.ToInt32(result ?? 0);

            if (exists == 0)
            {
                ClinicDatabase.ExecuteNonQuery(
                    "ALTER TABLE `" + tableName + "` " +
                    "ADD COLUMN `" + columnName + "` " +
                    definition + ";");
            }
        }

        // ============================================================
        // MASTER DATA
        // ============================================================

        private void LoadWarehouses()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"
                SELECT id, name
                FROM tbl_warehouse
                ORDER BY name;");

            cmbWarehouse.DataSource = dt;
            cmbWarehouse.DisplayMember = "name";
            cmbWarehouse.ValueMember = "id";
            cmbWarehouse.SelectedIndex = -1;
        }

        private void LoadLocationTypes()
        {
            cmbLocationType.Items.Clear();

            cmbLocationType.Items.Add(
                LocationTypeToDisplay("Vendor Location"));

            cmbLocationType.Items.Add(
                LocationTypeToDisplay("View"));

            cmbLocationType.Items.Add(
                LocationTypeToDisplay("Internal Location"));

            cmbLocationType.Items.Add(
                LocationTypeToDisplay("Customer Location"));

            cmbLocationType.Items.Add(
                LocationTypeToDisplay("Inventory Loss"));

            cmbLocationType.Items.Add(
                LocationTypeToDisplay("Production"));

            cmbLocationType.Items.Add(
                LocationTypeToDisplay("Transit Location"));
        }

        private void cmbWarehouse_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (cmbWarehouse.SelectedValue == null ||
                cmbWarehouse.SelectedValue is DataRowView)
            {
                return;
            }

            int warehouseId;

            if (!int.TryParse(
                cmbWarehouse.SelectedValue.ToString(),
                out warehouseId))
            {
                return;
            }

            LoadParentLocations(warehouseId);
        }

        private void LoadParentLocations(int warehouseId)
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"
                SELECT
                    id,
                    CONCAT(
                        IFNULL(NULLIF(code,''), name),
                        ' - ',
                        IFNULL(name,'')
                    ) AS display_name
                FROM tbl_warehouse_location
                WHERE warehouse_id = @warehouseId
                  AND state = 0
                  AND active = 1
                  AND id <> @locationId
                ORDER BY name;",
                ClinicDatabase.CreateParameterNew("@warehouseId", warehouseId),
                ClinicDatabase.CreateParameterNew("@locationId", locationId));

            DataRow empty = dt.NewRow();
            empty["id"] = 0;
            empty["display_name"] =
                T("(No Parent)", "بدون موقع رئيسي");

            dt.Rows.InsertAt(empty, 0);

            cmbParentLocation.DataSource = dt;
            cmbParentLocation.DisplayMember = "display_name";
            cmbParentLocation.ValueMember = "id";
            cmbParentLocation.SelectedIndex = 0;
        }

        // ============================================================
        // LOAD EXISTING LOCATION
        // ============================================================

        private void LoadRecord()
        {
            DataTable dt = ClinicDatabase.ExecuteDataTable(@"
                SELECT *
                FROM tbl_warehouse_location
                WHERE id = @id
                  AND state = 0
                LIMIT 1;",
                ClinicDatabase.CreateParameterNew("@id", locationId));

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show(
                    T(
                        "Location was not found.",
                        "لم يتم العثور على الموقع."),
                    T("Location", "الموقع"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    IsArabic
                        ? MessageBoxOptions.RightAlign |
                          MessageBoxOptions.RtlReading
                        : 0);

                Close();
                return;
            }

            DataRow row = dt.Rows[0];

            int warehouseId = SafeInt(row["warehouse_id"]);

            cmbWarehouse.SelectedValue = warehouseId;
            LoadParentLocations(warehouseId);

            txtLocationName.Text =
                row["name"] == DBNull.Value
                    ? ""
                    : row["name"].ToString();

            txtCode.Text =
                row["code"] == DBNull.Value
                    ? ""
                    : row["code"].ToString();

            cmbParentLocation.SelectedValue =
                SafeInt(row["parent_location_id"]);

            cmbLocationType.Text = LocationTypeToDisplay(
                row["location_type"] == DBNull.Value
                    ? "Internal Location"
                    : row["location_type"].ToString());

            chkScrap.Checked =
                SafeBool(row["is_scrap_location"]);

            chkReturn.Checked =
                SafeBool(row["is_return_location"]);

            chkReplenish.Checked =
                SafeBool(row["replenish_location"]);

            chkActive.Checked =
                SafeBool(row["active"]);

            txtExternalNote.Text =
                row["external_note"] == DBNull.Value
                    ? ""
                    : row["external_note"].ToString();

            lblTitle.Text =
                T("Locations / ", "المواقع / ") +
                txtLocationName.Text;

            btnPutawayRules.Enabled = true;
        }

        // ============================================================
        // SAVE
        // ============================================================

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureLocationAndPutawaySchema();

                if (cmbWarehouse.SelectedValue == null ||
                    cmbWarehouse.SelectedValue is DataRowView)
                {
                    MessageBox.Show(
                        T(
                            "Please select Warehouse.",
                            "يرجى اختيار المستودع."),
                        T("Location", "الموقع"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1,
                        IsArabic
                            ? MessageBoxOptions.RightAlign |
                              MessageBoxOptions.RtlReading
                            : 0);

                    return;
                }

                if (string.IsNullOrWhiteSpace(txtLocationName.Text))
                {
                    MessageBox.Show(
                        T(
                            "Please enter Location Name.",
                            "يرجى إدخال اسم الموقع."),
                        T("Location", "الموقع"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1,
                        IsArabic
                            ? MessageBoxOptions.RightAlign |
                              MessageBoxOptions.RtlReading
                            : 0);

                    txtLocationName.Focus();
                    return;
                }

                int warehouseId =
                    Convert.ToInt32(cmbWarehouse.SelectedValue);

                int parentLocationId =
                    cmbParentLocation.SelectedValue == null
                        ? 0
                        : SafeInt(cmbParentLocation.SelectedValue);

                string name = txtLocationName.Text.Trim();

                string code = txtCode.Text.Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    code = name
                        .ToUpperInvariant()
                        .Replace(" ", "_");
                }

                string locationType =
                    string.IsNullOrWhiteSpace(cmbLocationType.Text)
                        ? "Internal Location"
                        : LocationTypeToEnglish(cmbLocationType.Text);

                object duplicate = ClinicDatabase.ExecuteScalar(@"
                    SELECT COUNT(*)
                    FROM tbl_warehouse_location
                    WHERE warehouse_id = @warehouseId
                      AND id <> @id
                      AND state = 0
                      AND
                      (
                          name = @name
                          OR code = @code
                      );",
                    ClinicDatabase.CreateParameterNew("@warehouseId", warehouseId),
                    ClinicDatabase.CreateParameterNew("@id", locationId),
                    ClinicDatabase.CreateParameterNew("@name", name),
                    ClinicDatabase.CreateParameterNew("@code", code));

                if (Convert.ToInt32(duplicate ?? 0) > 0)
                {
                    MessageBox.Show(
                        T(
                            "A location with the same name or code already exists in this warehouse.",
                            "يوجد موقع بالاسم أو الرمز نفسه في هذا المستودع."),
                        T("Location", "الموقع"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1,
                        IsArabic
                            ? MessageBoxOptions.RightAlign |
                              MessageBoxOptions.RtlReading
                            : 0);

                    return;
                }

                if (locationId <= 0)
                {
                    object newId = ClinicDatabase.ExecuteScalar(@"
                        INSERT INTO tbl_warehouse_location
                        (
                            warehouse_id,
                            parent_location_id,
                            code,
                            name,
                            location_type,
                            is_scrap_location,
                            is_return_location,
                            replenish_location,
                            external_note,
                            active,
                            state
                        )
                        VALUES
                        (
                            @warehouseId,
                            @parentLocationId,
                            @code,
                            @name,
                            @locationType,
                            @isScrap,
                            @isReturn,
                            @replenish,
                            @externalNote,
                            @active,
                            0
                        );

                        SELECT LAST_INSERT_ID();",
                        ClinicDatabase.CreateParameterNew("@warehouseId", warehouseId),
                        ClinicDatabase.CreateParameterNew("@parentLocationId", parentLocationId),
                        ClinicDatabase.CreateParameterNew("@code", code),
                        ClinicDatabase.CreateParameterNew("@name", name),
                        ClinicDatabase.CreateParameterNew("@locationType", locationType),
                        ClinicDatabase.CreateParameterNew("@isScrap", chkScrap.Checked ? 1 : 0),
                        ClinicDatabase.CreateParameterNew("@isReturn", chkReturn.Checked ? 1 : 0),
                        ClinicDatabase.CreateParameterNew("@replenish", chkReplenish.Checked ? 1 : 0),
                        ClinicDatabase.CreateParameterNew("@externalNote", txtExternalNote.Text.Trim()),
                        ClinicDatabase.CreateParameterNew("@active", chkActive.Checked ? 1 : 0));

                    locationId = Convert.ToInt32(newId);
                }
                else
                {
                    ClinicDatabase.ExecuteNonQuery(@"
                        UPDATE tbl_warehouse_location
                        SET
                            warehouse_id = @warehouseId,
                            parent_location_id = @parentLocationId,
                            code = @code,
                            name = @name,
                            location_type = @locationType,
                            is_scrap_location = @isScrap,
                            is_return_location = @isReturn,
                            replenish_location = @replenish,
                            external_note = @externalNote,
                            active = @active
                        WHERE id = @id;",
                        ClinicDatabase.CreateParameterNew("@warehouseId", warehouseId),
                        ClinicDatabase.CreateParameterNew("@parentLocationId", parentLocationId),
                        ClinicDatabase.CreateParameterNew("@code", code),
                        ClinicDatabase.CreateParameterNew("@name", name),
                        ClinicDatabase.CreateParameterNew("@locationType", locationType),
                        ClinicDatabase.CreateParameterNew("@isScrap", chkScrap.Checked ? 1 : 0),
                        ClinicDatabase.CreateParameterNew("@isReturn", chkReturn.Checked ? 1 : 0),
                        ClinicDatabase.CreateParameterNew("@replenish", chkReplenish.Checked ? 1 : 0),
                        ClinicDatabase.CreateParameterNew("@externalNote", txtExternalNote.Text.Trim()),
                        ClinicDatabase.CreateParameterNew("@active", chkActive.Checked ? 1 : 0),
                        ClinicDatabase.CreateParameterNew("@id", locationId));
                }

                txtCode.Text = code;

                lblTitle.Text =
                    T("Locations / ", "المواقع / ") + name;

                btnPutawayRules.Enabled = true;

                MessageBox.Show(
                    T(
                        "Location saved successfully.\r\n\r\nYou can now click Putaway Rules.",
                        "تم حفظ الموقع بنجاح.\r\n\r\nيمكنك الآن فتح قواعد التخزين."),
                    T("Location", "الموقع"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    IsArabic
                        ? MessageBoxOptions.RightAlign |
                          MessageBoxOptions.RtlReading
                        : 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(
                        "Could not save Location.\r\n\r\n",
                        "تعذر حفظ الموقع.\r\n\r\n") + ex.Message,
                    T("Location", "الموقع"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    IsArabic
                        ? MessageBoxOptions.RightAlign |
                          MessageBoxOptions.RtlReading
                        : 0);
            }
        }

        // ============================================================
        // PUTAWAY RULES
        // ============================================================

        private void btnPutawayRules_Click(object sender, EventArgs e)
        {
            try
            {
                if (locationId <= 0)
                {
                    MessageBox.Show(
                        T(
                            "Save this Location first.\r\n\r\nAfter saving, Putaway Rules will be enabled.",
                            "يرجى حفظ الموقع أولاً.\r\n\r\nستُفعّل قواعد التخزين بعد الحفظ."),
                        T("Putaway Rules", "قواعد التخزين"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        IsArabic
                            ? MessageBoxOptions.RightAlign |
                              MessageBoxOptions.RtlReading
                            : 0);

                    return;
                }

                EnsureLocationAndPutawaySchema();

                using (frmPutawayRules frm =
                    new frmPutawayRules(locationId))
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    T(
                        "Could not open Putaway Rules.\r\n\r\n",
                        "تعذر فتح قواعد التخزين.\r\n\r\n") + ex.Message,
                    T("Putaway Rules", "قواعد التخزين"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    IsArabic
                        ? MessageBoxOptions.RightAlign |
                          MessageBoxOptions.RtlReading
                        : 0);
            }
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int SafeInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            int result;

            return int.TryParse(value.ToString(), out result)
                ? result
                : 0;
        }

        private bool SafeBool(object value)
        {
            if (value == null || value == DBNull.Value)
                return false;

            bool boolValue;

            if (bool.TryParse(value.ToString(), out boolValue))
                return boolValue;

            int intValue;

            if (int.TryParse(value.ToString(), out intValue))
                return intValue == 1;

            return false;
        }
    }
}