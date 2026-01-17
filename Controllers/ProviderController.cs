using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using System.Security.Claims;

[Authorize]
public class ProviderController : Controller
{
    private readonly AppDbContext _context;

    public ProviderController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // APPLY TO BE PROVIDER
    // =========================
    [Authorize(Roles = "Customer")]
    [HttpGet]
    public IActionResult Apply() => View();

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> Apply(
        string fullName,
        string phone,
        string businessName,
        IFormFile nationalId,
        IFormFile businessLicense)
    {
        if (nationalId == null || businessLicense == null)
        {
            ViewBag.Error = "Documents are required";
            return View();
        }

        var uploads = Path.Combine("wwwroot/uploads");
        Directory.CreateDirectory(uploads);

        string SaveFile(IFormFile file)
        {
            var path = Path.Combine(
                uploads,
                Guid.NewGuid() + Path.GetExtension(file.FileName)
            );

            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);
            return path.Replace("wwwroot", "");
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var application = new ProviderApplication
        {
            UserId = userId,
            FullName = fullName,
            Phone = phone,
            BusinessName = businessName,
            NationalIdPath = SaveFile(nationalId),
            BusinessLicensePath = SaveFile(businessLicense),
            Status = ApplicationStatus.Pending
        };

        _context.ProviderApplications.Add(application);
        await _context.SaveChangesAsync();

        TempData["ApplicationSuccess"] = "Application submitted, please wait for Admin Approval.";
        return RedirectToAction("Apply");
    }

    // =========================
    // PROVIDER DASHBOARD
    // =========================
    [Authorize(Roles = "Provider")]
    public IActionResult Dashboard()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var provider = _context.Providers.Single(p => p.UserId == userId);

        return View(provider);
    }
}
