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
        string storageBaseUrl = Environment.GetEnvironmentVariable("DATA_BASE_URL");

        AsyncFtpClient client = new AsyncFtpClient(
            Environment.GetEnvironmentVariable("FTP_SERVER"),
            Environment.GetEnvironmentVariable("FTP_USERNAME"),
            Environment.GetEnvironmentVariable("FTP_PASSWORD")
        );

        // Pastas de recursos com imagens
        List<Task> foldersToCreateTasks = new List<Task>() {
            client.CreateDirectory(storageBaseUrl + "/" + FtpManager.authorPicturesFolder),
            client.CreateDirectory(storageBaseUrl + "/" + FtpManager.recordPicturesFolder)
        };

        // Pastas de conteudos
        foreach (ContentTypeData ctd in ContentUtils.contentTypesData) {
            foldersToCreateTasks.Add(
                client.CreateDirectory(
                    storageBaseUrl 
                    + "/" 
                    + ctd.storageFolder
                )
            );
        }

        await Task.WhenAll(foldersToCreateTasks);
    }
}
