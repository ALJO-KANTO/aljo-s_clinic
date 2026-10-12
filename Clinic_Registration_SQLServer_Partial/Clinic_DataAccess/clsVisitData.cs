using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsVisitData
    {
        public static int AddNewVisitData(int VisitID, int AppointmentID, string Diagnosis, string VisitNotes, int CreatedByUserID,int DoctorID, DataTable services, DataTable dtMedicines, string PrescriptionNotes, DateTime PrescriptionDate)
        {
            int PrescriptionID = 0;
            using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();
                using (MySqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        UpdateVisitDetails(VisitID, AppointmentID, Diagnosis, VisitNotes, CreatedByUserID, DoctorID, tran);

                        // 2. حفظ الخدمات الجديدة
                        if (services != null && services.Rows.Count > 0)
                            clsVisitServicesData.SaveVisitServices(VisitID, services, tran);

                        // 3. حفظ الوصفة الجديدة
                        if (dtMedicines != null && dtMedicines.Rows.Count > 0)
                          PrescriptionID=  clsPrescriptionData.SavePrescription(VisitID, PrescriptionNotes, PrescriptionDate, dtMedicines, 1, tran);

                        tran.Commit();
                        return PrescriptionID;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        clsGlobalLogger.LogMySqlException(ex as MySqlException, clsGlobalLogger.LogLevel.Error);
                        return -1;
                    }
                }
            }
        }

        public static bool UpdateExistingVisitData(int VisitID, int AppointmentID, string Diagnosis, string VisitNotes, int CreatedByUserID, int DoctorID, DataTable services, DataTable dtMedicines,ref int PrescriptionID, string PrescriptionNotes, DateTime PrescriptionDate)
        {
            using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();
                using (MySqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. تحديث البيانات الأساسية
                        UpdateVisitDetails(VisitID, AppointmentID, Diagnosis, VisitNotes, CreatedByUserID, DoctorID, tran);

                        // 2. تحديث الخدمات (حذف ثم إعادة إدراج)
                        clsVisitServicesData.DeleteVisitServices(VisitID, tran);
                        if (services != null && services.Rows.Count > 0)
                            clsVisitServicesData.SaveVisitServices(VisitID, services, tran);

                        // 3. تحديث الوصفة
                        if (dtMedicines == null || dtMedicines.Rows.Count == 0)
                            clsPrescriptionData.DeletePrescription(PrescriptionID, tran);
                        else
                            if (PrescriptionID > 0)
                                clsPrescriptionData.UpdatePrescription(PrescriptionID, VisitID, PrescriptionNotes, PrescriptionDate, dtMedicines, tran);
                            else
                                PrescriptionID = clsPrescriptionData.SavePrescription(VisitID, PrescriptionNotes, PrescriptionDate, dtMedicines, 1, tran);
                        tran.Commit();
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        tran.Rollback();
                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                        return false;
                    }
                }
            }
        }
        private static bool UpdateVisitDetails(int VisitID, int AppointmentID, string Diagnosis, string VisitNotes, int UserID, int DoctorID, MySqlTransaction transaction)
        {
            // 1. تحديث حالة الموعد
            string queryAppt = @"UPDATE Appointments 
                    SET AppointmentStatus = 5,
                    LastModifiedDate = GETDATE(), 
                    ExaminationEndTime = GETDATE(), 
                    LastModifiedByUserID = @UserID 
                    WHERE AppointmentID = @AppointmentID";
            using (MySqlCommand cmdAppt = new MySqlCommand(queryAppt, transaction.Connection, transaction))
            {
                cmdAppt.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                cmdAppt.Parameters.AddWithValue("@UserID", UserID);
                cmdAppt.ExecuteNonQuery();
            }

            // 2. تحديث الزيارة
            string queryVisit = @"UPDATE Visits 
                                SET Diagnosis = @Diagnosis, 
                                VisitDate = GETDATE(),
                                VisitNotes = @VisitNotes,
                                DoctorID = @DoctorID, 
                                VisitStatus = 2
                                WHERE VisitID = @VisitID";
            using (MySqlCommand cmdVisit = new MySqlCommand(queryVisit, transaction.Connection, transaction))
            {
                cmdVisit.Parameters.AddWithValue("@VisitID", VisitID);
                cmdVisit.Parameters.AddWithValue("@Diagnosis", Diagnosis);
                cmdVisit.Parameters.AddWithValue("@VisitNotes", VisitNotes.ToDBValue());
                cmdVisit.Parameters.AddWithValue("@DoctorID", DoctorID);
                cmdVisit.ExecuteNonQuery();
            }
            return true;
        }

       

        public static bool GetVisitByID(int VisitID, ref int PatientID, ref string Diagnosis, ref DateTime VisitDate,
                                        ref int AppointmentID, ref string VisitNotes, ref int DoctorID,
                                        ref byte VisitStatus, ref int CreatedByUserID)
        {
            bool IsFound = false;
            string query = "SELECT * FROM Visits WHERE VisitID = @VisitID";

            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@VisitID", VisitID);
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PatientID = (int)reader["PatientID"];
                            Diagnosis = reader["Diagnosis"].ToString();
                            VisitDate = (DateTime)reader["VisitDate"];
                            AppointmentID = (int)reader["AppointmentID"];
                            VisitNotes = reader["VisitNotes"].ToStringOrEmpty();
                            DoctorID = (int)reader["DoctorID"];
                            VisitStatus = (byte)reader["VisitStatus"];
                            CreatedByUserID = (int)reader["CreatedByUserID"];
                            IsFound = true;
                        }
                    }
                }
                catch (MySqlException ex) { 
                    clsGlobalLogger.LogMySqlException(ex,clsGlobalLogger.LogLevel.Error);
                    IsFound = false;
                }
            }
            return IsFound;
        }

        
       
        public static bool IsVisitClosed(int VisitID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT 1 FROM Visits WHERE VisitID = @VisitID AND VisitStatus = 3";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@VisitID", VisitID);
                        connection.Open();
                        object result = cmd.ExecuteScalar();
                        return result != null; 
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }

           
        }

        public static DataTable GetPatientsWaitingForDoctors(int DoctorPersonID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT VisitID
                                          ,AppointmentID
                                          ,PatientName
                                          ,CheckInTime
                                          ,StatusText
                                          ,IsCalled
                                      FROM View_PatientsWaitingForDoctors
                                      WHERE DoctorPersonID=@DoctorPersonID and StatusText in('Ready_For_Doctor','In-Progress')
                                      ORDER BY CheckInTime;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@DoctorPersonID", DoctorPersonID);
                        connection.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
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


        public static bool DeleteVisit(int VisitID, int AppointmentID)
        {
            int rowsAffected = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Vitals 
                                      SET VisitID=NULL
                                      WHERE AppointmentID=@AppointmentID;
                        DELETE FROM Visits WHERE VisitID = @VisitID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VisitID", VisitID);
                        command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }
            return rowsAffected > 0;
        }

        public static DataTable GetAllVisits()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM View_VisitDetails
                                  ORDER BY VisitDate DESC;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
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