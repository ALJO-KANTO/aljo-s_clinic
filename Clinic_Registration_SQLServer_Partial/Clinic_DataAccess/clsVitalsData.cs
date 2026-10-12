using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsVitalsData
    {
        public static int AddNewVitals(int VisitID, int AppointmentID, string BloodPressure, decimal Temperature, decimal Weight, short Pulse, int CreatedByUserID)
        {
            int VitalID = -1;

            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                // نبدأ الـ Transaction هنا
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"INSERT INTO Vitals (VisitID, AppointmentID, BloodPressure, Temperature, Weight, Pulse, RecordedDate, CreatedByUserID)
                                 VALUES (@VisitID, @AppointmentID, @BloodPressure, @Temperature, @Weight, @Pulse, GETDATE(), @CreatedByUserID);
                                 SELECT LAST_INSERT_ID();";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@VisitID", VisitID);
                            command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                            command.Parameters.AddWithValue("@BloodPressure", BloodPressure);
                            command.Parameters.AddWithValue("@Temperature", Temperature);
                            command.Parameters.AddWithValue("@Weight", Weight);
                            command.Parameters.AddWithValue("@Pulse", Pulse);
                            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                            object result = command.ExecuteScalar();

                            // استدعاء تحديث حالة الموعد باستخدام نفس الـ transaction
                            clsAppointmentData.UpdateAppointmentStatus(AppointmentID, 9, CreatedByUserID, transaction);

                            if (result != null && int.TryParse(result.ToString(), out int newVitalID))
                            {
                                VitalID = newVitalID;
                            }
                        }

                        // تنفيذ الحفظ (Commit) هنا داخل الـ using
                        transaction.Commit();
                    }
                    catch (MySqlException ex)
                    {
                        // التراجع (Rollback) هنا داخل الـ using
                        transaction.Rollback();
                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                        VitalID = -1;
                    }
                } // هنا ينتهي الـ using للـ transaction ويتم إغلاقه بأمان
            }

            return VitalID;
        }

        public static bool GetVitalsByAppointmentID(int AppointmentID, ref int VitalID, ref int VisitID, ref string BloodPressure,
                                                    ref decimal Temperature, ref decimal Weight, ref short Pulse, ref DateTime RecordedDate,ref int CreatedByUserID)
        {
            bool IsFound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = @"SELECT * FROM Vitals WHERE AppointmentID = @AppointmentID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                VitalID = (int)reader["VitalID"];
                                VisitID = (int)reader["VisitID"];
                                BloodPressure = reader["BloodPressure"].ToString();
                                Temperature = (decimal)reader["Temperature"];
                                Weight = (decimal)reader["Weight"];
                                Pulse = (short)reader["Pulse"];
                                RecordedDate = (DateTime)reader["RecordedDate"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                IsFound = true;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                IsFound = false;
            }
            return IsFound;
        }

        public static bool GetVitalsByVisitID(int VisitID, ref int VitalID, ref int AppointmentID, ref string BloodPressure,
                                                   ref decimal Temperature, ref decimal Weight, ref short Pulse, ref DateTime RecordedDate, ref int CreatedByUserID)
        {
            bool IsFound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM Vitals WHERE VisitID = @VisitID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VisitID", VisitID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                VitalID = (int)reader["VitalID"];
                                AppointmentID =(int) reader["AppointmentID"];
                                BloodPressure = reader["BloodPressure"].ToString();
                                Temperature = (decimal)reader["Temperature"];
                                Weight = (decimal)reader["Weight"];
                                Pulse = (short)reader["Pulse"];
                                RecordedDate = (DateTime)reader["RecordedDate"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                IsFound = true;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                IsFound = false;
            }
            return IsFound;
        }

        public static bool UpdateVitals(int VitalID, string BloodPressure, decimal Temperature, decimal Weight, short Pulse,int CreatedByUserID)
        {
            bool IsUpdated = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // التحديث هنا يتم بناءً على الـ VitalID الذي حصلنا عليه عند الحفظ الأول
                    string query = @"UPDATE Vitals 
                                     SET BloodPressure = @BloodPressure, 
                                         Temperature = @Temperature, 
                                         Weight = @Weight, 
                                         Pulse = @Pulse ,
                                         CreatedByUserID=@CreatedByUserID
                                     WHERE VitalID = @VitalID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VitalID", VitalID);
                        command.Parameters.AddWithValue("@BloodPressure", BloodPressure);
                        command.Parameters.AddWithValue("@Temperature", Temperature);
                        command.Parameters.AddWithValue("@Weight", Weight);
                        command.Parameters.AddWithValue("@Pulse", Pulse);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        IsUpdated = (rowsAffected > 0);
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                IsUpdated = false;
            }
            return IsUpdated;
        }

        public static DataTable GetPatientsWaitingForVitals()
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM View_PatientsWaitingForVitals 
                             WHERE StatusText in('Waiting_For_Vitals','In-Queue')
                            ORDER BY CheckInTime ASC";

                                                                
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }
            return dt;
        }

    }
}