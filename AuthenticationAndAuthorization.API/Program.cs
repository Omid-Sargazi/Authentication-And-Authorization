using AuthenticationAndAuthorization.API.AuthDemo;
using AuthenticationAndAuthorization.API.AuthDemo.Application.Services;
using AuthenticationAndAuthorization.API.AuthDemo.Infrastructure.Auth;
using AuthenticationAndAuthorization.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddOpenApi();

// تنظیمات پایگاه داده
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection")));

// سرویس‌های برنامه
builder.Services.AddScoped<IAuthService, AuthService>();



builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HROnly", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("CanViewOrders", policy => policy.RequireClaim("Permission", "ViewOrders"));
    options.AddPolicy("HRAdminOnly", policy => policy.Requirements.Add(new HRAndManageUsersRequirement()));
});


builder.Services.AddSingleton<IAuthorizationHandler, HRAndManageUsersHandler>();

// مجوزها
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// سوگر
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "XCorp API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste your JWT token below:",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// کنترلرها

var app = builder.Build();

// ترتیب میدلورها (مهم!)
app.UseHttpsRedirection();
app.UseAuthentication(); // باید قبل از UseAuthorization و Swagger باشد
app.UseAuthorization();

// Middlewareها
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Path}");
    await next();
});

app.MapGet("/public", () => "this is a public API");

app.MapGet("/profile", (ClaimsPrincipal user) =>
{
    return $"hello:{user.Identity.Name ?? "User"}, wellcome to profile";

}).RequireAuthorization();

app.MapGet("/admin", (ClaimsPrincipal user) =>
{
    return $"you entered as a manager:{user.Identity?.Name}";

}).RequireAuthorization(policy => policy.RequireRole("Admin"));

app.MapGet("/hr-dashboard", () =>
{
    return "welcome HR Member";
}).RequireAuthorization("HROnly");

app.MapGet("/orders/view", () =>
{
    return "Order List:";
}).RequireAuthorization("CanViewOrders");

app.MapGet("/hr-admin-area", () =>
{
    return "Welcome HR Admin";
}).RequireAuthorization("HRAdminOnly");

app.Run();