using my_life_api.Resources;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

string dataBaseUrl = Environment.GetEnvironmentVariable("DATA_BASE_URL");
await DataBase.ConnectToDataBase(dataBaseUrl);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
