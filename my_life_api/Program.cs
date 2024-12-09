using my_life_api.Resources;
using my_life_api.Configurations;
using my_life_api.Middlewares;
using my_life_api.Database;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

string dataBaseUrl = Environment.GetEnvironmentVariable("DATA_BASE_URL");
DataBase.ConfigureDataBase(dataBaseUrl);

FtpManager.ConfigureFtpServer(
    Environment.GetEnvironmentVariable("FTP_SERVER"),
    Environment.GetEnvironmentVariable("FTP_USERNAME"),
    Environment.GetEnvironmentVariable("FTP_PASSWORD"),
    Environment.GetEnvironmentVariable("STORAGE_BASE_URL")
);

CustomApiConfigs customApiConfigs = new CustomApiConfigs();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        customApiConfigs.OverrideInvalidModels(options)
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

customApiConfigs.AddValidationScopes(builder.Services);

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestBodyBufferingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
