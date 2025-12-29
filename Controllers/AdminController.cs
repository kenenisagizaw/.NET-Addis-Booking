using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // DASHBOARD
    // =========================
    public IActionResult Dashboard()
    {
        var model = new DashboardViewModel
        {
            TotalUsers = _context.Users.Count(),
            TotalProviders = _context.Providers.Count(),
            TotalServices = _context.Services.Count(),
            TotalPendingProviders = _context.ProviderApplications
                .Count(a => a.Status == ApplicationStatus.Pending)
        };

        return View(model);
    }

    // =========================
    // PROVIDER APPLICATIONS
    // =========================
    public IActionResult Applications()
    {
        var apps = _context.ProviderApplications
            .Where(a => a.Status == ApplicationStatus.Pending)
            .Include(a => a.User)
            .ToList();

        return View(apps);
    }

    // APPROVE PROVIDER
    public async Task<IActionResult> Approve(int id)
    {
        var app = _context.ProviderApplications
            .Include(a => a.User)
            .Single(a => a.Id == id);

        app.Status = ApplicationStatus.Approved;

        _context.Providers.Add(new Provider
        {
            UserId = app.UserId,
            BusinessName = app.BusinessName,
            Phone = app.Phone,
            NationalIdPath = app.NationalIdPath,
            BusinessLicensePath = app.BusinessLicensePath,
            IsApproved = true
        });

        app.User.Role = UserRole.Provider;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Applications));
    }

    // REJECT PROVIDER
    public async Task<IActionResult> Reject(int id)
    {
        var app = _context.ProviderApplications.Single(a => a.Id == id);
        app.Status = ApplicationStatus.Rejected;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Applications));
    }

    // =========================
    // PROVIDERS MANAGEMENT
    // =========================
    public IActionResult Providers()
    {
        var providers = _context.Providers
            .Include(p => p.User)
            .Include(p => p.Services)
            .ToList();

        return View(providers);
    }


    // =========================
    // USERS MANAGEMENT
    // =========================
    public IActionResult Users()
    {
        var users = _context.Users
            .Include(u => u.Provider) // ✅ FIXED
            .ToList();

        return View(users);
    }
}
