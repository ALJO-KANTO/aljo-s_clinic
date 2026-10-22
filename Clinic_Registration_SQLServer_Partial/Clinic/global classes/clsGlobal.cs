using Clinic.Login;
using Clinic_Business;
using Clinic_DataAccess;
using Guna.UI2.WinForms;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    public static  class clsGlobal
    {
        public enum UserRole { Admin = 1, Doctor = 2, Nurse = 3, Receptionist = 4, Staff = 5,Pharmacist= 6, Cashier = 7 }

        public static clsUser  CurrentUser;

        public static string PersonName {  get; set; }
        public static bool RememberUsernameAndPassword(string Username, string Password)
        {
            try
            {
                //this will get the current project directory folder.
                string currentDirectory = System.IO.Directory.GetCurrentDirectory();


                // Define the path to the text file where you want to save the data
                string filePath = currentDirectory + "\\data.txt";

                //incase the username is empty, delete the file
                if (Username=="" && File.Exists(filePath)) 
                { 
                     File.Delete(filePath);
                    return true;

                }

                // concatonate username and passwrod withe seperator.
                string dataToSave = Username + "#//#"+Password ;

                // Create a StreamWriter to write to the file
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the data to the file
                    writer.WriteLine(dataToSave);
                   
                  return true;
                }
            }
            catch (IOException ex)
            {
               MessageBox.Show ($"An error occurred: {ex.Message}");
                return false;
            }

        }
        public static bool GetStoredCredential(ref string Username, ref string Password)
        {
            //this will get the stored username and password and will return true if found and false if not found.

 
            try
            {
                //gets the current project's directory
                string currentDirectory = System.IO.Directory.GetCurrentDirectory();

                // Path for the file that contains the credential.
                string filePath  = currentDirectory + "\\data.txt";

                // Check if the file exists before attempting to read it
                if (File.Exists(filePath))
                {
                    // Create a StreamReader to read from the file
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        // Read data line by line until the end of the file
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(line); // Output each line of data to the console
                            string[] result = line.Split(new string[] { "#//#" }, StringSplitOptions.None);

                            Username = result[0];
                            Password = result[1];
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show ($"An error occurred: {ex.Message}");
                return false;   
            }

        }

       private static string keyPath = @"HKEY_CURRENT_USER\SOFTWARE\ClinicSystem";
        public static bool SaveUserCredentialsToRegistry(string username, string password)
        {
            // المسار الخاص بتطبيقك في سجل النظام
          

            try
            {
                // 1. منطق الحذف (إذا كان اسم المستخدم فارغاً، نحذف البيانات)
                if (string.IsNullOrEmpty(username))
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ClinicSystem", true))
                    {
                        if (key != null)
                        {
                            if (key.GetValue("StoredUsername") != null) key.DeleteValue("StoredUsername");
                            if (key.GetValue("StoredPassword") != null) key.DeleteValue("StoredPassword");
                            return true;
                        }
                    }
                    return false;
                }

                // 2. منطق الحفظ (نخزن اسم المستخدم وكلمة المرور)
                Registry.SetValue(keyPath, "StoredUsername", username, RegistryValueKind.String);
                Registry.SetValue(keyPath, "StoredPassword", password, RegistryValueKind.String);

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"حدث خطأ أثناء حفظ البيانات: {ex.Message}");
                return false;
            }
        }

        public static bool GetStoredCredentialFromRegistry(ref string Username, ref string Password)
        {

            try
            {
                // Read the value from the Registry
                 Username = Registry.GetValue(keyPath, "StoredUsername", null) as string;
                 Password = Registry.GetValue(keyPath, "StoredPassword", null) as string;
                if(Username==null)
                    Username = "";
                if (Password == null)
                    Password = "";
                return true;



            }
           
            catch (IOException ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }

        }

        public static string ComputeHash(string input)
        {
            //SHA is Secutred Hash Algorithm.
            // Create an instance of the SHA-256 algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash value from the UTF-8 encoded input string
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert the byte array to a lowercase hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


    }
}
public static class PasswordHelper
{
    public static string HashPassword(string password, out string salt)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            salt = Convert.ToBase64String(saltBytes);
        }
        string saltedPassword = salt + password;
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        string saltedPassword = storedSalt + enteredPassword;
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            string enteredHash = Convert.ToBase64String(hashBytes);
            return enteredHash == storedHash;
        }
    }
}

public static class CryptoHelper
{
    // Simple fixed key and IV. Ensure these are 16 bytes for AES-128.
    private static readonly string key = "1234567890123456";  // 16 bytes key for AES-128
    private static readonly string iv = "1234567890123456";   // 16 bytes IV

    // Encrypt method
    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
                swEncrypt.Flush();
                csEncrypt.FlushFinalBlock();
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    // Decrypt method
    public static string Decrypt(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);  // Convert Base64 to byte array

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);  // Ensure key is 16 bytes
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);    // Ensure IV is 16 bytes

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();  // Return the decrypted string
            }
        }
    }
}
 public static class UserPermissions

{
    public static bool canDelete(string v)
    {
        return true;
    }

    public static bool canEdit(string v)
    {
        return true;
    }

    public static bool canView(string v)
    {
        return true;
    }
}
public static class BindCombos
{
    private static Dictionary<string, DataTable> _caches = new Dictionary<string, DataTable>();

    public static void ClearCache(string key = "")
    {
        if (key != "")
        {
            _caches.Remove(key);
            var matchingKeys = _caches.Keys
                .Where(k => k.Contains(key))
                .ToList();

            if (matchingKeys.Count > 0)
            {
                foreach (var k in matchingKeys)
                {
                    _caches.Remove(k);
                }
            }
        }
        else
        {
            _caches.Clear();
        }
    }

    private static void BindComboBox<T>(T cmb, string query, string valueMember, string displayMember,
        bool isAddOption = false, string addOptionValue = "0", string addOptionText = "<< Add >>",
        bool forceRefresh = false, params MySqlParameter[] parameters)
        where T : ListControl
    {
        DataTable dt;

        // Build a cache key that includes parameter values
        //string cacheKey = query + string.Join(";", parameters.Select(p => $"{p.ParameterName}={p.Value}"));

        //if (!forceRefresh && _cache.ContainsKey(cacheKey))
        //{
        //    dt = _cache[cacheKey].Copy();
        //}
        //else
        //{
        dt = ClinicDatabase.ExecuteDataTable(query, parameters);
        //_cache[cacheKey] = dt.Copy();
        //}

        if (isAddOption)
        {
            DataRow newRow = dt.NewRow();
            newRow[valueMember] = addOptionValue;
            newRow[displayMember] = addOptionText;
            dt.Rows.InsertAt(newRow, 0);
        }

        cmb.ValueMember = valueMember;
        cmb.DisplayMember = displayMember;
        cmb.DataSource = dt;
    }

    private static void BindCheckedListBox(
        CheckedListBox lst, string query, string valueMember, string displayMember, bool isAddOption = false, string addOptionValue = "0", string addOptionText = "<< Add >>", bool forceRefresh = false)
    {
        if (lst == null)
            throw new ArgumentNullException(nameof(lst)); // make sure not null

        DataTable dt;

        //if (!forceRefresh && _cache.ContainsKey(query))
        //{
        //    dt = _cache[query].Copy();
        //}
        //else
        //{
        dt = ClinicDatabase.ExecuteDataTable(query);
        //    _cache[query] = dt.Copy();
        //}

        if (isAddOption)
        {
            DataRow newRow = dt.NewRow();
            newRow[valueMember] = addOptionValue;
            newRow[displayMember] = addOptionText;
            dt.Rows.InsertAt(newRow, 0);
        }

        // clear old items
        lst.Items.Clear();

        // add rows manually
        foreach (DataRow row in dt.Rows)
        {
            lst.Items.Add(
                new
                {
                    Text = row[displayMember].ToString(),
                    Value = row[valueMember].ToString()
                },
                false // not checked initially
            );
        }

        // make sure list shows text instead of object
        lst.DisplayMember = "Text";
        lst.ValueMember = "Value";
    }


    private static void BindDataGridViewComboBoxColumn(DataGridViewComboBoxColumn cmbColumn, string query, string valueMember, string displayMember,
        bool isAddOption = false, string addOptionValue = "0", string addOptionText = "<< Add >>",
        bool forceRefresh = false, params MySqlParameter[] parameters)
    {
        DataTable dt;

        //string cacheKey = query + string.Join(";", parameters.Select(p => $"{p.ParameterName}={p.Value}"));

        //if (!forceRefresh && _cache.ContainsKey(cacheKey))
        //{
        //    dt = _cache[cacheKey].Copy();
        //}
        //else
        //{
        dt = ClinicDatabase.ExecuteDataTable(query, parameters);
        //    _cache[cacheKey] = dt.Copy();
        //}

        if (isAddOption)
        {
            DataRow newRow = dt.NewRow();
            newRow[valueMember] = addOptionValue;
            newRow[displayMember] = addOptionText;
            dt.Rows.InsertAt(newRow, 0);
        }

        cmbColumn.ValueMember = valueMember;
        cmbColumn.DisplayMember = displayMember;
        cmbColumn.DataSource = dt;
    }

    public static Guna2ComboBox PopulateCustomers(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , NAME) as name , id FROM tbl_customer";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , NAME) as NAME ,id FROM tbl_customer");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateCustomers(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT id,code,name FROM tbl_customer";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }
    public static ComboBox PopulateSoledCustomers(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT id,code,name FROM tbl_customer where id IN (SELECT customer_id from tbl_sales)";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }
    public static Guna2ComboBox PopulateFixedAssetsCategories(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_fixed_assets_category";
        BindComboBox(cmb, query, "id", "category_name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_fixed_assets_category");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["category_name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "category_name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static CheckedListBox PopulateListCustomer(CheckedListBox lst, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , name) as NAME ,id FROM tbl_customer";
        BindCheckedListBox(lst, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return lst;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , name) as NAME ,id FROM tbl_customer");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //lst.DisplayMember = "Name";
        //lst.ValueMember = "id";
        //lst.DataSource = dt;
        //return lst;
    }

    public static CheckedListBox PopulateListProject(CheckedListBox lst, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_projects";
        BindCheckedListBox(lst, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return lst;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , name) as NAME ,id FROM tbl_customer");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //lst.DisplayMember = "Name";
        //lst.ValueMember = "id";
        //lst.DataSource = dt;
        //return lst;
    }

    public static void PopulateItems()
    {
        throw new NotImplementedException();
    }

    public static Guna2ComboBox PopulateRegisterBanks(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = 0";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = 0");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateRegisterBanks(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = 0";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select name, id from tbl_bank where state = 0");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateRegisterBanksAll(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select name, id from tbl_bank where state = 0");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateAccountNames(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select account_name, id from tbl_bank_card";
        BindComboBox(cmb, query, "id", "account_name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select account_name from tbl_bank_card");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["account_name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "account_name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateBanks(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        BindComboBox(cmb, "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank", "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateNonRegisterBanks(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        BindComboBox(cmb, "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = -1", "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = -1");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateNonRegisterBanks(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        BindComboBox(cmb, "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = -1", "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = -1");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateDepartments(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select id, name from tbl_departments";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_departments");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static ComboBox LoadMachincombox(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_fixed_assets where manufacture = 1";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_fixed_assets where manufacture = 1");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateItems(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select id,name from tbl_items where state = 0 and active = 0";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select id,name from tbl_items where state = 0 and active = 0");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulatePositions(Guna2ComboBox cmb, int departId)
    {
        string query = "select * from tbl_position where department_id=@id";
        BindComboBox(cmb, query, "id", "name", false, null, null, false,
            ClinicDatabase.CreateParameterNew("id", departId));
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_position where department_id=@id ",
        //    ClinicDatabase.CreateParameterNew("id", departId));
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static int SelectDefaultLevelAccount(string type)
    {
        using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"SELECT category, account_id FROM tbl_coa_config WHERE category =@type",
                ClinicDatabase.CreateParameterNew("type", type)))
            if (reader.Read())
                return int.Parse(reader["account_id"].ToString());
            else return 0;
    }

    public static Dictionary<string, int> DefaultAccounts()
    {
        frmLogin.defaultAccounts = LoadDefaultAccounts();
        return frmLogin.defaultAccounts;
    }
    public static Dictionary<string, int> LoadDefaultAccounts()
    {
        Dictionary<string, int> defaultAccounts = new Dictionary<string, int>();

        try
        {
            using (MySqlDataReader reader = ClinicDatabase.ExecuteReader(@"
            SELECT category, account_id FROM tbl_coa_config"))
            {
                while (reader.Read())
                {
                    string category = reader["category"].ToString();
                    int accountId = reader["account_id"] != DBNull.Value ? Convert.ToInt32(reader["account_id"]) : 0;
                    defaultAccounts[category] = accountId;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading default accounts: " + ex.Message);
        }

        return defaultAccounts;
    }

    public static Guna2ComboBox PopulateVendors(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false, string type = "")
    {
        string query = "SELECT CONCAT(CODE ,' - ' , NAME) as name , id FROM tbl_vendor";
        if (!string.IsNullOrEmpty(type))
            query += " WHERE type = '" + type + "'";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;

        //BindComboBox(cmb, "SELECT CONCAT(CODE ,' - ' , NAME) as NAME ,id FROM tbl_vendor", "id", "name", isAddOption);
        //return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , NAME) as NAME ,id FROM tbl_vendor");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static ComboBox PopulateVendors(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false, string type = "")
    {
        string query = "SELECT id, code, name FROM tbl_vendor";
        if (!string.IsNullOrEmpty(type))
            query += " WHERE type = '" + type + "'";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }
    public static ComboBox PopulatePurchasedVendors(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false, string type = "")
    {
        string query = "SELECT id, code, name FROM tbl_vendor where id IN (SELECT vendor_id from tbl_purchase) and type='Vendor'";
        if (!string.IsNullOrEmpty(type))
            query += " WHERE type = '" + type + "'";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }
    public class ListItemModel
    {
        public string Name { get; set; } // This will be shown in the list
        public int Id { get; set; }      // This is the hidden value
    }
    public static CheckedListBox PopulateListEmployeesNew(CheckedListBox lst, bool isAddOption = false)
    {
        DataTable dt = ClinicDatabase.ExecuteDataTable(
            "SELECT CONCAT(CODE ,' - ' , Name) AS Name, Id FROM tbl_employee"
        );

        //if (dt == null)
        //{
        //    MessageBox.Show("No data returned from tbl_employee.");
        //    return lst;
        //}
        //DataTable dt = new DataTable();
        //dt.Columns.Add("id", typeof(int));
        //dt.Columns.Add("name", typeof(string));

        //// Add sample rows
        //dt.Rows.Add(1, "E001 - John Doe");

        //lst.DataSource = dt;
        List<ListItemModel> items = new List<ListItemModel>();
        foreach (DataRow row in dt.Rows)
        {
            items.Add(new ListItemModel
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString()
            });
        }
        // Bind the list to the CheckedListBox
        foreach (var item in items)
        {
            lst.Items.Add(item);
        }
        lst.DisplayMember = "name";
        lst.ValueMember = "id";


        return lst;
    }
    public static CheckedListBox PopulateListEmployeesOld(CheckedListBox lst, bool isAddOption = false)
    {
        //string query = "SELECT CONCAT(CODE ,' - ' , Name) as name ,id FROM tbl_employee";
        //BindCheckedListBox(lst, query, "id", "name", isAddOption, "0", "<< Add >>", false);
        //return lst;
        DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , Name) as NAME ,id FROM tbl_employee");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        lst.DisplayMember = "name";
        lst.ValueMember = "id";
        if (dt != null && dt.Rows.Count > 0)
            lst.DataSource = dt;

        return lst;
    }
    public static Guna2ComboBox PopulateEmployees(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , Name) as NAME ,id FROM tbl_employee";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , Name) as NAME ,id FROM tbl_employee");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateEmployees(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , Name) as NAME ,id FROM tbl_employee";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , Name) as NAME ,id FROM tbl_employee");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateEmployeesForLoan(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT NAME ,id FROM tbl_employee";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateCountries(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CountryID as Id,CountryName as Name from tbl_country";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select id,name from tbl_country");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateCountries(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select id,name from tbl_country";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select id,name from tbl_country");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulatePrepaidExpenseCategories(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select id,name from tbl_prepaid_expense_category";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select id,name from tbl_prepaid_expense_category");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateCities(Guna2ComboBox cmb, int countryId)
    {
        string query = "select * from tbl_city where country_id=@id";
        BindComboBox(cmb, query, "id", "name", false, null, null, false,
            ClinicDatabase.CreateParameterNew("id", countryId));
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_city where country_id=@id",
        //    ClinicDatabase.CreateParameterNew("id", countryId));
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateAllLevel4Account(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_4 ORDER BY code";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_4 ORDER BY code");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static ComboBox LoadAllLevel4Account(Guna2ComboBox cmb)
    {
        if (BindDataTable.tableLevel4 != null && BindDataTable.tableLevel4.Rows.Count != 0)
        {
            cmb.DataSource = BindDataTable.tableLevel4;
            cmb.ValueMember = "id";
            cmb.DisplayMember = "name";
            return cmb;
        }
        else
        {
            string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_4 ORDER BY code";
            BindDataTable.tableLevel4 = ClinicDatabase.ExecuteDataTable(query);

            cmb.DataSource = BindDataTable.tableLevel4;
            cmb.ValueMember = "id";
            cmb.DisplayMember = "name";
            return cmb;
        }
    }

    public static ComboBox PopulateAllLevel4Account(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select id, code, name from tbl_coa_level_4 ORDER BY name";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }

    public static Guna2ComboBox Populatelevel3Account(Guna2ComboBox cmb, int level3Id, bool isAddOption = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_3 where main_id=@id ORDER BY code";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", false,
            ClinicDatabase.CreateParameterNew("id", level3Id));
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_3 where main_id=@id ORDER BY code",
        //    ClinicDatabase.CreateParameterNew("id", level3Id));
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox Populatelevel3AccountAll(Guna2ComboBox cmb, bool isAddOption = false)
    {
        int level3Id = 0;
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_3 ORDER BY code";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", false,
            ClinicDatabase.CreateParameterNew("id", level3Id));
        return cmb;
    }
    public static Guna2ComboBox Populatelevel2Account(Guna2ComboBox cmb, int level2Id, bool isAddOption = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_2 where main_id=@id ORDER BY code";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", false,
            ClinicDatabase.CreateParameterNew("id", level2Id));
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_2 where main_id=@id ORDER BY code",
        //    ClinicDatabase.CreateParameterNew("id", level2Id));
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox Populatelevel1Account(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_1";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_1");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateVendorCategory(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_vendor_category";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_vendor_category");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static Guna2ComboBox PopulateSubContractCategory(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_vendor_category";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_vendor_category");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static Guna2ComboBox PopulateCustomerCategory(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_customer_category";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_customer_category");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateWarehouse(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select concat(code,' - ', name) as name,id from tbl_warehouse";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select concat(code,' - ', name) as name,id from tbl_warehouse");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateItemsCategory(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select concat(code,' - ', name) as name,id from tbl_item_category";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select concat(code,' - ', name) as name,id from tbl_item_category");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateItemUnit(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_unit";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_unit");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateUserRoles(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_sec_roles";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_sec_roles");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateItemTax(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_tax";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_tax");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateCostCenter(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_cost_center";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_cost_center");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateCostCenter(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_cost_center";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }
    public static Guna2ComboBox PopulateEmployeesToPettyCard(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = @"
                SELECT 
                    CONCAT(code, ' - ', name) AS name, 
                    id 
                FROM tbl_employee 
                WHERE id NOT IN (SELECT CAST(name AS UNSIGNED) FROM tbl_petty_cash_card)";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //string query = @"SELECT CONCAT(code, ' - ', name) AS name, id FROM tbl_employee WHERE id NOT IN (SELECT CAST(name AS UNSIGNED) FROM tbl_petty_cash_card)";
        //DataTable dt = ClinicDatabase.ExecuteDataTable(query);
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;

        //return cmb;
    }
    public static Guna2ComboBox PopulateEmployeesToPettyCardAllEmp(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = @"
                SELECT 
                    CONCAT(code, ' - ', name) AS name, 
                    id 
                FROM tbl_employee";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //string query = @"SELECT CONCAT(code, ' - ', name) AS name, id FROM tbl_employee WHERE id NOT IN (SELECT CAST(name AS UNSIGNED) FROM tbl_petty_cash_card)";
        //DataTable dt = ClinicDatabase.ExecuteDataTable(query);
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;

        //return cmb;
    }

    public static Guna2ComboBox PopulatePettyCash(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = @"
                SELECT 
                    CONCAT(pcc.code, ' - ', emp.name) AS name, 
                    CAST(emp.id AS CHAR) AS value
                FROM tbl_petty_cash_card pcc 
                JOIN tbl_employee emp ON CAST(pcc.name AS UNSIGNED) = emp.id";
        BindComboBox(cmb, query, "value", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable(@"
        //        SELECT 
        //            CONCAT(pcc.code, ' - ', emp.name) AS name, 
        //            CAST(emp.id AS CHAR) AS value
        //        FROM tbl_petty_cash_card pcc 
        //        JOIN tbl_employee emp ON CAST(pcc.name AS UNSIGNED) = emp.id;
        //    ");

        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["value"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.DisplayMember = "name";
        //cmb.ValueMember = "value";
        //cmb.DataSource = dt;

        //return cmb;
    }

    public static ListView PopulateAllLevel4AccountList(ListView lst)
    {
        lst.Items.Clear();
        lst.View = View.Details;
        lst.FullRowSelect = true;
        lst.Columns.Clear();

        // Add columns
        lst.Columns.Add("Code - Name", 300);
        lst.Columns.Add("ID", 0);

        DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE, ' - ', NAME) AS name, id FROM tbl_coa_level_4 ORDER BY code");
        ListViewItem item = new ListViewItem("Add New");
        item.SubItems.Add("0");
        lst.Items.Add(item);
        foreach (DataRow row in dt.Rows)
        {
            ListViewItem itema = new ListViewItem(row["name"].ToString());
            itema.SubItems.Add(row["id"].ToString());
            lst.Items.Add(itema);
        }

        return lst;
    }
    public static Guna2ComboBox resturentcombox(Guna2ComboBox cmb, int departId)
    {
        string query = "select * from tbl_position where department_id=@id";
        BindComboBox(cmb, query, "id", "name", false, null, null, false,
            ClinicDatabase.CreateParameterNew("id", departId));
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_position where department_id=@id ",
        //    ClinicDatabase.CreateParameterNew("id", departId));
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox resturantdepartment(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_departments where name ='Restaurant'";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_departments where name ='Restaurant'");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static void PopulateDGVRegisterBanks(DataGridViewComboBoxColumn cmbColumn)
    {
        string query = "SELECT CONCAT(CODE , ' - ' , NAME) AS name, id FROM tbl_bank WHERE state = 0";
        BindDataGridViewComboBoxColumn(cmbColumn, query, "id", "name", false, null, null, false);
        //cmbColumn.ValueMember = "id";
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_bank where state = 0");
        //cmbColumn.ValueMember = "id";
        //cmbColumn.DisplayMember = "name";
        //cmbColumn.DataSource = dt;
    }

    public static void PopulateLevel4Account(DataGridViewComboBoxColumn cmbColumn, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CAST(code as CHAR) code,name FROM tbl_coa_level_4;";
        BindDataGridViewComboBoxColumn(cmbColumn, query, "code", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        //cmbColumn.DataSource = dt;
        //cmbColumn.DisplayMember = "name";
        //cmbColumn.ValueMember = "code";
    }

    public static Guna2ComboBox PopulatePettyCashCard(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = @"
                                            SELECT 
                    e.id, 
                    e.name, 
                    SUM(r.pay) AS total_amount
                FROM tbl_petty_cash_request r
                INNER JOIN tbl_employee e ON r.petty_cash_name = e.id
                WHERE r.state = 'Approved'
                GROUP BY e.id, e.name;
                                ";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable(query);
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.DisplayMember = "name";
        //cmb.ValueMember = "id";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateProjects(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT '' as name ,1 id";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_projects");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateTenderedProjects(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = @"
                SELECT 
                    CONCAT(p.CODE, ' - ', p.name) AS name, 
                    p.id 
                FROM tbl_projects p
                WHERE p.id IN (SELECT project_id FROM tbl_project_tender WHERE amount > 0)";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_projects where id IN (select project_id from tbl_project_tender WHERE amount>0)");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateTender(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT tender_name as name ,id FROM tbl_project_tender";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT tender_name as name ,id FROM tbl_project_tender");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateTenderNames(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_tender_names";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_tender_names");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox populateSites(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_project_sites";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_project_sites");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox PopulateCityAllNormalComboBox(ComboBox cmb)
    {
        string query = "select * from tbl_city";
        BindComboBox(cmb, query, "id", "name", false, null, null, false);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_city");
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static Guna2ComboBox monthcheckcmb(Guna2ComboBox cmb, int id1)
    {
        string query = "SELECT MONTHNAME(WorkDate) AS MonthName FROM tbl_attendancesheet WHERE code =@id GROUP BY MONTHNAME(WorkDate)";
        BindComboBox(cmb, query, "MonthName", "MonthName", false, null, null, false,
            ClinicDatabase.CreateParameterNew("id", id1));
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT MONTHNAME(WorkDate) AS MonthName FROM tbl_attendancesheet WHERE code =@id GROUP BY MONTHNAME(WorkDate) ",
        //    ClinicDatabase.CreateParameterNew("id", id1));
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "MonthName";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateProjectResourceRole(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_project_role";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_project_role");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static Guna2ComboBox PopulateProjectResourceLabor(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT CONCAT(CODE ,' - ' , name) as name ,id FROM tbl_project_resource WHERE TYPE='Labour'";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }

    public static ComboBox LoadDepartManufacture(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_departments";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_departments");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static ComboBox DepartPositionEmployee(ComboBox cmb, int departId, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_position where department_id=@id";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh,
            ClinicDatabase.CreateParameterNew("id", departId));
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_position where department_id=@id ",
        //ClinicDatabase.CreateParameterNew("id", departId));
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }
    public static ComboBox LoadProcessStageManufacture(ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "select * from tbl_Manufacturing_Steps";
        BindComboBox(cmb, query, "id", "step_name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
        //DataTable dt = ClinicDatabase.ExecuteDataTable("select * from tbl_Manufacturing_Steps");
        //if (isAddOption)
        //{
        //    // Add the "<< Add >>" row at the top
        //    DataRow newRow = dt.NewRow();
        //    newRow["id"] = 0;
        //    newRow["name"] = "<< Add >>";
        //    dt.Rows.InsertAt(newRow, 0);
        //}
        //cmb.ValueMember = "id";
        //cmb.DisplayMember = "name";
        //cmb.DataSource = dt;
        //return cmb;
    }

    public static Guna2ComboBox PopulateUsers(Guna2ComboBox cmb, bool isAddOption = false, bool forceRefresh = false)
    {
        string query = "SELECT user_name as name,id FROM tbl_sec_users";
        BindComboBox(cmb, query, "id", "name", isAddOption, "0", "<< Add >>", forceRefresh);
        return cmb;
    }

    public class Level4AccountComboItem
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
public static class BindDataTable
{
    public static DataTable tableCountries, tableCities, tableGeneralSettings, tableModules;
    public static DataTable tableLevel4, tableCoaConfig;
    public static Dictionary<string, int> coaConfigDict;

    public static void FetchData()
    {
        LoadTable(ref tableGeneralSettings, "SELECT id, NAME, VALUE, description,status FROM tbl_general_settings");
        LoadTable(ref tableCountries, "SELECT * FROM tbl_country");
        LoadTable(ref tableCities, "SELECT * FROM tbl_city");
        LoadTable(ref tableLevel4, "select CONCAT(CODE , ' - ' , NAME) AS name, id from tbl_coa_level_4 ORDER BY code");
        LoadTable(ref tableModules, "select id, name, status from tbl_set_main_menu id");
    }

    //public static DataTable tableBank, tableBankCard, tableBankCheque, tableCompany, tableCostCenter, tableCustomers, tableCustomerCategory, tableDepartments, tableEmployee, tableFixedAssetsCategory, tableItemCategory, tableProducts, tableProductUnits, tableProductWarehouses, tablePettyCashCategory, tablePosition, tablePrepaidExpenseCategory,
    //    tableSecRoles, tableSecUsers, tableSubCostCenter, tableProductsUnitList, tableVendor, tableVendorCategory, tableAccount1, tableAccount2, tableAccount3, tableAccount4, tableWarehouses;

    //public static void LoadAllData()
    //{
    //    LoadTable(ref tableBank, "SELECT * FROM tbl_bank");
    //    LoadTable(ref tableBankCard, "SELECT * FROM tbl_bank_card");
    //    LoadTable(ref tableBankCard, "SELECT * FROM tbl_bank_register");
    //    LoadTable(ref tableBankCheque, "SELECT * FROM tbl_cheque");
    //    LoadTable(ref tableCompany, "SELECT * FROM tbl_company");
    //    LoadTable(ref tableCostCenter, "SELECT * FROM tbl_cost_center");
    //    LoadTable(ref tableAccount1, "SELECT * FROM tbl_coa_level_1");
    //    LoadTable(ref tableAccount2, "SELECT * FROM tbl_coa_level_2");
    //    LoadTable(ref tableAccount3, "SELECT * FROM tbl_coa_level_3");
    //    LoadTable(ref tableAccount4, "SELECT * FROM tbl_coa_level_4");
    //    LoadTable(ref tableCustomers, "SELECT * FROM tbl_customer");
    //    LoadTable(ref tableCustomerCategory, "SELECT * FROM tbl_customer_category");
    //    LoadTable(ref tableDepartments, "SELECT * FROM tbl_departments");
    //    LoadTable(ref tableEmployee, "SELECT * FROM tbl_employee");
    //    LoadTable(ref tableFixedAssetsCategory, "SELECT * FROM tbl_fixed_assets_category");
    //    LoadTable(ref tableItemCategory, "SELECT * FROM tbl_item_category");
    //    LoadTable(ref tableProducts, "SELECT * FROM tbl_items");
    //    LoadTable(ref tableProductsUnitList, "SELECT * FROM tbl_items_unit");
    //    LoadTable(ref tableProductWarehouses, "SELECT * FROM tbl_items_warehouse");
    //    LoadTable(ref tablePettyCashCategory, "SELECT * FROM tbl_petty_cash_category");
    //    LoadTable(ref tablePosition, "SELECT * FROM tbl_position");
    //    LoadTable(ref tablePrepaidExpenseCategory, "SELECT * FROM tbl_prepaid_expense_category");
    //    LoadTable(ref tableSecRoles, "SELECT * FROM tbl_sec_roles");
    //    LoadTable(ref tableSecUsers, "SELECT * FROM tbl_sec_users");
    //    LoadTable(ref tableSubCostCenter, "SELECT * FROM tbl_sub_cost_center");
    //    LoadTable(ref tableProductUnits, "SELECT * FROM tbl_unit");
    //    LoadTable(ref tableVendor, "SELECT * FROM tbl_vendor");
    //    LoadTable(ref tableVendorCategory, "SELECT * FROM tbl_vendor_category");
    //    LoadTable(ref tableWarehouses, "SELECT * FROM tbl_warehouse");
    //}

    //public static void ReloadItems()
    //{
    //    LoadTable(ref tableProducts, "SELECT * FROM tbl_items");
    //}


    // Generic loader
    private static void LoadTable(ref DataTable table, string query)
    {
        table = ClinicDatabase.ExecuteDataTable(query);
    }

    public static void LoadConfigData()
    {
        string configQuery = "SELECT category, account_id FROM tbl_coa_config";
        BindDataTable.tableCoaConfig = ClinicDatabase.ExecuteDataTable(configQuery);
    }

    //private static DataTable GetCustomers()
    //{
    //    var cached = CacheManager<DataTable>.Get("customers");
    //    if (cached != null) return cached;

    //    string query = "SELECT id, code, name FROM tbl_customer";
    //    DataTable dt = ClinicDatabase.ExecuteDataTable(query);

    //    CacheManager<DataTable>.Set("customers", dt, TimeSpan.FromMinutes(10));
    //    return dt;
    //}
}
public  class ItemSuggestion
{
    public int Id { get; set; }
    public  string Code { get; set; }
    public  string Name { get; set; }
    public override string ToString()
    {
        return Code + " - " + Name;
    }
}
public static class AttachmentManager
{
    public static bool HasAttachment(int purchaseId, string v)
    {
        return false;
    }

    public static void OpenAttachmentByType(int purchaseId, string v)
    {
        string a = "";
    }

    public static void SaveAttachment(string v, int fId, string fileName)
    {
        string a = "";
    }
}
public static class AmountToWordsHelper
{
    #region PUBLIC METHODS

    public static string ConvertToEnglish(decimal amount, string mainCurrency, string subCurrency)
    {
        long integerPart = (long)Math.Floor(amount);
        int fractionPart = (int)Math.Round((amount - integerPart) * 100, 0);

        string words = "";

        if (integerPart == 0)
            words = "Zero";
        else
            words = NumberToEnglish(integerPart);

        words += " " + (integerPart == 1 ? mainCurrency : mainCurrency + "s");

        if (fractionPart > 0)
        {
            words += " and " + NumberToEnglish(fractionPart) + " " + (fractionPart == 1 ? subCurrency : subCurrency);
        }

        words += " Only";
        return words;
    }

    public static string ConvertToArabic(decimal amount, string mainCurrency, string subCurrency)
    {
        long integerPart = (long)Math.Floor(amount);
        int fractionPart = (int)Math.Round((amount - integerPart) * 100, 0);

        string words = "";

        if (integerPart == 0)
            words = "صفر";
        else
            words = NumberToArabic(integerPart);

        words += " " + GetArabicCurrencyName(integerPart, mainCurrency);

        if (fractionPart > 0)
        {
            words += " و" + NumberToArabic(fractionPart) + " " + GetArabicCurrencyName(fractionPart, subCurrency);
        }

        words += " فقط لا غير";
        return words;
    }

    #endregion

    #region ENGLISH

    private static string NumberToEnglish(long number)
    {
        if (number == 0)
            return "Zero";

        if (number < 0)
            return "Minus " + NumberToEnglish(Math.Abs(number));

        string words = "";

        if ((number / 1000000000) > 0)
        {
            words += NumberToEnglish(number / 1000000000) + " Billion ";
            number %= 1000000000;
        }

        if ((number / 1000000) > 0)
        {
            words += NumberToEnglish(number / 1000000) + " Million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += NumberToEnglish(number / 1000) + " Thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += NumberToEnglish(number / 100) + " Hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "and ";

            string[] unitsMap = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                                  "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
                                  "Eighteen", "Nineteen" };

            string[] tensMap = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
        }

        return words.Trim();
    }

    #endregion

    #region ARABIC

    private static string NumberToArabic(long number)
    {
        if (number == 0)
            return "صفر";

        if (number < 0)
            return "سالب " + NumberToArabic(Math.Abs(number));

        return ConvertArabic(number).Trim();
    }

    private static string ConvertArabic(long number)
    {
        string[] ones =
        {
            "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة",
            "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر",
            "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
        };

        string[] tens =
        {
            "", "", "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون"
        };

        string[] hundreds =
        {
            "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة", "خمسمائة",
            "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
        };

        if (number < 20)
            return ones[number];

        if (number < 100)
        {
            if (number % 10 == 0)
                return tens[number / 10];
            return ones[number % 10] + " و" + tens[number / 10];
        }

        if (number < 1000)
        {
            if (number % 100 == 0)
                return hundreds[number / 100];
            return hundreds[number / 100] + " و" + ConvertArabic(number % 100);
        }

        if (number < 1000000)
        {
            long thousands = number / 1000;
            long remainder = number % 1000;

            string thousandsText = "";

            if (thousands == 1)
                thousandsText = "ألف";
            else if (thousands == 2)
                thousandsText = "ألفان";
            else if (thousands >= 3 && thousands <= 10)
                thousandsText = ConvertArabic(thousands) + " آلاف";
            else
                thousandsText = ConvertArabic(thousands) + " ألف";

            if (remainder == 0)
                return thousandsText;

            return thousandsText + " و" + ConvertArabic(remainder);
        }

        if (number < 1000000000)
        {
            long millions = number / 1000000;
            long remainder = number % 1000000;

            string millionsText = "";

            if (millions == 1)
                millionsText = "مليون";
            else if (millions == 2)
                millionsText = "مليونان";
            else if (millions >= 3 && millions <= 10)
                millionsText = ConvertArabic(millions) + " ملايين";
            else
                millionsText = ConvertArabic(millions) + " مليون";

            if (remainder == 0)
                return millionsText;

            return millionsText + " و" + ConvertArabic(remainder);
        }

        long billions = number / 1000000000;
        long billionsRemainder = number % 1000000000;

        string billionsText = "";

        if (billions == 1)
            billionsText = "مليار";
        else if (billions == 2)
            billionsText = "ملياران";
        else if (billions >= 3 && billions <= 10)
            billionsText = ConvertArabic(billions) + " مليارات";
        else
            billionsText = ConvertArabic(billions) + " مليار";

        if (billionsRemainder == 0)
            return billionsText;

        return billionsText + " و" + ConvertArabic(billionsRemainder);
    }

    private static string GetArabicCurrencyName(long number, string baseCurrency)
    {
        // Simple practical version for ERP usage
        // You can customize per currency if you want exact grammar.
        if (baseCurrency == "درهم")
        {
            if (number == 1) return "درهم";
            if (number == 2) return "درهمان";
            if (number >= 3 && number <= 10) return "دراهم";
            return "درهماً";
        }

        if (baseCurrency == "فلس")
        {
            if (number == 1) return "فلس";
            if (number == 2) return "فلسان";
            if (number >= 3 && number <= 10) return "فلوس";
            return "فلساً";
        }

        return baseCurrency;
    }

    public static string ConvertToSelectedLanguage(decimal value) //,bool isCurrency, string baseCurrency)
    {
        string savedCulture = Clinic.Properties.Settings.Default.ApplicationLanguage ?? "en";

        switch (savedCulture)
        {
            case "en":
                return ConvertToEnglish(value, "Dirham", "Fils");
            case "ar":
                return ConvertToArabic(value, "درهم", "فلس");
            case "fr":
                return ConvertToEnglish(value, "Euro", "Centime");
            case "hi":
                return ConvertToEnglish(value, "Rupees", "Paisa");
            case "ur":
                return ConvertToEnglish(value, "Rupees", "Paisa");
            case "es":
                return ConvertToEnglish(value, "Euro", "Céntimo");
            case "gr":
                return ConvertToEnglish(value, "Euro", "Cent");
            case "ja":
                return ConvertToEnglish(value, "Yen", "Sen");
            case "ko":
                return ConvertToEnglish(value, "Won", "Jeon");
            case "fil":
                return ConvertToEnglish(value, "Piso", "Sentimo");
            case "zh-CN":
                return ConvertToEnglish(value, "Yuan", "Fen");
            case "zh-TW":
                return ConvertToEnglish(value, "Yuan", "Fen");
            default:
                return ConvertToEnglish(value, "", "");

        }
    }

    #endregion
}
public static class EventHub
{

    public static event EventHandler lvl1Account;
    public static event EventHandler lvl2Account;
    public static event EventHandler lvl3Account;
    public static event EventHandler lvl4Account;
    public static event EventHandler Journal;
    public static event EventHandler User;
    public static event EventHandler Roles;
    public static event EventHandler Customer;
    public static event EventHandler CustomerCategory;
    public static event EventHandler ReceiptVoucher;
    public static event EventHandler PaymentVoucher;
    public static event EventHandler PettyCashVoucher;
    public static event EventHandler SalesInv;
    public static event EventHandler JobCard;
    public static event EventHandler SalesOrder;
    public static event EventHandler SalesQuotation;
    public static event EventHandler SalesProforma;
    public static event EventHandler DeliveryNote;
    public static event EventHandler SalesReturn;
    public static event EventHandler Vendor;
    public static event EventHandler SubContract;
    public static event EventHandler VendorCategory;
    public static event EventHandler SubContractCategory;
    public static event EventHandler PurchaseInv;
    public static event EventHandler PurchaseOrder;
    public static event EventHandler PurchaseOffer;
    public static event EventHandler MaterialRequest;
    public static event EventHandler MaterialReceived;
    public static event EventHandler PurchaseReturn;
    public static event EventHandler DamageInv;
    public static event EventHandler Item;
    public static event EventHandler ItemCategory;
    public static event EventHandler FixedAssetsCategory;
    public static event EventHandler ItemUnit;
    public static event EventHandler wareHouse;
    public static event EventHandler Employee;
    public static event EventHandler EmployeeDept;
    public static event EventHandler EmployeePosition;
    public static event EventHandler Bank;
    public static event EventHandler PDCPayable;
    public static event EventHandler PDCReceivable;
    public static event EventHandler Check;
    public static event EventHandler BankCard;
    public static event EventHandler PrepaidExpense;
    public static event EventHandler FixedAsset;
    public static event EventHandler CreditNote;
    public static event EventHandler DebitNote;
    public static event EventHandler MainCostCenter;
    public static event EventHandler TaxSchedule;
    public static event EventHandler Project;
    public static event EventHandler ProjectPlanning;
    public static event EventHandler ProjectTendering;
    public static event EventHandler ProjectViewSites;
    public static event EventHandler ProjectViewResource;
    public static event EventHandler ProjectInvoice;
    public static event EventHandler ProjectClassification;
    public static event EventHandler ProjectPhase;
    public static event EventHandler MaterialReservation;
    public static event EventHandler MaterialProcurement;
    public static event EventHandler MaterialIssue;
    public static event EventHandler MaterialReceipt;
    public static event EventHandler MaterialReturn;
    public static event EventHandler MaterialTransfer;
    public static event EventHandler MaterialConsumption;
    public static event EventHandler StockReconciliation;

    private static void RaiseEvent(EventHandler handler)
    {
        handler?.Invoke(null, EventArgs.Empty);
    }
    public static void Refreshlvl1Account() => RaiseEvent(lvl1Account);
    public static void Refreshlvl2Account() => RaiseEvent(lvl2Account);
    public static void Refreshlvl3Account() => RaiseEvent(lvl3Account);
    public static void Refreshlvl4Account() => RaiseEvent(lvl4Account);

    public static void RefreshJournal() => RaiseEvent(Journal);
    public static void RefreshUser() => RaiseEvent(User);
    public static void RefreshRoles() => RaiseEvent(Roles);
    public static void RefreshCustomer() => RaiseEvent(Customer);
    public static void RefreshCustomerCategory() => RaiseEvent(CustomerCategory);
    public static void RefreshJobCard() => RaiseEvent(JobCard);
    public static void RefreshSales() => RaiseEvent(SalesInv);
    public static void RefreshSalesOrder() => RaiseEvent(SalesOrder);
    public static void RefreshQuotation() => RaiseEvent(SalesQuotation);
    public static void RefreshSalesProforma() => RaiseEvent(SalesProforma);
    public static void RefreshDeliveryNote() => RaiseEvent(DeliveryNote);
    public static void RefreshSalesReturn() => RaiseEvent(SalesReturn);
    public static void RefreshVendor() => RaiseEvent(Vendor);
    public static void RefreshSubContract() => RaiseEvent(SubContract);
    public static void RefreshVendorCategory() => RaiseEvent(VendorCategory);
    public static void RefreshSubContractCategory() => RaiseEvent(SubContractCategory);
    public static void RefreshPurchase() => RaiseEvent(PurchaseInv);
    public static void RefreshPurchaseOrder() => RaiseEvent(PurchaseOrder);
    public static void RefreshPurchaseOffer() => RaiseEvent(PurchaseOffer);
    public static void RefreshMaterialRequest() => RaiseEvent(MaterialRequest);
    public static void RefreshMaterialReceived() => RaiseEvent(MaterialReceived);
    public static void RefreshPurchaseReturn() => RaiseEvent(PurchaseReturn);
    public static void RefreshDamage() => RaiseEvent(DamageInv);
    public static void RefreshItem() => RaiseEvent(Item);
    public static void RefreshWarehouse() => RaiseEvent(wareHouse);
    public static void RefreshItemUnit() => RaiseEvent(ItemUnit);
    public static void RefreshItemCategory() => RaiseEvent(ItemCategory);
    public static void RefreshFixedAsset() => RaiseEvent(FixedAsset);
    public static void RefreshFixedAssetsCategory() => RaiseEvent(FixedAssetsCategory);
    public static void RefreshReceiptVoucher() => RaiseEvent(ReceiptVoucher);
    public static void RefreshPaymentVoucher() => RaiseEvent(PaymentVoucher);
    public static void RefreshPettyCashVoucher() => RaiseEvent(PettyCashVoucher);
    public static void RefreshEmployee() => RaiseEvent(Employee);
    public static void RefreshEmployeeDept() => RaiseEvent(EmployeeDept);
    public static void RefreshEmployeePosition() => RaiseEvent(EmployeePosition);
    public static void RefreshBank() => RaiseEvent(Bank);
    public static void RefreshCheck() => RaiseEvent(Check);
    public static void RefreshPDCPayable() => RaiseEvent(PDCPayable);
    public static void RefreshPDCReceivable() => RaiseEvent(PDCReceivable);
    public static void RefreshBankCard() => RaiseEvent(BankCard);
    public static void RefreshPrepaidExpense() => RaiseEvent(PrepaidExpense);
    public static void RefreshCreditNote() => RaiseEvent(CreditNote);
    public static void RefreshDebitNote() => RaiseEvent(DebitNote);
    public static void RefreshMainCostCenter() => RaiseEvent(MainCostCenter);
    public static void RefreshTaxSchedule() => RaiseEvent(TaxSchedule);
    public static void RefreshProject() => RaiseEvent(Project);
    public static void RefreshProjectPlanning() => RaiseEvent(ProjectPlanning);
    public static void RefreshProjectTendering() => RaiseEvent(ProjectTendering);
    public static void RefreshProjectViewSites() => RaiseEvent(ProjectViewSites);
    public static void RefreshProjectViewResource() => RaiseEvent(ProjectViewResource);
    public static void RefreshProjectInvoice() => RaiseEvent(ProjectInvoice);
    public static void RefreshProjectClassification() => RaiseEvent(ProjectClassification);
    public static void RefreshPhase() => RaiseEvent(ProjectPhase);
    public static void RefreshMaterialReservation() => RaiseEvent(MaterialReservation);
    public static void RefreshMaterialProcurement() => RaiseEvent(MaterialProcurement);
    public static void RefreshMaterialIssue() => RaiseEvent(MaterialIssue);
    public static void RefreshMaterialReceipt() => RaiseEvent(MaterialReceipt);
    public static void RefreshMaterialReturn() => RaiseEvent(MaterialReturn);
    public static void RefreshMaterialTransfer() => RaiseEvent(MaterialTransfer);
    public static void RefreshMaterialConsumption() => RaiseEvent(MaterialConsumption);
    public static void RefreshStockReconciliation() => RaiseEvent(StockReconciliation);
}