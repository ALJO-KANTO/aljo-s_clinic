using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsPatientData
    {
        public static bool GetPatientByPatientID(
     int PatientID,
     ref int PersonID,
     ref string EmergencyContact,
     ref int BloodTypeID,
     ref string MedicalHistory,
     ref DateTime CreatedDate,
     ref int CreatedByUserID)
        {
            try
            {
                using (MySqlConnection conn =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                SELECT
                    p.PersonID,
                    p.EmergencyContact,
                    p.BloodTypeID,
                    p.MedicalHistory,
                    p.CreatedDate,
                    p.CreatedByUserID
                FROM patients p
                INNER JOIN persons per
                    ON p.PersonID = per.PersonID
                WHERE p.PatientID = @PatientID
                LIMIT 1;";

                    using (MySqlCommand cmd =
                        new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", PatientID);

                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return false;

                            PersonID = Convert.ToInt32(reader["PersonID"]);

                            EmergencyContact =
                                reader["EmergencyContact"] == DBNull.Value
                                ? ""
                                : reader["EmergencyContact"].ToString();

                            BloodTypeID =
                                reader["BloodTypeID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["BloodTypeID"]);

                            MedicalHistory =
                                reader["MedicalHistory"] == DBNull.Value
                                ? ""
                                : reader["MedicalHistory"].ToString();

                            CreatedDate =
                                Convert.ToDateTime(reader["CreatedDate"]);

                            CreatedByUserID =
                                reader["CreatedByUserID"] == DBNull.Value
                                ? -1
                                : Convert.ToInt32(reader["CreatedByUserID"]);

                            return true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error);

                System.Diagnostics.Debug.WriteLine(
                    "GetPatientByPatientID Error: " + ex.Message);

                return false;
            }
        }

        public static int AddNewPatient(int PersonID, string EmergencyContact, int BloodTypeID, string MedicalHistory, DateTime CreatedDate, int CreatedByUserID)
        {


            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO Patients (PersonID, EmergencyContact, BloodTypeID, MedicalHistory,CreatedDate, CreatedByUserID)
                             VALUES (@PersonID, @EmergencyContact, @BloodTypeID, @MedicalHistory,@CreatedDate, @CreatedByUserID);
                             SELECT LAST_INSERT_ID();";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PersonID", PersonID);
                        cmd.Parameters.AddWithValue("@EmergencyContact", EmergencyContact);
                        cmd.Parameters.AddWithValue("@BloodTypeID", BloodTypeID);
                        cmd.Parameters.AddWithValue("@MedicalHistory", MedicalHistory.ToDBValue());
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        // تحويل وإرجاع مباشر بدون حجز متغير مؤقت
                        if (result != null && int.TryParse(result.ToString(), out int PatientID))
                            return PatientID;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, CreatedByUserID);
            }

            return -1;
        }

        public static bool UpdatePatient(int PatientID, int PersonID, string EmergencyContact, int BloodTypeID, string MedicalHistory, int CreatedByUserID)
        {

            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Patients SET PersonID = @PersonID, EmergencyContact = @EmergencyContact,
                             BloodTypeID = @BloodTypeID, MedicalHistory = @MedicalHistory, CreatedByUserID = @CreatedByUserID 
                             WHERE PatientID = @PatientID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", PatientID);
                        cmd.Parameters.AddWithValue("@PersonID", PersonID);
                        cmd.Parameters.AddWithValue("@EmergencyContact", EmergencyContact);
                        cmd.Parameters.AddWithValue("@BloodTypeID", BloodTypeID);
                        cmd.Parameters.AddWithValue("@MedicalHistory", MedicalHistory.ToDBValue());
                        cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, CreatedByUserID);
                return false;
            }
        }

        public static bool DeletePatient(int PatientID)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                UPDATE patients
                SET IsActive = 0
                WHERE PatientID = @PatientID
                  AND IsActive = 1;";

                    using (MySqlCommand cmd =
                        new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", PatientID);

                        connection.Open();

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "DeletePatient Error: " + ex.Message);

                return false;
            }
        }
        public static DataTable GetAllPatients()
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
    SELECT
        p.PatientID,
        p.PersonID,
        per.FullName,
        p.EmergencyContact,
        p.MedicalHistory,
        p.CreatedDate
    FROM patients p
    LEFT JOIN persons per
        ON p.PersonID = per.PersonID
    WHERE p.IsActive = 1
    ORDER BY p.PersonID DESC;";

                    using (MySqlCommand cmd =
                        new MySqlCommand(query, connection))
                    {
                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error);

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return dt;
        }

        public static DataRow GetPatientByID(int PatientID)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                SELECT
                    p.PatientID,
                    p.PersonID,
                    per.FullName,
                    p.EmergencyContact,
                    p.MedicalHistory,
                    p.CreatedDate
                FROM patients p
                LEFT JOIN persons per
                    ON p.PersonID = per.PersonID
                WHERE p.PatientID = @PatientID;";

                    using (MySqlCommand cmd =
                        new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", PatientID);

                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error);

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static bool IsPatientExistForPersonID(int PersonID)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
    SELECT 1
    FROM patients
    WHERE PersonID = @PersonID
    LIMIT 1;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        connection.Open();

                        return (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }
        }
    }
}