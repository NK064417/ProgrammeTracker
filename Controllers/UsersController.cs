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
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        // ── GET: Users ──
        public async Task<IActionResult> Index()
        {
            var users = await _db.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id, FirstName = u.FirstName, LastName = u.LastName,
                    Email = u.Email ?? "", Role = u.Role, IsActive = u.IsActive,
                    AssignedDeliverableCount = u.AssignedDeliverables.Count(d => d.Status != "Complete")
                }).ToListAsync();
            return View(users);
        }

        // ── GET: Users/Create ──
        public IActionResult Create()
        {
            var vm = new UserViewModel { Roles = new SelectList(Roles.All) };
            return View(vm);
        }

        // ── POST: Users/Create ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Password))
                ModelState.AddModelError("Password", "Password is required for new users");

            if (!ModelState.IsValid)
            {
                vm.Roles = new SelectList(Roles.All);
                return View(vm);
            }

            var user = new ApplicationUser
            {
                FirstName = vm.FirstName, LastName = vm.LastName,
                Email = vm.Email, UserName = vm.Email,
                Role = vm.Role, IsActive = vm.IsActive
            };

            var result = await _userManager.CreateAsync(user, vm.Password!);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                vm.Roles = new SelectList(Roles.All);
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, vm.Role);
            TempData["Success"] = $"User {user.FullName} created.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET: Users/Edit/5 ──
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var vm = new UserViewModel
            {
                Id = user.Id, FirstName = user.FirstName, LastName = user.LastName,
                Email = user.Email ?? "", Role = user.Role, IsActive = user.IsActive,
                Roles = new SelectList(Roles.All)
            };
            return View(vm);
        }

        // ── POST: Users/Edit/5 ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel vm)
        {
            // Password optional on edit
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                vm.Roles = new SelectList(Roles.All);
                return View(vm);
            }

            var user = await _userManager.FindByIdAsync(vm.Id.ToString());
            if (user == null) return NotFound();

            // Check email uniqueness
            if (!string.Equals(user.Email, vm.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userManager.FindByEmailAsync(vm.Email);
                if (existing != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    vm.Roles = new SelectList(Roles.All);
                    return View(vm);
                }
            }

            user.FirstName = vm.FirstName; user.LastName = vm.LastName;
            user.Email = vm.Email; user.UserName = vm.Email;
            user.IsActive = vm.IsActive;

            if (user.Role != vm.Role)
            {
                await _userManager.RemoveFromRoleAsync(user, user.Role);
                user.Role = vm.Role;
                await _userManager.AddToRoleAsync(user, vm.Role);
            }

            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, vm.Password);
            }

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "User updated.";
            return RedirectToAction(nameof(Index));
        }

        // ── POST: Toggle active status ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
