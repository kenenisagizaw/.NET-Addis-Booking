using AddisBookingAdmin.Data;
using AddisBookingAdmin.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================
// SERVICES
// ======================

// MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        "Host=localhost;Port=5432;Database=capstone_db;Username=postgres;Password=1937205"
    ));

// Custom services
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// ======================
// MIDDLEWARE
// ======================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANT ORDER
app.UseAuthentication(); // 🔐 MUST come before Authorization
app.UseAuthorization();

// ======================
// ROUTING
// ======================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
