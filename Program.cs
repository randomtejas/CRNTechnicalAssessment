using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SDTechnicalAssessment.Application.Interfaces;
using SDTechnicalAssessment.Application.Services;
using SDTechnicalAssessment.Application.Validators;
using SDTechnicalAssessment.Domain.Entities;
using SDTechnicalAssessment.Infrastructure.Data;
using SDTechnicalAssessment.Infrastructure.Data.Repositories;
using SDTechnicalAssessment.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Add MVC Controllers.
//
// This allows ASP.NET Core to discover and execute our
// API controllers such as ProductsController.
// ------------------------------------------------------------
builder.Services.AddControllers();



// ------------------------------------------------------------
// Configure Response Compression.
//
// This compresses API responses before sending them to the client.
// It helps reduce response size and improves network performance.
// ------------------------------------------------------------
builder.Services.AddResponseCompression(options =>
{
    // Enable compression even when the API is accessed through HTTPS.
    options.EnableForHttps = true;
});
// ------------------------------------------------------------
// Configure CORS.
//
// CORS allows a browser-based frontend (for example React)
// running on another origin to call our API.
//
// Example:
// React  -> http://localhost:5173
// API    -> https://localhost:7130
//
// Without CORS configuration, the browser may block the request.
// ------------------------------------------------------------
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy
//            // Allow our local React frontend.
//            .WithOrigins(
//                "http://localhost:5179",
//                "https://localhost:5179"
//            )

//            // Allow headers such as Authorization and Content-Type.
//            .AllowAnyHeader()

//            // Allow GET, POST, PUT, DELETE, etc.
//            .AllowAnyMethod();
//    });
//});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:50067")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy
//            .AllowAnyOrigin()
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});



// Configure API versioning.
// This allows us to maintain different versions of our API
// without breaking existing clients.
builder.Services.AddApiVersioning(options =>
{
    // If the client does not specify a version,
    // use version 1.0 by default.
    options.DefaultApiVersion =
        new Asp.Versioning.ApiVersion(1, 0);

    // Assume the default version when no version is specified.
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Add API version information to response headers.
    options.ReportApiVersions = true;
});










// ------------------------------------------------------------
// Configure Entity Framework Core.
//
// This connects our application to SQL Server using the
// connection string stored in appsettings.json.
// ------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));




// ------------------------------------------------------------
// Configure JWT Authentication.
//
// This tells ASP.NET Core that our API will use JWT Bearer
// tokens to authenticate users.
// ------------------------------------------------------------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // ----------------------------------------------------
        // Configure how the incoming JWT token should be
        // validated.
        // ----------------------------------------------------
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate the secret signature of the token.
            ValidateIssuerSigningKey = true,

            // Use the secret key stored in appsettings.json.
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            ),

            // Validate who created the token.
            ValidateIssuer = true,

            // Expected issuer.
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            // Validate who the token is intended for.
            ValidateAudience = true,

            // Expected audience.
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // Validate whether the token has expired.
            ValidateLifetime = true,

            // Small clock difference allowed between systems.
            ClockSkew = TimeSpan.Zero
        };
    });

// ------------------------------------------------------------
// Add Authorization services.
//
// Authorization decides whether an authenticated user
// is allowed to access a protected API endpoint.
// ------------------------------------------------------------
builder.Services.AddAuthorization();







// ------------------------------------------------------------
// Register Product Repository.
//
// This allows ProductService to use IProductRepository
// through Dependency Injection.
// ------------------------------------------------------------
builder.Services.AddScoped<IProductRepository, ProductRepository>();




// ------------------------------------------------------------
// Register User Repository.
//
// AuthService will use this repository to find users
// in the database during login.
// ------------------------------------------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();


// Register the refresh token repository with dependency injection.
builder.Services.AddScoped<
    IRefreshTokenRepository,
    RefreshTokenRepository>();


// ------------------------------------------------------------
// Register Product Service.
//
// The ProductsController will use IProductService
// instead of directly accessing the database.
// ------------------------------------------------------------
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();



// ------------------------------------------------------------
// Register Auth Service.
//
// This allows AuthController to use IAuthService.
// ------------------------------------------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
// ------------------------------------------------------------
// Add API Explorer.
//
// This allows ASP.NET Core to discover our controller
// endpoints so that Swagger can document them.
// ------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();

// ------------------------------------------------------------
// Add Swagger generator.
//
// Swashbuckle uses this to generate the Swagger/OpenAPI
// documentation for our API.
// ------------------------------------------------------------
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    // Define JWT Bearer authentication for Swagger.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",

        // JWT is sent through the HTTP Authorization header.
        Type = SecuritySchemeType.Http,

        // Tell Swagger that we are using Bearer authentication.
        Scheme = "bearer",

        // Tell Swagger that the token format is JWT.
        BearerFormat = "JWT",

        Description = "Enter your JWT token."
    });

    // Apply the Bearer authentication scheme to the API.
    // Swashbuckle 10 uses OpenApiSecuritySchemeReference here.
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// ------------------------------------------------------------
// Register FluentValidation.
// ------------------------------------------------------------
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

// all service registrations...

var app = builder.Build();


// existing middleware...


// ------------------------------------------------------------
// Configure Swagger.
//
// This makes Swagger JSON available and also enables
// the interactive Swagger UI.
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // Generates the Swagger/OpenAPI JSON document.
    app.UseSwagger();

    // Enables the Swagger UI in the browser.
    // Default URL:
    // https://localhost:<port>/swagger
    app.UseSwaggerUI();
}



// ------------------------------------------------------------
// Enable response compression.
//
// This compresses responses such as JSON before they are
// sent back to the client.
// ------------------------------------------------------------
app.UseResponseCompression();





// ------------------------------------------------------------
// Redirect HTTP requests to HTTPS.
// ------------------------------------------------------------
app.UseHttpsRedirection();



// ------------------------------------------------------------
// Enable CORS.
//
// This checks whether the browser request is coming from
// one of the allowed frontend origins.
// ------------------------------------------------------------
app.UseCors("AllowFrontend");



// Global exception handling middleware.
// It catches unhandled exceptions from the API.
app.UseMiddleware<ExceptionHandlingMiddleware>();


// Add security headers to API responses.
app.UseMiddleware<SecurityHeadersMiddleware>();


// ------------------------------------------------------------
// Authentication checks whether the request contains
// a valid JWT token.
// ------------------------------------------------------------
app.UseAuthentication();

// ------------------------------------------------------------
// Authorization checks whether the authenticated user
// has permission to access the requested resource.
// ------------------------------------------------------------
app.UseAuthorization();

// ------------------------------------------------------------
// Map controller endpoints.
// ------------------------------------------------------------
app.MapControllers();
// ------------------------------------------------------------
// Start the application.
// ------------------------------------------------------------
app.Run();