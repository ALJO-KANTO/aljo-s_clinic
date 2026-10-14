using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
namespace Clinic_DataAccess
{
    public class clsAppointmentData
    {
        public static bool GetAppointmentByID(int AppointmentID, ref int PatientID, ref int DoctorID,
            ref int CreatedByUserID, ref byte AppointmentStatus, ref int AppointmentTypeID,
            ref decimal AppointmentFees, ref DateTime AppointmentDate, ref DateTime CreatedDate)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM Appointments WHERE AppointmentID = @AppointmentID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                PatientID = (int)reader["PatientID"];
                                DoctorID = (int)reader["DoctorID"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                                AppointmentStatus = (byte)reader["AppointmentStatus"];
                                AppointmentTypeID = (int)reader["AppointmentTypeID"];
                                AppointmentFees = (decimal)reader["AppointmentFees"];
                                AppointmentDate = (DateTime)reader["AppointmentDate"];
                                CreatedDate = (DateTime)reader["CreatedDate"];
                                return true;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }
            return false;
        }


        public static DataRow GetAppointmentInfoByID(int AppointmentID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM View_AppointmentsDetails WHERE AppointmentID = @AppointmentID;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppointmentID", AppointmentID);

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

            return (dt.Rows.Count > 0) ? dt.Rows[0] : null;
        }

        public static int AddNewAppointment(int PatientID, int DoctorID, int CreatedByUserID,
            byte AppointmentStatus, int AppointmentTypeID, decimal AppointmentFees, DateTime AppointmentDate)
        {
            int AppointmentID = -1;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO Appointments (PatientID, DoctorID, CreatedByUserID, AppointmentStatus, AppointmentTypeID, AppointmentFees, AppointmentDate)
                                 VALUES (@PatientID, @DoctorID, @CreatedByUserID, @AppointmentStatus, @AppointmentTypeID, @AppointmentFees, @AppointmentDate);
                                 SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PatientID", PatientID);
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        command.Parameters.AddWithValue("@AppointmentStatus", AppointmentStatus);
                        command.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
                        command.Parameters.AddWithValue("@AppointmentFees", AppointmentFees);
                        command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);


                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                            AppointmentID = insertedID;
                    }
                }

            }

            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, CreatedByUserID);
            }

            return AppointmentID;
        }

        public static bool UpdateAppointment(int AppointmentID, int PatientID, int DoctorID, int CreatedByUserID,
            byte AppointmentStatus, int AppointmentTypeID, decimal AppointmentFees, DateTime AppointmentDate)
        {
            int rowsAffected = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE Appointments SET PatientID = @PatientID, DoctorID = @DoctorID, 
                                 CreatedByUserID = @CreatedByUserID, AppointmentStatus = @AppointmentStatus, 
                                 AppointmentTypeID = @AppointmentTypeID, AppointmentFees = @AppointmentFees, 
                                 AppointmentDate = @AppointmentDate
                                 WHERE AppointmentID = @AppointmentID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                        command.Parameters.AddWithValue("@PatientID", PatientID);
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        command.Parameters.AddWithValue("@AppointmentStatus", AppointmentStatus);
                        command.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
                        command.Parameters.AddWithValue("@AppointmentFees", AppointmentFees);
                        command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);


                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error, CreatedByUserID);
            }

            return (rowsAffected > 0);
        }

        public static bool DeleteAppointment(int AppointmentID)
        {
            int rowsAffected = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "DELETE FROM Appointments WHERE AppointmentID = @AppointmentID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppointmentID", AppointmentID);


                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllAppointments()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT * FROM View_AppointmentsDetails
                                         ORDER BY AppointmentID DESC";
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


        public static bool DoesAppointmentExist(int AppointmentID)
        {
            bool IsFound = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT Found = 1 FROM Appointments WHERE AppointmentID = @AppointmentID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppointmentID", AppointmentID);

                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null) IsFound = true;

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

       
        public static bool IsDoctorBusy(int DoctorID, DateTime AppointmentDate)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT 1 FROM Appointments 
                                    WHERE DoctorID = @DoctorID 
                                    AND AppointmentDate = @AppointmentDate 
                                     AND AppointmentStatus !=6"; // 6 = Canceled

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);

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

        public static bool IsPatinentBlakListed(int PatientID)
        {
            bool IsBlacklisted = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"Select 1 From PatientBlacklist
                              Where PatientID=@PatientID
                                and UnbanDate IS NULL";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PatientID", PatientID);
                        connection.Open();
                        return (command.ExecuteScalar() != null);

                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }
            return IsBlacklisted;
        }

        private static bool _IsPatientAlreadyBookedInTimeRange(int PatientID, DateTime AppointmentDateTime, TimeSpan range)
        {
            bool HasAppointment = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                                SELECT TOP 1 1 
                                FROM Appointments 
                                WHERE PatientID = @PatientID 
                                AND AppointmentStatus != 6
                                AND AppointmentDate BETWEEN @StartRange AND @EndRange";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // حساب نطاق الوقت (قبل وبعد الموعد بـ 24 ساعة)
                        DateTime startRange = AppointmentDateTime.Subtract(range);
                        DateTime endRange = AppointmentDateTime.Add(range);

                        command.Parameters.AddWithValue("@PatientID", PatientID);
                        command.Parameters.AddWithValue("@StartRange", startRange);
                        command.Parameters.AddWithValue("@EndRange", endRange);

                        connection.Open();
                        object result = command.ExecuteScalar();
                        return (result != null);
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }
            return HasAppointment;
        }

        public static bool IsPatientHasConflict(int PatientID, DateTime AppointmentDateTime)
        {
            // نحدد النطاق الزمني الذي نعتبره "تضارباً" (مثلاً 24 ساعة كما اتفقنا)
            TimeSpan conflictRange = TimeSpan.FromHours(24);

            // استدعاء دالة التحقق بنطاق زمني
            return _IsPatientAlreadyBookedInTimeRange(PatientID, AppointmentDateTime, conflictRange);
        }
        private static int GetDayID(DateTime dateTime)
        {
            int DayOfWeek = (int)dateTime.DayOfWeek;

            return (DayOfWeek == 6) ? 1 : (DayOfWeek + 2);
        }
        public static bool IsTimeCapacityAvailable(int DoctorID, DateTime AppointmentDate, int AppointmentTypeID)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // الاستعلام المحدث ليتوافق مع جداولك الحالية
                    string query = @"
            -- 1. جلب مدة الموعد الجديد
            DECLARE @NewDuration INT = (SELECT TOP 1 DefaultDuration FROM AppointmentTypes WHERE AppointmentTypeID = @AppointmentTypeID);
            
            -- 2. حساب السعة المتبقية من اليوم (فقط الفترات المستقبلية)
            DECLARE @DailyCap INT = (SELECT ISNULL(SUM(DATEDIFF(MINUTE, 
                                     CASE 
                                        WHEN StartTime > CAST(@Date AS TIME) THEN StartTime 
                                        ELSE CAST(@Date AS TIME) 
                                     END, EndTime)), 0)
                                     FROM DoctorWorkingDays 
                                     WHERE DoctorID = @DoctorID AND DayID = @DayID
                                     AND EndTime > CAST(@Date AS TIME));

            -- 3. حساب المستهلك من المواعيد المستقبلية فقط
            DECLARE @Consumed INT = (SELECT ISNULL(SUM(AT.DefaultDuration), 0)
                                     FROM Appointments A
                                     INNER JOIN AppointmentTypes AT ON A.AppointmentTypeID = AT.AppointmentTypeID
                                     WHERE A.DoctorID = @DoctorID
                                     AND CAST(A.AppointmentDate AS DATE) = CAST(@Date AS DATE)
                                     AND A.AppointmentStatus != 6 
                                     AND A.AppointmentDate > GETDATE());
                                     
            SELECT CASE WHEN (@Consumed + @NewDuration) <= @DailyCap THEN 1 ELSE 0 END;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", DoctorID);
                        cmd.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
                        cmd.Parameters.AddWithValue("@DayID", GetDayID(AppointmentDate));
                        cmd.Parameters.AddWithValue("@Date", AppointmentDate.Date);

                        conn.Open();
                        return Convert.ToBoolean(cmd.ExecuteScalar());
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }

        }
        public static bool UpdateAppointmentStatus(
      int AppointmentID,
      string Status,
      int UserID)
        {
            try
            {
                using (MySqlConnection conn =
                    new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                UPDATE appointments
                SET AppointmentStatus = @Status
                WHERE AppointmentID = @AppointmentID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                        cmd.Parameters.AddWithValue("@Status", Status);

                        conn.Open();

                        return cmd.ExecuteNonQuery() > 0;
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
        public static bool UpdateAppointmentStatus(
    int AppointmentID,
    string Status,
    int UserID,
    MySqlTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            string query = @"
        UPDATE appointments
        SET AppointmentStatus = @Status
        WHERE AppointmentID = @AppointmentID;";

            using (MySqlCommand command = new MySqlCommand(
                query,
                transaction.Connection,
                transaction))
            {
                command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                command.Parameters.AddWithValue("@Status", Status);

                return command.ExecuteNonQuery() > 0;
            }
        }
        public static DataTable GetWaitingPatients(int DoctorID)
        {
            DataTable dt = new DataTable();

            try
            {

                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @" SELECT AppointmentID
                                          ,PatientName
                                          ,CallType
                                          ,IsCalled
                                          ,CheckInTime
                                          ,StatusText
                                      FROM View_WaitingPatients
                                      WHERE DoctorID = @DoctorID
                                      AND AppointmentStatus IN (2, 7, 8)
                                       ORDER BY CheckInTime ASC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", DoctorID);
                        conn.Open();
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

        public static int GetCountByStatusForDoctor(int Status, int DoctorID)
        {
            int count = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // استعلام لحساب عدد المرضى حسب الحالة ولطبيب معين فقط
                    string query = @"SELECT COUNT(*) FROM Appointments 
                                     WHERE AppointmentStatus = @Status 
                                     AND DoctorID = @DoctorID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", Status);
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);

                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int c))
                        {
                            count = c;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }
            return count;
        }

        public static bool PromotePatientsToVitalsForDoctor(int Count, int DoctorID)
        {
            bool IsPromoted = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // التحديث الذكي: ترقية أول (Count) مريض من الحالة (2) إلى (7)
                    // ملاحظة: نستخدم الترتيب حسب وقت الوصول (CheckInTime) لضمان العدالة
                    string query = @"
                UPDATE Appointments 
                SET AppointmentStatus = 7 
                WHERE AppointmentID IN (
                    SELECT TOP (@Count) AppointmentID 
                    FROM Appointments 
                    WHERE AppointmentStatus = 2 
                    AND DoctorID = @DoctorID 
                    ORDER BY CheckInTime ASC
                );";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Count", Count);
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        IsPromoted = (rowsAffected > 0);
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                IsPromoted = false;
            }
            return IsPromoted;
        }

        public static bool RescheduleAppointmentTransaction(int OldAppointmentID, int PatientID, int DoctorID,
                           int CreatedByUserID, int AppointmentTypeID, decimal AppointmentFees, DateTime NewDate, int UserID)
        {
            MySqlTransaction transaction = null;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();


                    // 1. إضافة الموعد الجديد
                    string insertQuery = @"INSERT INTO Appointments (PatientID, DoctorID, CreatedByUserID, AppointmentStatus, 
                                 AppointmentTypeID, AppointmentFees, AppointmentDate)
                                 VALUES (@PatientID, @DoctorID, @CreatedByUserID, 1, @AppointmentTypeID, @AppointmentFees, @AppointmentDate);
                                 SELECT LAST_INSERT_ID();";

                    MySqlCommand cmdInsert = new MySqlCommand(insertQuery, connection, transaction);
                    cmdInsert.Parameters.AddWithValue("@PatientID", PatientID);
                    cmdInsert.Parameters.AddWithValue("@DoctorID", DoctorID);
                    cmdInsert.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    cmdInsert.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
                    cmdInsert.Parameters.AddWithValue("@AppointmentFees", AppointmentFees);
                    cmdInsert.Parameters.AddWithValue("@AppointmentDate", NewDate);

                    object result = cmdInsert.ExecuteScalar();

                    // 2. إلغاء الموعد القديم
                    string updateQuery = @"UPDATE Appointments SET AppointmentStatus = 6, LastModifiedDate = GETDATE(), 
                                 LastModifiedByUserID = @UserID WHERE AppointmentID = @OldAppointmentID";

                    MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, connection, transaction);
                    cmdUpdate.Parameters.AddWithValue("@OldAppointmentID", OldAppointmentID);
                    cmdUpdate.Parameters.AddWithValue("@UserID", UserID);
                    cmdUpdate.ExecuteNonQuery();

                    transaction.Commit(); // إذا نجح الإجراءان، اعتمد التغييرات
                    return true;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // إذا فشل أي شيء، تراجع عن كل شيء
                clsGlobalLogger.LogMySqlException((MySqlException)ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }
        }

        public static bool UpdatePatientCallStatus(int AppointmentID, bool IsCalled, int CallType)
        {
            try
            {

                using (MySqlConnection conn = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                                UPDATE Appointments 
                                SET IsCalled = @IsCalled, 
                                    CallType = @CallType
                                WHERE AppointmentID = @AppointmentID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                        cmd.Parameters.AddWithValue("@IsCalled", IsCalled);
                        cmd.Parameters.AddWithValue("@CallType", CallType);


                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return (rowsAffected > 0);
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
                return false;
            }
        }

        public static decimal GetAppointmentFees(int AppointmentID)
        {
            decimal AppointmentFees = 0.0m;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = " Select  AppointmentFees From Appointments Where AppointmentID = @AppointmentID;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);

                        connection.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            AppointmentFees = Convert.ToDecimal(result);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
               
                clsGlobalLogger.LogMySqlException(ex, clsGlobalLogger.LogLevel.Error);
            }
            return AppointmentFees;
        }

    }
}
