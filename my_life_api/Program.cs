using my_life_api.Resources;
using my_life_api.Validators;
using my_life_api.Configurations;
using my_life_api.Middlewares;
using my_life_api.Database;
using my_life_api.Validators.Author;
using my_life_api.Validators.Security;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

string dataBaseUrl = Environment.GetEnvironmentVariable("DATA_BASE_URL");
DataBase.ConfigureDataBase(dataBaseUrl);

string ftpServer = Environment.GetEnvironmentVariable("FTP_SERVER");
string ftpUsername = Environment.GetEnvironmentVariable("FTP_USERNAME");
string ftpPassword = Environment.GetEnvironmentVariable("FTP_PASSWORD");
string storageBaseUrl = Environment.GetEnvironmentVariable("STORAGE_BASE_URL");
FtpManager.ConfigureFtpServer(ftpServer, ftpUsername, ftpPassword, storageBaseUrl);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        new CustomApiConfigs().OverrideInvalidModels(options)
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TokenValidationFilter>();
builder.Services.AddScoped<LoginValidationFilter>();
builder.Services.AddScoped<CreateMovieValidationFilter>();
builder.Services.AddScoped<UpdateMovieValidationFilter>();
builder.Services.AddScoped<DeleteMovieValidationFilter>();
builder.Services.AddScoped<CreateAuthorValidationFilter>();
builder.Services.AddScoped<UpdateAuthorValidationFilter>();
builder.Services.AddScoped<DeleteAuthorValidationFilter>();
builder.Services.AddScoped<DeleteAuthorImgValidationFilter>();
builder.Services.AddScoped<CreateCategoryValidationFilter>();
builder.Services.AddScoped<UpdateCategoryValidationFilter>();
builder.Services.AddScoped<DeleteCategoryValidationFilter>();
builder.Services.AddScoped<ContentTypeParamValidationFilter>();
builder.Services.AddScoped<ResourceFiltersParamValidationFilter>();
builder.Services.AddScoped<DeleteResourceImgValidationFilter>();

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
