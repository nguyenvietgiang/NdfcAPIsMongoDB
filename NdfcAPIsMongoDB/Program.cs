using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using NdfcAPIsMongoDB.FileService;
using NdfcAPIsMongoDB.Repository.ContactService;
using NdfcAPIsMongoDB.Repository.LeagueService;
using NdfcAPIsMongoDB.Repository.MatchService;
using NdfcAPIsMongoDB.Repository.NewsService;
using NdfcAPIsMongoDB.Repository.PlayerService;
using NdfcAPIsMongoDB.Repository.ReportService;
using NdfcAPIsMongoDB.Repository.SliderService;
using NdfcAPIsMongoDB.Repository.VideoService;
using Syncfusion.Licensing;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// cấu hình lấy full patch http or https cho các file
builder.Services.AddHttpContextAccessor();
// thêm cấu hình để có thể truyền vào token
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
// cấu hình CROS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                   policy =>
                   {
                       policy.AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader();
                   });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Cấu hình thông tin xác thực JWT ở đây
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "my-ndfc",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my-secret-key-123"))
        };
    });
// khai báo mã syncfusion phục vụ nhập/xuất file-extend
SyncfusionLicenseProvider.RegisterLicense("MTQwNUAzMTM4MmUzNDJlMzBGT29sdENza2kyME1jUHpPNVd5enVXY1AvNVZ1SVdPQlVMNUE4R1c1M0FvPQ==");
// khai báo để sử dụng DI
var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
var mongoClient = new MongoClient(connectionString);
var databaseName = builder.Configuration.GetValue<string>("DatabaseSettings:DatabaseName");
var database = mongoClient.GetDatabase(databaseName);
builder.Services.AddSingleton(database);
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ISliderRepository, SliderRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<IContact,ContactRepository>();
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

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
