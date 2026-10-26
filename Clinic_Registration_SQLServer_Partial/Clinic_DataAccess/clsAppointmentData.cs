using Clinic_DataAccess.SaveMySqlException;
using System;
using System.Data;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using Clinic_Common;
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

                                PatientID = Convert.ToInt32(reader["PatientID"]);
                                DoctorID = Convert.ToInt32(reader["DoctorID"]);
                                CreatedByUserID =
                                    reader["CreatedByUserID"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(reader["CreatedByUserID"]);

                                AppointmentStatus = StatusFromDatabase(
     Convert.ToString(reader["AppointmentStatus"]));

                                AppointmentTypeID =
                                    Convert.ToInt32(reader["AppointmentTypeID"]);

                                AppointmentFees =
                                    Convert.ToDecimal(reader["AppointmentFees"]);

                                AppointmentDate =
                                    Convert.ToDateTime(reader["AppointmentDate"]);

                                int createdDateIndex = -1;

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (string.Equals(
                                        reader.GetName(i),
                                        "CreatedDate",
                                        StringComparison.OrdinalIgnoreCase))
                                    {
                                        createdDateIndex = i;
                                        break;
                                    }
                                }

                                if (createdDateIndex >= 0 &&
                                    !reader.IsDBNull(createdDateIndex))
                                {
                                    CreatedDate =
                                        Convert.ToDateTime(reader.GetValue(createdDateIndex));
                                }
                                else
                                {
                                    CreatedDate = AppointmentDate;
                                }
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

        private static byte StatusFromDatabase(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new InvalidOperationException(
                    "Appointment status cannot be empty.");

            status = status.Trim();

            // Support existing numeric statuses: 1 to 9
            if (byte.TryParse(status, out byte numericStatus))
            {
                if (numericStatus >= 1 && numericStatus <= 9)
                    return numericStatus;
            }

            // Support text statuses
            switch (status)
            {
                case "Scheduled": return 1;
                case "InQueue": return 2;
                case "Progress": return 3;
                case "Postponed": return 4;
                case "Completed": return 5;
                case "Cancelled": return 6;
                case "Waiting_For_Vitals": return 7;
                case "Ready_For_Doctor": return 8;
                case "In_Billing": return 9;

                default:
                    throw new InvalidOperationException(
                        "Unknown appointment status: " + status);
            }
        }
        private static string StatusToDatabase(byte status)
        {
            switch (status)
            {
                case 1: return "Scheduled";
                case 2: return "InQueue";
                case 3: return "Progress";
                case 4: return "Postponed";
                case 5: return "Completed";
                case 6: return "Cancelled";
                case 7: return "Waiting_For_Vitals";
                case 8: return "Ready_For_Doctor";
                case 9: return "In_Billing";

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(status));
            }
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
                        command.Parameters.AddWithValue(
                            "@AppointmentStatus",
                            StatusToDatabase(AppointmentStatus)); command.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
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
                        command.Parameters.AddWithValue(
                            "@AppointmentStatus",
                            StatusToDatabase(AppointmentStatus)); command.Parameters.AddWithValue("@AppointmentTypeID", AppointmentTypeID);
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

            string query = @"
        SELECT
            AppointmentID,
            PatientID,
            PatientName,
            DoctorID,
            DoctorName,
            AppointmentDate,
            AppointmentStatus,
            AppointmentStatusText,
            AppointmentTypeID,
            TypeName,
            AppointmentFees,
            CreatedByUserID
        FROM View_AppointmentsDetails
        ORDER BY AppointmentID DESC;";

            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(
                        clsDataAccessSettings.ConnectionString))
                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (MySqlException ex)
            {
                clsGlobalLogger.LogMySqlException(
                    ex,
                    clsGlobalLogger.LogLevel.Error);

                System.Diagnostics.Debug.WriteLine(
                    "GetAllAppointments ERROR: " + ex);

                // Do not silently return an empty table
                throw;
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
                    string query = @"
    SELECT 1
    FROM Appointments
    WHERE AppointmentID = @AppointmentID
    LIMIT 1;"; using (MySqlCommand command = new MySqlCommand(query, connection))
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
                                SELECT * 
                                FROM Appointments 
                                WHERE PatientID = @PatientID 
                                AND AppointmentStatus <> 'Cancelled'
                                AND AppointmentDate BETWEEN @StartRange AND @EndRange LIMIT 1";

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
        public static bool IsTimeCapacityAvailable(
            int DoctorID,
            DateTime AppointmentDate,
            int AppointmentTypeID)
        {
            try
            {
                using (MySqlConnection conn =
                    new MySqlConnection(
                        clsDataAccessSettings.ConnectionString))
                {
                    string query = @"
                SELECT
                    CASE
                        WHEN COALESCE(
                            (
                                SELECT SUM(
                                    TIMESTAMPDIFF(
                                        MINUTE,
                                        GREATEST(
                                            DW.StartTime,
                                            TIME(@AppointmentDate)
                                        ),
                                        DW.EndTime
                                    )
                                )
                                FROM doctorworkingdays DW
                                WHERE DW.DoctorID = @DoctorID
                                  AND DW.DayID = @DayID
                                  AND DW.EndTime >
                                      TIME(@AppointmentDate)
                            ), 0
                        ) >=
                        (
                            SELECT COUNT(*) * 30
                            FROM appointments A
                            WHERE A.DoctorID = @DoctorID
                              AND DATE(A.AppointmentDate) =
                                  DATE(@AppointmentDate)
                              AND A.AppointmentDate >=
                                  @AppointmentDate
                              AND A.AppointmentStatus NOT IN
                                  ('Cancelled', '6')
                        ) + 30
                        THEN 1
                        ELSE 0
                    END AS IsCapacityAvailable;";

                    using (MySqlCommand cmd =
                        new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DoctorID", DoctorID);

                        cmd.Parameters.AddWithValue(
                            "@DayID", GetDayID(AppointmentDate));

                        cmd.Parameters.AddWithValue(
                            "@AppointmentDate", AppointmentDate);

                        conn.Open();

                        object result = cmd.ExecuteScalar();

                        return result != null &&
                               result != DBNull.Value &&
                               Convert.ToInt32(result) == 1;
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
        }
        public static bool UpdateAppointmentStatus(
          int AppointmentID, enAppointmentStatus Status,
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
            enAppointmentStatus Status,
            int UserID,
            MySqlTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (!Enum.IsDefined(typeof(enAppointmentStatus), Status))
            {
                throw new ArgumentException("Invalid appointment status.", nameof(Status));
            }

            string query = @"
                    UPDATE appointments
                    SET AppointmentStatus = @Status
                    WHERE AppointmentID = @AppointmentID
                      AND AppointmentStatus = @ExpectedStatus;";

            using (MySqlCommand command = new MySqlCommand(
                query,
                transaction.Connection,
                transaction))
            {
                command.Parameters.AddWithValue(
                    "@AppointmentID", AppointmentID);

                command.Parameters.AddWithValue(
                    "@Status", (byte)Status);

                command.Parameters.AddWithValue(
                    "@ExpectedStatus",
                    (byte)enAppointmentStatus.Scheduled
                );

                return command.ExecuteNonQuery() > 0;
            }
        }

        public static DataTable GetWaitingPatients(int DoctorID)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn =
                new MySqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT
                AppointmentID,
                PatientName,
                CallType,
                IsCalled,
                CheckInTime,
                StatusText
            FROM View_WaitingPatients
            WHERE DoctorID = @DoctorID
              AND StatusText IN
              (
                  'InQueue',
                  'Waiting_For_Vitals',
                  'Ready_For_Doctor'
              )
            ORDER BY CheckInTime ASC;";

                using (MySqlCommand cmd =
                    new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", DoctorID);

                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
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
                                    SET AppointmentStatus = 'Waiting_For_Vitals'
                                    WHERE AppointmentID IN (
                                        SELECT AppointmentID
                                        FROM (
                                            SELECT AppointmentID
                                            FROM Appointments
                                            WHERE AppointmentStatus = 'InQueue'
                                              AND DoctorID = @DoctorID
                                            ORDER BY CheckInTime ASC
                                            LIMIT @Count
                                        ) AS SelectedAppointments
                                    );";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Count", Count);
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);

                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();
                        IsPromoted = rowsAffected > 0;
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
                    string updateQuery = @"UPDATE Appointments SET AppointmentStatus = 6, LastModifiedDate = NOW(), 
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
