using System.Data;
using MySql.Data.MySqlClient;

namespace my_life_api.Database;

public static class DataBase {
    static public MySqlConnection connection;

    public static async Task OpenConnectionIfClosed() {
        if (connection.State == ConnectionState.Closed) {
            await connection.OpenAsync();
        }
    }

    public static async Task CloseConnection() {
        await connection.CloseAsync();
    }

    public static void ConfigureDataBase(string connectionString) {
        connection = new MySqlConnection(connectionString);
    }
}
