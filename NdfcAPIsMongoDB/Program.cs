using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// khai báo để sử dụng DI
var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
var mongoClient = new MongoClient(connectionString);
var databaseName = builder.Configuration.GetValue<string>("DatabaseSettings:DatabaseName");
var database = mongoClient.GetDatabase(databaseName);
builder.Services.AddSingleton(database);
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// cấu hình để truy cập ảnh từ upload folder từ URL
app.UseStaticFiles();// For the wwwroot folder

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});

app.UseAuthorization();

app.MapControllers();

app.Run();
