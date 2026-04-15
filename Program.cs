using Epros_CareerHubAPI.Data;
using Epros_CareerHubAPI.Helpers;
using Epros_CareerHubAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Use specific origin for development
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<EprosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBCon")));

// Register helpers and repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJobPostingRepository, JobPostingRepository>();
builder.Services.AddSingleton<IPasswordHelper, PasswordHelper>();
builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

// Configure JWT authentication (preserve previous configuration)
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Missing Jwt:Key in configuration.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
        ValidIssuer = jwtIssuer,
        ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateLifetime = true
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure wwwroot/resumes folder exists (create if missing)
var wwwrootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
var resumesPath = Path.Combine(wwwrootPath, "resumes");
Directory.CreateDirectory(resumesPath);

// Global exception handling:
// - In Development use developer exception page for detailed diagnostics.
// - In non-Development use custom middleware to return ProblemDetails.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");

// Serve static files from wwwroot (so files under /resumes/... are downloadable)       
app.UseStaticFiles();

// Authentication must run before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();