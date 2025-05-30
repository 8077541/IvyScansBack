using IvyScans.API.Data;
using IvyScans.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure for Railway deployment
if (builder.Environment.IsProduction())
{
    // Railway provides the PORT environment variable
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    // Override connection string with Railway's DATABASE_URL if available
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        builder.Configuration["ConnectionStrings:DefaultConnection"] = databaseUrl;
    }

    // Override JWT settings with environment variables if available
    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
    if (!string.IsNullOrEmpty(jwtSecret))
    {
        builder.Configuration["Jwt:SecretKey"] = jwtSecret;
    }
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ivy Scans API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider");

if (databaseProvider == "PostgreSQL")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register Services
builder.Services.AddScoped<IComicService, ComicService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is required");
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is required");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is required");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Configure CORS
var configuredOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
var envAllowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");

// Log the environment variable for debugging
Console.WriteLine($"Environment ALLOWED_ORIGINS: {envAllowedOrigins}");
Console.WriteLine($"Configured origins from appsettings: {string.Join(", ", configuredOrigins ?? new string[0])}");

var allowedOrigins = !string.IsNullOrEmpty(envAllowedOrigins)
    ? envAllowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray()
    : configuredOrigins ?? new[] { "http://localhost:3000", "https://is.dominikjaniak.com" };

Console.WriteLine($"Final allowed origins: {string.Join(", ", allowedOrigins)}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp", corsBuilder =>
    {
        corsBuilder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });

    // Add a more permissive policy for development/testing
    options.AddPolicy("AllowAll", corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    // Add a policy specifically for production debugging
    options.AddPolicy("ProductionDebug", corsBuilder =>
    {
        corsBuilder.SetIsOriginAllowed(origin =>
        {
            Console.WriteLine($"CORS origin check: {origin}");
            return true; // Allow all origins temporarily for debugging
        })
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Configure forwarded headers for Railway deployment
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Add logging for CORS debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    
    // Log all request details
    logger.LogInformation($"=== REQUEST ===");
    logger.LogInformation($"Method: {context.Request.Method}");
    logger.LogInformation($"Path: {context.Request.Path}");
    logger.LogInformation($"Origin: {context.Request.Headers.Origin}");
    logger.LogInformation($"Host: {context.Request.Headers.Host}");
    logger.LogInformation($"User-Agent: {context.Request.Headers.UserAgent}");
    
    // Log all headers
    foreach (var header in context.Request.Headers)
    {
        logger.LogInformation($"Header: {header.Key} = {header.Value}");
    }
    
    await next();
    
    // Log response headers
    logger.LogInformation($"=== RESPONSE ===");
    logger.LogInformation($"Status: {context.Response.StatusCode}");
    foreach (var header in context.Response.Headers)
    {
        logger.LogInformation($"Response Header: {header.Key} = {header.Value}");
    }
});

// Auto-migrate database on startup in production
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Enable Swagger in production for portfolio purposes
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ivy Scans API v1");
        c.RoutePrefix = "swagger";
    });
}

// Railway provides HTTPS termination, but we'll handle both
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
}

app.UseHttpsRedirection();

// Manual CORS middleware for debugging
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers.Origin.ToString();
    
    // Always add CORS headers using indexer to avoid duplicates
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
    context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Requested-With";
    context.Response.Headers["Access-Control-Max-Age"] = "86400";
    
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Manual CORS headers added for origin: {origin}");
    
    // Handle preflight requests
    if (context.Request.Method == "OPTIONS")
    {
        logger.LogInformation("Handling OPTIONS preflight request");
        context.Response.StatusCode = 200;
        return;
    }
    
    await next();
});

// Apply CORS policy - ensure this is before UseAuthentication and UseAuthorization
// Force use of most permissive policy for debugging
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

Console.WriteLine("CORS Policy Applied: Allow any origin, method, and header");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();