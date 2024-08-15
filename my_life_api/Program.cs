using my_life_api.Resources;
using my_life_api.Filters;
using my_life_api.Validators;
using my_life_api.Configurations;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

string dataBaseUrl = Environment.GetEnvironmentVariable("DATA_BASE_URL");
await DataBase.ConnectToDataBase(dataBaseUrl);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        CustomApiConfigurations.OverrideInvalidModels(options)
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TokenValidationFilter>();
builder.Services.AddScoped<LoginValidationFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RequestBodyBufferingMiddleware>();

app.MapControllers();

app.Run();
