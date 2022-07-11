using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using FluentValidation;
using Common.Services;
using Common.Validators;
using Practice.Services;
using Practice.Data;
using WebAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddCors(
    options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    }
);
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(
    options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WebAppContext>(
    options =>
    {
        var connString =
            new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .GetSection("ConnectionStrings")
            ["DefaultConnection"];

        options.UseSqlite(
            connString
        );
    }
);
builder.Services.AddScoped<IUserService, DefaultUserService>();
builder.Services.AddScoped<ITokenBasedAuthService<string>, JwtAuthService>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCredentialsValidator>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSecret"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
