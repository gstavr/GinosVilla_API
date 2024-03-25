using Asp.Versioning;
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
using Microsoft.AspNetCore.Mvc;
using GinosVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using GinosVilla_VillaAPI.Filters;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Diagnostics;
using GinosVilla_VillaAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Connection String from the appsettings.json file
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});
// Add Identity 
// builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>(); This is the default
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>(); // This is our extend user
//Add Caching
builder.Services.AddResponseCaching();

//Register the Repository
builder.Services.AddScoped<IVillaRepository, VillaRepository>(); // We add the IVillaRepository and the implementation of the interface is the VillaRepository
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Automapper
builder.Services.AddAutoMapper(typeof(MappingConfig)); // We can have all the mappings in the MappingConfig file

// Add Versioning
builder.Services.AddApiVersioning( options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);    
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    //Tell swagger that we have versions
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});




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
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://magicvilla-api.com",
            ValidAudience = "https://test-magic-api.com",
            ClockSkew = TimeSpan.Zero,
        };
});

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
//builder.Host.UseSerilog(); // Define to use Serilog not the default logger

builder.Services.AddControllers(option =>
    {
        //option.CacheProfiles.Add("Default30", new CacheProfile() { Duration = 30}); // Add A Cache Profile in the programm
        //option.ReturnHttpNotAcceptable = true; // With this line we say what type of return we accept for is json and not XML example application/xml etc etc

        // Add filter
        option.Filters.Add<CustomExceptionFilter>();
    })
    .AddNewtonsoftJson()  // Add NewtonSoftJson for the PATCH API
    .AddXmlDataContractSerializerFormatters() // Add XML FORMATERS if we disable them as we do in line 8
    .ConfigureApiBehaviorOptions(options =>
    {
        options.ClientErrorMapping[StatusCodes.Status500InternalServerError] = new ClientErrorData
        {
            Link = "https://ginos.com/500" // with this you customazie the status codes error with yours that is custom for your server
        };
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Move Swagger Options to another file and add the options there
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

// Add Custom Logger to DI
//builder.Services.AddSingleton<ILogging, LoggingV2>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{  
    app.UseSwaggerUI(options =>
    {   
        // Add Version 2 Document
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Ginos_VillaV2");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ginos_VillaV1");
        //options.RoutePrefix = "";
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Add Version 2 Document
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Ginos_VillaV2");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ginos_VillaV1");
        options.RoutePrefix = "";
    });
}



// app.UseExceptionHandler("/ErrorHandling/ProcessError");

app.HandlerError(app.Environment.IsDevelopment()); // Move the logic in a file and extend the app.


app.UseStaticFiles(); // Make wwwroot accessable

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration(); // In order to apply migration Automatically create a function that does this not not by hand
app.Run();



// Migration Steps
// 1) add-migration AddVillaTable
// 2) update-database
// add-migration SeedVillaTable (but if we run something wrong
// add-migration add-migration SeedVillaTableWithCreatedDate
// and after // 2) update-database

void ApplyMigration()
{
    using(var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
        }
    }
}