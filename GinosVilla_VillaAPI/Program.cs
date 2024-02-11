using GinosVilla_VillaAPI.Data;
using GinosVilla_VillaAPI.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Connection String from the appsettings.json file
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
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
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();



// Migration Steps
// 1) add-migration AddVillaTable
// 2) update-database
// add-migration SeedVillaTable (but if we run something wrong
// add-migration add-migration SeedVillaTableWithCreatedDate
// and after // 2) update-database