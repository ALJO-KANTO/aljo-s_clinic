
using Clinic_DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Data;

public static class clsRoleData
{
    public static bool FindByName(
        string RoleName, ref int RoleID)
    {
        using (var connection = ClinicDatabase.GetConnection())
        using (var command = new MySqlCommand(@"
            SELECT RoleID
            FROM roles
            WHERE RoleName = @RoleName
            LIMIT 1;", connection))
        {
            command.Parameters.AddWithValue("@RoleName", RoleName);

            connection.Open();

            object result = command.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return false;

            RoleID = Convert.ToInt32(result);
            return true;
        }
    }

    public static bool FindByID(
        int RoleID, ref string RoleName)
    {
        using (var connection = ClinicDatabase.GetConnection())
        using (var command = new MySqlCommand(@"
            SELECT RoleName
            FROM roles
            WHERE RoleID = @RoleID
            LIMIT 1;", connection))
        {
            command.Parameters.AddWithValue("@RoleID", RoleID);

            connection.Open();

            object result = command.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return false;

            RoleName = Convert.ToString(result);
            return true;
        }
    }

    public static DataTable GetAllRoles()
    {
        DataTable dt = new DataTable();

        using (var connection = ClinicDatabase.GetConnection())
        using (var command = new MySqlCommand(@"
            SELECT RoleID, RoleName
            FROM roles
            ORDER BY RoleName ASC;", connection))
        using (var adapter = new MySqlDataAdapter(command))
        {
            connection.Open();
            adapter.Fill(dt);
        }

        return dt;
    }
}
