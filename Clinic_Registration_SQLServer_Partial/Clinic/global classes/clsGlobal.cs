using Clinic_Business;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    internal static  class clsGlobal
    {
        public enum UserRole { Admin = 1, Doctor = 2, Receptionist = 3, Nurse = 4, Cashier = 1004, Pharmacist= 1005 }

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
