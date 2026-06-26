using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammeTracker.Data;
using ProgrammeTracker.Models;
using ProgrammeTracker.ViewModels;

namespace ProgrammeTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search, string? status, string? type)
        {
            var query = _db.Programmes
                .Include(p => p.ProgrammeType)
                .Include(p => p.AccountManager)
                .Include(p => p.Coordinator)
                .Include(p => p.Facilitator)
                .Include(p => p.Deliverables)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.ClientName.Contains(search) || p.ProgrammeName.Contains(search));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(p => p.DeliveryType == type);

            var programmes = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            var allDeliverables = await _db.Deliverables
                .Where(d => d.Status != "Complete")
                .ToListAsync();

            var vm = new DashboardViewModel
            {
                Programmes = programmes,
                TotalProgrammes = programmes.Count,
                ActiveProgrammes = programmes.Count(p => p.Status == "Active"),
                OverdueDeliverables = allDeliverables.Count(d => d.IsOverdue),
                DueSoonDeliverables = allDeliverables.Count(d => d.IsDueSoon),
                SearchTerm = search,
                FilterStatus = status,
                FilterType = type
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
