// using Microsoft.OpenApi.Models;

using Database;
using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
//  using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Read configuration
var configuration = builder.Configuration;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
 .ReadFrom.Configuration(configuration)
    .WriteTo.Console()
    .WriteTo.File(
        path: configuration["Logging:File:Path"] ?? "logs/log.txt",
        rollingInterval: Enum.Parse<RollingInterval>(configuration["Logging:File:RollingInterval"] ?? "Day")
    )
    // .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
//builder.Services.AddControllers(options =>
//{
    // options.Filters.Add<ValidationFilter>();
//});

builder.Logging.ClearProviders(); 
builder.Services.AddLogging();
builder.Services.AddAuthorization(); 


builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    // Define Basic Authentication scheme
    options.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Basic Authentication with username and password"
    });

    // Apply Basic Authentication to all API operations
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BasicAuth"
                }
            },
            new string[] { }
        }
    });

    // Configure Swagger document
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "A sample API for testing"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
     app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at the root
        c.DocumentTitle = "API Documentation (Basic Auth Required)";
        c.DisplayRequestDuration(); // Show request duration in Swagger UI
    });
    // app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
// app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseMiddleware<BasicAuthMiddleware>();
// app.UseAuthentication();
app.UseAuthorization();
// app.UseEndpoints(endpoints =>
// {
//     _ = endpoints.MapControllers();
// });
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
