using my_life_api.Database.Managers;

namespace my_life_api.Services;

public class SetupService {
    public async Task Setup() {
        SetupDBManager setupManager = new SetupDBManager();

        // Monta todo o banco de dados do zero e insere os respectivos dados iniciais
        await setupManager.CreateTables();
        await setupManager.InsertInitialData();

        // Cria toda a estrutura de pastas do storage FTP
        await setupManager.CreateFtpFolders();
    }
}
