using FluentFTP;
using MySql.Data.MySqlClient;
using my_life_api.Resources;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Database.Managers;

public class SetupDBManager {
    public async Task CreateTables() {
        string creationSql = File.ReadAllText(
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"), 
                "Database/SQL/TableCreations.sql"
            )
        );

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.CommandTimeout = 900;
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText = creationSql;

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task InsertInitialData() {
        string insertSql = File.ReadAllText(
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"), 
                "Database/SQL/Inserts.sql"
            )
        );

        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText = insertSql;

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }
    public async Task CreateFtpFolders() { 
        AsyncFtpClient client = FtpManager.GetClient();

        // Pastas de recursos com imagens
        await client.CreateDirectory(FtpManager.authorPicturesFolder);
        await client.CreateDirectory(FtpManager.recordPicturesFolder);

        // Pastas de conteudos
        foreach (ContentTypeData ctd in ContentUtils.contentTypesData) {
            await client.CreateDirectory(ctd.storageFolder);
        }
    }
}
