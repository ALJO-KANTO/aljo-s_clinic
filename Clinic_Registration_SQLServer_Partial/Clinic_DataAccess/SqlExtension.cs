using System.Data;
using MySql.Data.MySqlClient;
public static class SqlExtension
{
    // دالة لتنفيذ الأوامر البسيطة (مثل CREATE TABLE أو UPDATE بسيط)
    public static int ExecuteNonQuery(this MySqlConnection conn, string query, MySqlTransaction transaction = null)
    {
        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        {
            if (transaction != null)
                cmd.Transaction = transaction;
            return cmd.ExecuteNonQuery();
        }
    }
}