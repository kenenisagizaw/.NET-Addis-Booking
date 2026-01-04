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
// SERVICES MANAGEMENT (ADMIN)
// =========================
public IActionResult Services()
{
    var services = _context.Services
        .Include(s => s.Provider)
        .ThenInclude(p => p.User)
        .ToList();

    return View(services);
}

    // =========================
    // USERS MANAGEMENT
    // =========================
    public IActionResult Users()
    {
        var users = _context.Users
            .Include(u => u.Provider) // 
            .ToList();

        return View(users);
    }
    // =========================
    // DELETE USER
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Provider)
                .ThenInclude(p => p.Services)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        _context.Users.Remove(user); // cascades provider & services
        await _context.SaveChangesAsync();

        TempData["Success"] = "User deleted successfully.";
        return RedirectToAction("Users");
    }

    // =========================
    // DELETE PROVIDER
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProvider(int id)
    {
        var provider = await _context.Providers
            .Include(p => p.Services)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (provider == null) return NotFound();

        _context.Providers.Remove(provider);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Provider deleted successfully.";
        return RedirectToAction("Providers");
    }

    // =========================
    // DELETE SERVICE
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null) return NotFound();

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Service deleted successfully.";
        return RedirectToAction("Services");
    }
}

