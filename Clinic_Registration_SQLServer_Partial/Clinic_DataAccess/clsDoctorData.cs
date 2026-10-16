using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{

    public class clsDoctorData
    {

        public static bool GetDoctorByID(
            int DoctorID,
            ref int PersonID,
            ref float ConsultationFees,
            ref string LicenseNumber,
            ref bool IsActive,
            ref DateTime CreatedDate,
            ref int CreatedByUserID)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                SELECT
                    PersonID,
                    ConsultationFees,
                    LicenseNumber,
                    CreatedByUserID,
                    IsActive
                FROM doctors
                WHERE DoctorID = @DoctorID;";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return false;

                            PersonID = Convert.ToInt32(reader["PersonID"]);
                            ConsultationFees =
                                Convert.ToSingle(reader["ConsultationFees"]);

                            LicenseNumber =
                                reader["LicenseNumber"] == DBNull.Value
                                    ? ""
                                    : reader["LicenseNumber"].ToString();

                            CreatedByUserID =
                                reader["CreatedByUserID"] == DBNull.Value
                                    ? -1
                                    : Convert.ToInt32(reader["CreatedByUserID"]);

                            IsActive = Convert.ToBoolean(reader["IsActive"]);
                            CreatedDate = DateTime.MinValue;

                            return true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex, clsGlobalLogger.LogLevel.Error);

                return false;
            }
        }

        private static List<int> GetGenericList(int DoctorID, string query, string parameterName)
        {
            List<int> listIDs = new List<int>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(parameterName, DoctorID);
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listIDs.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);

            }

            return listIDs;
        }

        public static List<int> GetDoctorSpecialtyIDs(int DoctorID)
        {
            string query = "SELECT SpecializationID FROM DoctorSpecialties WHERE DoctorID = @DoctorID";
            return GetGenericList(DoctorID, query, "@DoctorID");
        }

        public static bool DeleteDoctor(int doctorID)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendLine("DELETE FROM DoctorSpecialties WHERE DoctorID = @DoctorID;");
            queryBuilder.AppendLine("DELETE FROM DoctorWorkingDays WHERE DoctorID = @DoctorID;");
            queryBuilder.AppendLine("DELETE FROM Doctors WHERE DoctorID = @DoctorID;");

            MySqlTransaction transaction = null;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    using (transaction = connection.BeginTransaction())
                    {
                        using (MySqlCommand command = new MySqlCommand(queryBuilder.ToString(), connection, transaction))
                        {
                            command.Parameters.AddWithValue("@DoctorID", doctorID);

                            // تنفيذ حزمة الحذف الشاملة
                            command.ExecuteNonQuery();

                            // إذا وصلنا هنا بدون أي MySqlException، نعتمد الحذف فوراً
                            transaction.Commit();
                            return true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }



        }
        public static DataTable GetAllDoctors()
        {
            DataTable dt = new DataTable();


            string query = @"
    SELECT
        DoctorID,
        PersonID,
        FullName,
        LicenseNumber,
        ConsultationFees,
        Specializations,
        WorkingDays,
        CountryName,
        IsActive
    FROM View_DoctorsDetails
    ORDER BY DoctorID DESC;";


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

            return dt;
        }

        public static DataRow GetDoctorByID(int DoctorID)
        {
            DataTable dt = new DataTable();

            string query = @"
        SELECT
            d.DoctorID,
            d.PersonID,
            p.FullName,
            d.LicenseNumber,
            d.ConsultationFees,
            s.SpecializationName AS Specializations,
            '' AS WorkingDays,
            COALESCE(c.CountryName, '') AS CountryName,
            d.IsActive
        FROM doctors d
        LEFT JOIN persons p
            ON p.PersonID = d.PersonID
        LEFT JOIN specializations s
            ON s.SpecializationID = d.SpecializationID
        LEFT JOIN countries c
            ON c.CountryID = p.NationalityCountryID
        WHERE d.DoctorID = @DoctorID;";

            using (MySqlConnection connection =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            using (MySqlCommand command =
                new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DoctorID", DoctorID);

                connection.Open();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        private static string GetGenericString(int DoctorID, string query, string parameterName, string defaultValue)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(parameterName, DoctorID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        // نتحقق إذا كانت النتيجة فارغة (null) أو فارغة نصياً (DBNull)
                        return (result != null && result != DBNull.Value) ? result.ToString() : defaultValue;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return defaultValue;
            }


        }

        public static string GetSpecializations(int DoctorID)
        {
            string query = @"
        SELECT s.SpecializationName
        FROM doctors d
        INNER JOIN specializations s
            ON s.SpecializationID = d.SpecializationID
        WHERE d.DoctorID = @DoctorID;";

            return GetGenericString(
                DoctorID,
                query,
                "@DoctorID",
                "No Specialization Found");
        }

        public static string GetWorkingDays(int DoctorID)
        {
            string query = @"
        SELECT GROUP_CONCAT(
            DA.DayName
            ORDER BY DA.DayID
            SEPARATOR ', '
        )
        FROM doctorworkingdays DW
        INNER JOIN days DA
            ON DA.DayID = DW.DayID
        WHERE DW.DoctorID = @DoctorID;";

            return GetGenericString(
                DoctorID,
                query,
                "@DoctorID",
                "No Working Days Set");
        }
        public static float GetConsultationFees(int DoctorID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "Select ConsultationFees From Doctors Where DoctorID=@DoctorID;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        connection.Open ();
                        object result = command.ExecuteScalar();
                        if (result != null)
                            return Convert.ToSingle(result);

                    }
                }

            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);

            }
            return 0.0F;
        }
        public static bool IsDoctorExistForPersonID(int PersonID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT 1\r\nFROM doctors\r\nWHERE PersonID = @PersonID\r\nLIMIT 1;";
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

    
        public static bool IsDoctorWorkingOnThisDay(int DoctorID, DateTime AppointmentDate)
        {
            // تحويل يوم C# نظامي (السبت=1، الأحد=2... الجمعة=7)
            int dayOfWeekCS = (int)AppointmentDate.DayOfWeek; // 0=Sun, 1=Mon ... 6=Sat
            int MyDayID = (dayOfWeekCS == 6) ? 1 : (dayOfWeekCS + 2);

            try
            {

                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT 1 FROM DoctorWorkingDays 
                         WHERE DoctorID = @DoctorID AND DayID = @DayID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        command.Parameters.AddWithValue("@DayID", MyDayID);
                        connection.Open();
                        return command.ExecuteScalar() != null;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }

        }

        public static bool IsDoctorWorkingAtThisTime(int DoctorID, DateTime AppointmentDateTime)
        {
            int dayOfWeekCS = (int)AppointmentDateTime.DayOfWeek;
            int dayOfWeek = (dayOfWeekCS == 6) ? 1 : (dayOfWeekCS + 2);
            TimeSpan appointmentTime = AppointmentDateTime.TimeOfDay;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT 1 FROM DoctorWorkingDays 
                         WHERE DoctorID = @DoctorID 
                         AND DayID = @DayID 
                         AND @Time BETWEEN StartTime AND EndTime";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        command.Parameters.AddWithValue("@DayID", dayOfWeek);
                        command.Parameters.AddWithValue("@Time", appointmentTime);


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

        public static DataTable GetAllDays()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Days order by DayID;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
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

        public static DataTable GetDoctorShifts(int DoctorID)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT DW.DayID,
                                    D.DayName, DW.StartTime, DW.EndTime
                                FROM DoctorWorkingDays DW INNER JOIN Days D ON D.DayID = DW.DayID
                            WHERE DW.DoctorID = @DoctorID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
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

        public static int AddNewDoctor(int PersonID, float ConsultationFees, string LicenseNumber, int CreatedByUserID, bool IsActive,
                        List<int> SelectedSpecialtyIDs, DataTable dtWorkDays)
        {
            using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. إضافة الطبيب الأساسي واسترجاع الـ ID
                        string insertDoctor = @"INSERT INTO Doctors (PersonID, ConsultationFees, LicenseNumber, CreatedByUserID, IsActive) 
                                        VALUES (@PersonID, @ConsultationFees, @LicenseNumber, @CreatedByUserID, @IsActive);
                                        SELECT LAST_INSERT_ID();";

                        int doctorID;
                        using (MySqlCommand cmd = new MySqlCommand(insertDoctor, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@PersonID", PersonID);
                            cmd.Parameters.AddWithValue("@ConsultationFees", ConsultationFees);
                            cmd.Parameters.AddWithValue("@LicenseNumber", LicenseNumber);
                            cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                            cmd.Parameters.AddWithValue("@IsActive", IsActive);
                            doctorID = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 2. تجهيز البيانات للتخصصات (بدون بناء SQL String)
                        DataTable dtSpecs = new DataTable();
                        dtSpecs.Columns.Add("DoctorID", typeof(int));
                        dtSpecs.Columns.Add("SpecializationID", typeof(int));
                        SelectedSpecialtyIDs.ForEach(id => dtSpecs.Rows.Add(doctorID, id));

                        // 3. تجهيز البيانات لأيام العمل
                        dtWorkDays.Columns.Add("DoctorID", typeof(int), doctorID.ToString());

                        // تعبئة البيانات
                       

                        // 4. تنفيذ الـ Bulk Copy (هنا السحر الحقيقي، لا توجد حلقات SQL)
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            // حفظ التخصصات
                            bulkCopy.DestinationTableName = "DoctorSpecialties";
                            bulkCopy.ColumnMappings.Add("DoctorID", "DoctorID");
                            bulkCopy.ColumnMappings.Add("SpecializationID", "SpecializationID");
                            bulkCopy.WriteToServer(dtSpecs);
                            bulkCopy.ColumnMappings.Clear();
                            // حفظ أيام العمل (بدون ShiftID)
                            bulkCopy.DestinationTableName = "DoctorWorkingDays";
                            bulkCopy.ColumnMappings.Add("DoctorID", "DoctorID");
                            bulkCopy.ColumnMappings.Add("DayID", "DayID");
                            bulkCopy.ColumnMappings.Add("StartTime", "StartTime");
                            bulkCopy.ColumnMappings.Add("EndTime", "EndTime");
                            bulkCopy.WriteToServer(dtWorkDays);
                        }

                        transaction.Commit();
                        return doctorID;
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);

                        return -1;
                    }
                }
            }
        }

        public static bool UpdateDoctor(int DoctorID, int PersonID, float ConsultationFees,
                                string LicenseNumber, int CreatedByUserID, bool IsActive,
                                List<int> SelectedSpecialtyIDs,
                                DataTable dtWorkDays)
        {
            StringBuilder queryBuilder = new StringBuilder();

            // 1. تحديث البيانات الأساسية
            queryBuilder.AppendLine(@"UPDATE Doctors 
                              SET PersonID=@PersonID, ConsultationFees=@ConsultationFees, 
                                  LicenseNumber=@LicenseNumber, CreatedByUserID=@CreatedByUserID, 
                                  IsActive=@IsActive 
                              WHERE DoctorID=@DoctorID;");

            // 2. حذف البيانات القديمة لضمان عدم التكرار (Clean and Re-insert)
            queryBuilder.AppendLine("DELETE FROM DoctorSpecialties WHERE DoctorID = @DoctorID;");
            queryBuilder.AppendLine("DELETE FROM DoctorWorkingDays WHERE DoctorID = @DoctorID;");
            MySqlTransaction transaction = null;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    using (transaction = connection.BeginTransaction())
                    {
                        using (MySqlCommand cmd = new MySqlCommand(queryBuilder.ToString(), connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@DoctorID", DoctorID);
                            cmd.Parameters.AddWithValue("@PersonID", PersonID);
                            cmd.Parameters.AddWithValue("@ConsultationFees", ConsultationFees);
                            cmd.Parameters.AddWithValue("@LicenseNumber", LicenseNumber);
                            cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                            cmd.Parameters.AddWithValue("@IsActive", IsActive);
                            cmd.ExecuteNonQuery();
                        }
                        DataTable dtSpecs = new DataTable();
                        dtSpecs.Columns.Add("DoctorID", typeof(int));
                        dtSpecs.Columns.Add("SpecializationID", typeof(int));
                        SelectedSpecialtyIDs.ForEach(id => dtSpecs.Rows.Add(DoctorID, id));

                        // 3. تجهيز البيانات لأيام العمل
                        dtWorkDays.Columns.Add("DoctorID", typeof(int), DoctorID.ToString());

                        // تعبئة البيانات


                        // 4. تنفيذ الـ Bulk Copy (هنا السحر الحقيقي، لا توجد حلقات SQL)
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {
                            // حفظ التخصصات
                            bulkCopy.DestinationTableName = "DoctorSpecialties";
                            bulkCopy.ColumnMappings.Add("DoctorID", "DoctorID");
                            bulkCopy.ColumnMappings.Add("SpecializationID", "SpecializationID");
                            bulkCopy.WriteToServer(dtSpecs);
                            bulkCopy.ColumnMappings.Clear();
                            // حفظ أيام العمل (بدون ShiftID)
                            bulkCopy.DestinationTableName = "DoctorWorkingDays";
                            bulkCopy.ColumnMappings.Add("DoctorID", "DoctorID");
                            bulkCopy.ColumnMappings.Add("DayID", "DayID");
                            bulkCopy.ColumnMappings.Add("StartTime", "StartTime");
                            bulkCopy.ColumnMappings.Add("EndTime", "EndTime");
                            bulkCopy.WriteToServer(dtWorkDays);
                        }

                       
                            transaction.Commit();
                            return true;

                        
                    }
                }
              
            }   
            catch (MySqlException ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, CreatedByUserID);
                return false;

            }

        }

        public static DataTable LoadQueueData()
        {
            DataTable dt = new DataTable();
            try
            {
                string query = @"SELECT
                                        ROW_NUMBER() OVER (ORDER BY A.AppointmentDate ASC) AS QueueNumber, 
                                        Pe.FullName AS PatientName
                                    FROM Appointments A inner join Patients P ON A.PatientID=P.PatientID
                                      INNER JOIN Persons Pe ON P.PersonID=Pe.PersonID
                                    WHERE A.AppointmentStatus = 1;";
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
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

        public static bool IsDoctorAvailable(
      int DoctorID,
      DateTime AppointmentDateTime)
        {
            try
            {
                // MySQL days table:
                // Saturday = 1
                // Sunday = 2
                // Monday = 3
                // Tuesday = 4
                // Wednesday = 5
                // Thursday = 6
                // Friday = 7

                int dayOfWeekCS = (int)AppointmentDateTime.DayOfWeek;

                int dayID = (dayOfWeekCS == 6)
                    ? 1
                    : dayOfWeekCS + 2;

                using (MySqlConnection connection =
                    new MySqlConnection(
                        clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();

                    // STEP 1: Check doctor's working day and time
                    string workingDayQuery = @"
                SELECT COUNT(*)
                FROM doctorworkingdays
                WHERE DoctorID = @DoctorID
                  AND DayID = @DayID
                  AND StartTime <= @AppointmentTime
                  AND EndTime >= @AppointmentTime;";

                    bool isWorkingTime;

                    using (MySqlCommand command =
                        new MySqlCommand(workingDayQuery, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@DoctorID", DoctorID);

                        command.Parameters.AddWithValue(
                            "@DayID", dayID);

                        command.Parameters.AddWithValue(
                            "@AppointmentTime",
                            AppointmentDateTime.ToString("HH:mm:ss"));

                        int count = Convert.ToInt32(
                            command.ExecuteScalar());

                        isWorkingTime = count > 0;
                    }

                    if (!isWorkingTime)
                        return false;

                    // STEP 2: Check whether an appointment
                    // already exists at the selected date and time
                    string appointmentQuery = @"
                SELECT COUNT(*)
                FROM appointments
                WHERE DoctorID = @DoctorID
                  AND AppointmentDate = @AppointmentDateTime
                  AND AppointmentStatus <> 'Cancelled';";

                    using (MySqlCommand command =
                        new MySqlCommand(appointmentQuery, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@DoctorID", DoctorID);

                        command.Parameters.AddWithValue(
                            "@AppointmentDateTime",
                            AppointmentDateTime);

                        int existingAppointments =
                            Convert.ToInt32(command.ExecuteScalar());

                        return existingAppointments == 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error);

                System.Diagnostics.Debug.WriteLine(
                    "Doctor availability error: " + ex);

                return false;
            }
        }
        public static DataTable GetAllDoctorsForQueue()
        {
            DataTable dt = new DataTable();
            
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT D.DoctorID, P.FullName 
                         FROM Doctors D
                         INNER JOIN Persons P ON D.PersonID = P.PersonID;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                }

            }
            catch(MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }
            return dt;
        }
    }
}