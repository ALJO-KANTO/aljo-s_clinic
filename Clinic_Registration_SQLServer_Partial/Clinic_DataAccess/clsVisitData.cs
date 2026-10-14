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
                        if (!UpdateVisitDetails(VisitID, AppointmentID, Diagnosis,
                            VisitNotes, CreatedByUserID, DoctorID, tran))
                        {
                            throw new Exception("Visit details could not be updated.");
                        }
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
                        if (!UpdateVisitDetails(
                            VisitID,
                            AppointmentID,
                            Diagnosis,
                            VisitNotes,
                            CreatedByUserID,
                            DoctorID,
                            tran))
                        {
                            throw new Exception(
                                "UpdateVisitDetails failed for Visit ID " + VisitID);
                        }
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
                    catch (Exception ex)
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Rollback error: " + rollbackEx);
                        }

                        System.Diagnostics.Debug.WriteLine(
                            "UPDATE VISIT ERROR: " + ex);

                        throw new Exception(
                            "Failed to save Visit ID " + VisitID +
                            ". Details: " + ex.Message,
                            ex);
                    }
                }
            }
        }
        private static bool UpdateVisitDetails(
            int VisitID,
            int AppointmentID,
            string Diagnosis,
            string VisitNotes,
            int UserID,
            int DoctorID,
            MySqlTransaction transaction)
        {
            MySqlConnection connection = transaction.Connection;

            // Update visit details and move the visit to billing.
            string query = @"
    UPDATE visits
    SET
        Diagnosis = @Diagnosis,
        VisitNotes = @VisitNotes,
        DoctorID = @DoctorID,
        VisitStatus = 'Pending_Billing'
    WHERE VisitID = @VisitID;";

            using (MySqlCommand cmd =
     new MySqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@VisitID", VisitID);
                cmd.Parameters.AddWithValue("@Diagnosis",
                    (object)Diagnosis ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VisitNotes",
                    (object)VisitNotes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DoctorID", DoctorID);

                if (cmd.ExecuteNonQuery() == 0)
                    return false;
            }

            // Move appointment to billing.
            string queryAppointment = @"
        UPDATE appointments
SET AppointmentStatus = 'In_Billing'        WHERE AppointmentID = @AppointmentID;";

            using (MySqlCommand cmd =
                new MySqlCommand(queryAppointment, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                cmd.ExecuteNonQuery();
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
                            PatientID = Convert.ToInt32(reader["PatientID"]);
                            Diagnosis = reader["Diagnosis"].ToString();
                            VisitDate = Convert.ToDateTime(reader["VisitDate"]);

                            AppointmentID = reader["AppointmentID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["AppointmentID"]);

                            VisitNotes = reader["VisitNotes"].ToStringOrEmpty();
                            DoctorID = Convert.ToInt32(reader["DoctorID"]);

                            // Convert MySQL VARCHAR status to C# byte status
                            string status = Convert.ToString(reader["VisitStatus"]);

                            switch (status)
                            {
                                case "Open":
                                    VisitStatus = 1;
                                    break;

                                case "Completed":
                                    VisitStatus = 2;
                                    break;

                                case "Closed":
                                    VisitStatus = 3;
                                    break;

                                default:
                                    VisitStatus = 0;
                                    break;
                            }

                            CreatedByUserID = reader["CreatedByUserID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["CreatedByUserID"]);

                            IsFound = true;
                            CreatedByUserID = reader["CreatedByUserID"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["CreatedByUserID"]); IsFound = true;
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
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                SELECT
                    VisitID,
                    AppointmentID,
                    PatientName,
                    CheckInTime,
                    StatusText,
                    IsCalled
                FROM View_PatientsWaitingForDoctors
                WHERE
                    (@DoctorPersonID = 0
                     OR DoctorPersonID = @DoctorPersonID)
                   AND StatusText IN
(
    'Scheduled',
    'Ready_For_Doctor',
    'Progress',
    'InProgress',
    'In-Progress'
)
                ORDER BY CheckInTime;";

                    using (MySqlCommand cmd =
                        new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DoctorPersonID", DoctorPersonID);

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

                throw;
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

            string query = @"
        SELECT *
        FROM View_VisitDetails
        ORDER BY VisitDate DESC;";

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "GetAllVisits ERROR: " + ex);

                throw;
            }

            return dt;
        }
    }
}