using khoahoconline.Data;
using khoahoconline.Data.Repositories;
using khoahoconline.Data.Repositories.Impl;
using khoahoconline.Mappings;
using khoahoconline.Middleware.Exceptions;
using khoahoconline.Services;
using khoahoconline.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 2️⃣ Add services to the container
// ============================================

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// DbContext and Repositories
builder.Services.AddDbContext<CourseOnlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();

// Services
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// authentication & authorization
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // validate the token expiration
        ValidateIssuerSigningKey = true, // validate the signing key
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey), // symmetricSecurityKey use the HMACSHA256 algorithm
        ClockSkew = TimeSpan.Zero // eliminate default clock skew
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "https://127.0.0.1:5500",
                "https://localhost:5500",
                "http://127.0.0.1:5500",
                "http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// seed data initializer
builder.Services.AddTransient<DataInitializer>();

var app = builder.Build();

app.UseCors("AllowFrontend");

// ============================================
// 3️⃣ Configure HTTP request pipeline
// ============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Quản lý bán thuốc API v1");
    });
}

app.UseHttpsRedirection();

// Custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create data sample
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CourseOnlDbContext>();
    await DataInitializer.SeedData(dbContext);
}

// ✅ THIS WAS MISSING - Start the web server!
app.Run();