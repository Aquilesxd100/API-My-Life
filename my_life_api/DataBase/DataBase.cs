using System.Data;
using MySql.Data.MySqlClient;

namespace my_life_api.Resources
{
    public static class DataBase
    {
        static private MySqlConnection connection;

        public static async Task OpenConnectionIfClosed()
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
        }

        public static async Task CloseConnection()
        {
            await connection.CloseAsync();
        }

        public static async Task ConnectToDataBase(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
        }




        public static async Task<string> TesteConexao()
        {
            await OpenConnectionIfClosed();
            try
            {
                // create a MySQL command and set the SQL statement with parameters
                MySqlCommand myCommand = new MySqlCommand();
                myCommand.Connection = connection;
                myCommand.CommandText = @"SELECT * FROM teste;";

                // execute the command and read the results
                List<string> nomesBanco = new List<string>();
                using var myReader = await myCommand.ExecuteReaderAsync();
                while (myReader.Read())
                {
                    nomesBanco.Add(myReader.GetString("nome"));
                }

                await CloseConnection();
                return $"o nome do primeiro item é {nomesBanco[0]}, do ultimo item é {nomesBanco[(nomesBanco.Count() - 1)]}";
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return "Um erro ocorreu!";
            }
        }
    }
}
