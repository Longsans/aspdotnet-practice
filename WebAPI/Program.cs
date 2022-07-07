using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Practice.Services;
using Common.Validators;
using Practice.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddScoped<IAuthenticationService, CookieAuthenticationService>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCredentialsValidator>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
    options =>
    {
        options.Cookie.Name = "Authentication";
    }
);

//builder.Services.AddAuthorization(
//    options =>
//    {
//        options.FallbackPolicy =
//            new AuthorizationPolicyBuilder()
//                .RequireAuthenticatedUser()
//                .Build();
//    }
//);


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
