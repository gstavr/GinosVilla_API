using GinosVilla_VillaAPI;
using GinosVilla_VillaAPI.Data;
using GinosVilla_VillaAPI.Logging;
using GinosVilla_VillaAPI.Repository;
using GinosVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Connection String from the appsettings.json file
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});
//Register the Repository
builder.Services.AddScoped<IVillaRepository, VillaRepository>(); // We add the IVillaRepository and the implementation of the interface is the VillaRepository
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Automapper
builder.Services.AddAutoMapper(typeof(MappingConfig)); // We can have all the mappings in the MappingConfig file

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x=> {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
});



//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
//builder.Host.UseSerilog(); // Define to use Serilog not the default logger

builder.Services
    .AddControllers(option =>
    {
        //option.ReturnHttpNotAcceptable = true; // With this line we say what type of return we accept for is json and not XML example application/xml etc etc
    })
    .AddNewtonsoftJson()  // Add NewtonSoftJson for the PATCH API
    .AddXmlDataContractSerializerFormatters(); // Add XML FORMATERS if we disable them as we do in line 8


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = 
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                    "Enter the 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
                    "Example: \"Bearer 1234asdfad\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name =  "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Add Custom Logger to DI
//builder.Services.AddSingleton<ILogging, LoggingV2>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



// Migration Steps
// 1) add-migration AddVillaTable
// 2) update-database
// add-migration SeedVillaTable (but if we run something wrong
// add-migration add-migration SeedVillaTableWithCreatedDate
// and after // 2) update-database