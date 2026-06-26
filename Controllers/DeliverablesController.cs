using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProgrammeTracker.Data;
using ProgrammeTracker.Models;
using ProgrammeTracker.ViewModels;

namespace ProgrammeTracker.Controllers
{
    [Authorize]
    public class DeliverablesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliverablesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── GET: Deliverables ──
        public async Task<IActionResult> Index(int? programmeId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdminOrManager = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.AccountManager)
                || User.IsInRole(Roles.ProjectCoordinator) || User.IsInRole(Roles.Management);

            var query = _db.Deliverables
                .Include(d => d.Programme)
                .Include(d => d.AssignedTo)
                .AsQueryable();

            if (programmeId.HasValue)
                query = query.Where(d => d.ProgrammeId == programmeId.Value);

            // Non-admin users only see their own deliverables
            if (!isAdminOrManager)
                query = query.Where(d => d.AssignedToId == currentUser!.Id);

            var deliverables = await query.OrderBy(d => d.DueDate).ToListAsync();

            ViewBag.ProgrammeId = programmeId;
            return View(deliverables);
        }

        // ── GET: Deliverables/Create ──
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Create(int? programmeId)
        {
            var vm = new DeliverableViewModel { ProgrammeId = programmeId ?? 0 };

            if (programmeId.HasValue)
            {
                var p = await _db.Programmes.FindAsync(programmeId.Value);
                vm.ProgrammeName = p?.ProgrammeName;
                vm.ClientName = p?.ClientName;
            }

            await PopulateDropdowns(vm);
            return View(vm);
        }

        // ── POST: Deliverables/Create ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Create(DeliverableViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm);
                return View(vm);
            }

            var d = new Deliverable
            {
                ProgrammeId = vm.ProgrammeId,
                Title = vm.Title,
                Description = vm.Description,
                DueDate = vm.DueDate,
                AssignedToId = vm.AssignedToId,
                Status = vm.Status,
                NotifyDaysBefore = vm.NotifyDaysBefore
            };

            _db.Deliverables.Add(d);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Deliverable created.";
            return RedirectToAction(nameof(Index), new { programmeId = vm.ProgrammeId });
        }

        // ── GET: Deliverables/Edit/5 ──
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Edit(int id)
        {
            var d = await _db.Deliverables
                .Include(d => d.Programme)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (d == null) return NotFound();

            var vm = new DeliverableViewModel
            {
                Id = d.Id, ProgrammeId = d.ProgrammeId,
                ProgrammeName = d.Programme?.ProgrammeName,
                ClientName = d.Programme?.ClientName,
                Title = d.Title, Description = d.Description,
                DueDate = d.DueDate, AssignedToId = d.AssignedToId,
                Status = d.Status, NotifyDaysBefore = d.NotifyDaysBefore
            };

            await PopulateDropdowns(vm);
            return View(vm);
        }

        // ── POST: Deliverables/Edit/5 ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Edit(DeliverableViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm);
                return View(vm);
            }

            var d = await _db.Deliverables.FindAsync(vm.Id);
            if (d == null) return NotFound();

            d.Title = vm.Title; d.Description = vm.Description;
            d.DueDate = vm.DueDate; d.AssignedToId = vm.AssignedToId;
            d.Status = vm.Status; d.NotifyDaysBefore = vm.NotifyDaysBefore;
            d.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Deliverable updated.";
            return RedirectToAction(nameof(Index), new { programmeId = d.ProgrammeId });
        }

        // ── POST: Quick status update ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var d = await _db.Deliverables.FindAsync(id);
            if (d == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdminOrCoord = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.AccountManager)
                || User.IsInRole(Roles.ProjectCoordinator);

            if (!isAdminOrCoord && d.AssignedToId != currentUser?.Id)
                return Forbid();

            d.Status = status;
            d.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { programmeId = d.ProgrammeId });
        }

        // ── GET: Deliverables/Delete/5 ──
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Deliverables
                .Include(d => d.Programme)
                .Include(d => d.AssignedTo)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (d == null) return NotFound();
            return View(d);
        }

        // ── POST: Deliverables/Delete/5 ──
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var d = await _db.Deliverables.FindAsync(id);
            int? progId = d?.ProgrammeId;
            if (d != null) { _db.Deliverables.Remove(d); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Deliverable deleted.";
            return RedirectToAction(nameof(Index), new { programmeId = progId });
        }

        private async Task PopulateDropdowns(DeliverableViewModel vm)
        {
            var users = await _db.Users.Where(u => u.IsActive)
                .Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName + " (" + u.Role + ")" })
                .ToListAsync();
            vm.Users = new SelectList(users, "Id", "Name", vm.AssignedToId);
            vm.Statuses = new SelectList(DeliverableStatuses.All);
        }
    }
}
