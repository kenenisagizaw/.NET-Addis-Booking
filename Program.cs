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
{
    // Direct connection string for local development
    var cs = "Host=localhost;Port=5432;Database=capstone_db;Username=postgres;Password=1937205";
    options.UseNpgsql(cs);
});


// Custom services
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtService>();

// ----------------------
// SESSION CONFIGURATION ✅
// ----------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ----------------------
// JWT Authentication
// ----------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Read JWT from cookie
                context.Token = context.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
        };

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        if (db.Database.CanConnect())
        {
            Console.WriteLine("✅ PostgreSQL connection successful");
        }
        else
        {
            Console.WriteLine("❌ PostgreSQL connection failed");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ DB connection error:");
        Console.WriteLine(ex.Message);
    }
}

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

// ----------------------
// IMPORTANT ORDER ✅
// ----------------------
app.UseSession();          // ✅ MUST come BEFORE auth

app.UseAuthentication();   // 🔐
app.UseAuthorization();

// ======================
// ROUTING
// ======================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
