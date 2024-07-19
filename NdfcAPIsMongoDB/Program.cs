using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Common;
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
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Reflection;
using FluentValidation.AspNetCore;
using FluentValidation;
using NdfcAPIsMongoDB.Validators;
using NdfcAPIsMongoDB.Models.DTO;
using NdfcAPIsMongoDB.Repository.HistoryService;
using NdfcAPIsMongoDB.GraphQL;
using Hangfire;
using Hangfire.MemoryStorage;
using NdfcAPIsMongoDB.Common.EmailService;
using NdfcAPIsMongoDB.Middleware;
using NdfcAPIsMongoDB.Common.ElasticSearch;
using NdfcAPIsMongoDB.Repository.SubscribService;
using NdfcAPIsMongoDB.Repository.TiketService;
using NdfcAPIsMongoDB.Common.PagingComon;
using Serilog.Sinks.Elasticsearch;
using NdfcAPIsMongoDB.Repository.LogService;

var builder = WebApplication.CreateBuilder(args);
// chuỗi kn mongoDB
var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
var mongoClient = new MongoClient(connectionString);
var databaseName = builder.Configuration.GetValue<string>("DatabaseSettings:DatabaseName");
var database = mongoClient.GetDatabase(databaseName);
// log vào file cứng
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error) 
    .Enrich.FromLogContext()
    .WriteTo.File(new JsonFormatter(), "logs/log.txt", rollingInterval: RollingInterval.Day)
   // .WriteTo.Sink(new MongoDBSink(database, "LogEntries"))
    .CreateLogger();

// log vào elk stack
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "logstash-{0:yyyy.MM.dd}"
            })
            .CreateLogger();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
builder.Services.AddControllers()
        .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Logging.AddSerilog();
builder.Services.AddLogging();
// cấu hình lấy full patch http or https cho các file
builder.Services.AddHttpContextAccessor();
// thêm cấu hình để có thể truyền vào token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NDFC APIs",
        Version = "v1",
        Description = "Nam Dinh Football Club Management APIs",
    });
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
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
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
builder.Services.AddHangfire(configuration => configuration.UseMemoryStorage());
builder.Services.AddSignalR();
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

builder.Services.AddScoped<Query>();
builder.Services.AddScoped<Mutation>();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

// khai báo mã syncfusion phục vụ nhập/xuất file-extend
SyncfusionLicenseProvider.RegisterLicense("MTQwNUAzMTM4MmUzNDJlMzBGT29sdENza2kyME1jUHpPNVd5enVXY1AvNVZ1SVdPQlVMNUE4R1c1M0FvPQ==");
builder.Services.AddSingleton<LogService>();
// phân trang
builder.Services.AddScoped<IPagingComon, PagingCommon>();
// khai báo để sử dụng DI
builder.Services.AddSingleton(database);
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<ISliderRepository, SliderRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<IContact,ContactRepository>();
builder.Services.AddScoped<IHistoryRepositorycs, HistoryRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();
//validate
builder.Services.AddTransient<IValidator<LeagueDTO>, LeagueValidator>();
builder.Services.AddTransient<IValidator<PlayerDto>, PlayerValidator>();
builder.Services.AddTransient<IValidator<ContactDTO>, ContactDTOValidator>();
builder.Services.AddTransient<IValidator<NewsDTO>, NewsDTOValidator>();

//elastic Search
builder.Services.Configure<ElasticsearchSettings>(builder.Configuration.GetSection("ElasticsearchSettings"));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

var app = builder.Build();
// ngăn chăn truy cập APIs từ một vài nguồn không uy tín
app.UseMiddleware<DomainRestrictionMiddleware>();
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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub");
});
app.UseHangfireServer();
app.UseHangfireDashboard("/hangfile");
app.MapGraphQL("/graphql");
app.Run();
