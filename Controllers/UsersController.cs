using Microsoft.AspNetCore.Mvc;
using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using System.Linq;

namespace AddisBookingAdmin.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

            // GET: /Users/ProviderServices/{providerId}
            public IActionResult ProviderServices(int providerId)
            {
                var provider = _context.Providers.FirstOrDefault(p => p.Id == providerId);
                if (provider == null) return NotFound();
                var services = _context.Services.Where(s => s.ProviderId == providerId).ToList();
                ViewBag.ProviderName = provider.FullName;
                return View(services);
            }

            // GET: /Users/Providers
            public IActionResult Providers()
            {
                var providers = _context.Providers.ToList();
                return View(providers);
            }

        // GET: /Users
        public IActionResult Index()
        {
            var users = _context.Users.Where(u => u.Role != UserRole.Admin).ToList();
            return View(users);
        }

        // GET: /Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Delete/5
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
