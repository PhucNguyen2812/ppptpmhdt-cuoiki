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

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Cấu hình để cho phép upload file lớn (tối đa 500MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000; // 500MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Cấu hình Kestrel để cho phép request body lớn
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 524288000; // 500MB
});

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
builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();

// Services
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDanhMucService, DanhMucService>();
builder.Services.AddScoped<IKhoaHocService, KhoaHocService>();
builder.Services.AddScoped<IGioHangService, GioHangService>();
builder.Services.AddScoped<IVideoUploadService, VideoUploadService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

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
        ClockSkew = TimeSpan.Zero, // eliminate default clock skew
        RoleClaimType = System.Security.Claims.ClaimTypes.Role // Explicitly set role claim type
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

// seed data initializer (commented out to prevent duplicate data on each run)
// builder.Services.AddTransient<DataInitializer>();

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

// Disable HTTPS redirection for development to avoid connection issues
// Comment this out if you need HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Ensure upload directories exist
var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "uploads");
var videosPath = Path.Combine(uploadsPath, "videos");
var avatarsPath = Path.Combine(uploadsPath, "avatars");
var courseIntroPath = Path.Combine(uploadsPath, "course-intro");

if (!Directory.Exists(videosPath)) Directory.CreateDirectory(videosPath);
if (!Directory.Exists(avatarsPath)) Directory.CreateDirectory(avatarsPath);
if (!Directory.Exists(courseIntroPath)) Directory.CreateDirectory(courseIntroPath);

// Serve static files (for uploaded avatars, images, videos, etc.)
app.UseStaticFiles();

// Custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create data sample (commented out to prevent duplicate data on each run)
// Uncomment the lines below only when you need to seed initial data
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<CourseOnlDbContext>();
//     await DataInitializer.SeedData(dbContext);
// }

// ✅ THIS WAS MISSING - Start the web server!
app.Run();