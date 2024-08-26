using my_life_api.Resources;
using my_life_api.Validators;
using my_life_api.Configurations;
using my_life_api.Middlewares;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

string dataBaseUrl = Environment.GetEnvironmentVariable("DATA_BASE_URL");
await DataBase.ConnectToDataBase(dataBaseUrl);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        new CustomApiConfigs().OverrideInvalidModels(options)
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TokenValidationFilter>();
builder.Services.AddScoped<LoginValidationFilter>();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestBodyBufferingMiddleware>();

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
