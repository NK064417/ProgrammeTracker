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
    public class ProgrammesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProgrammesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── GET: Programmes ──
        public async Task<IActionResult> Index()
        {
            var programmes = await _db.Programmes
                .Include(p => p.ProgrammeType)
                .Include(p => p.AccountManager)
                .Include(p => p.Coordinator)
                .Include(p => p.Facilitator)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(programmes);
        }

        // ── GET: Programmes/Details/5 ──
        public async Task<IActionResult> Details(int id)
        {
            var p = await _db.Programmes
                .Include(p => p.ProgrammeType)
                .Include(p => p.AccountManager)
                .Include(p => p.Coordinator)
                .Include(p => p.Facilitator)
                .Include(p => p.TalentManager)
                .Include(p => p.Payroll)
                .Include(p => p.CreatedByUser)
                .Include(p => p.Deliverables).ThenInclude(d => d.AssignedTo)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return NotFound();
            return View(p);
        }

        // ── GET: Programmes/Phase1/Create ──
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Phase1(int? id)
        {
            ProgrammePhase1ViewModel vm;

            if (id.HasValue)
            {
                var p = await _db.Programmes.FindAsync(id.Value);
                if (p == null) return NotFound();
                vm = MapToPhase1ViewModel(p);
            }
            else
            {
                vm = new ProgrammePhase1ViewModel();
            }

            await PopulatePhase1Dropdowns(vm);
            return View(vm);
        }

        // ── POST: Programmes/Phase1 ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Phase1(ProgrammePhase1ViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePhase1Dropdowns(vm);
                return View(vm);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (vm.Id == 0)
            {
                var p = new Programme { CreatedByUserId = currentUser!.Id };
                MapFromPhase1ViewModel(vm, p);
                p.ComputePhaseCompletion();
                _db.Programmes.Add(p);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Phase 1 saved. You can now complete Phase 2.";
                return RedirectToAction(nameof(Phase2), new { id = p.Id });
            }
            else
            {
                var p = await _db.Programmes.FindAsync(vm.Id);
                if (p == null) return NotFound();
                MapFromPhase1ViewModel(vm, p);
                p.UpdatedAt = DateTime.UtcNow;
                p.ComputePhaseCompletion();
                await _db.SaveChangesAsync();
                TempData["Success"] = "Phase 1 updated.";
                return RedirectToAction(nameof(Details), new { id = p.Id });
            }
        }

        // ── GET: Programmes/Phase2/5 ──
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Phase2(int id)
        {
            var p = await _db.Programmes.FindAsync(id);
            if (p == null) return NotFound();

            var vm = MapToPhase2ViewModel(p);
            await PopulatePhase2Dropdowns(vm);
            return View(vm);
        }

        // ── POST: Programmes/Phase2 ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Account Manager,Project Coordinator")]
        public async Task<IActionResult> Phase2(ProgrammePhase2ViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePhase2Dropdowns(vm);
                return View(vm);
            }

            var p = await _db.Programmes.FindAsync(vm.Id);
            if (p == null) return NotFound();

            MapFromPhase2ViewModel(vm, p);
            p.UpdatedAt = DateTime.UtcNow;
            p.ComputePhaseCompletion();
            await _db.SaveChangesAsync();
            TempData["Success"] = "Phase 2 saved successfully.";
            return RedirectToAction(nameof(Details), new { id = p.Id });
        }

        // ── GET: Programmes/Delete/5 ──
        [Authorize(Roles = "Admin,Account Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Programmes
                .Include(p => p.ProgrammeType)
                .Include(p => p.Deliverables)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) return NotFound();
            return View(p);
        }

        // ── POST: Programmes/Delete/5 ──
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Account Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _db.Programmes.FindAsync(id);
            if (p != null)
            {
                _db.Programmes.Remove(p);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Programme deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ── API: Get default period for programme type ──
        [HttpGet]
        public async Task<IActionResult> GetDefaultPeriod(int typeId)
        {
            var pt = await _db.ProgrammeTypes.FindAsync(typeId);
            return Json(new { months = pt?.DefaultPeriodMonths });
        }

        // ── Mapping helpers ──
        private static ProgrammePhase1ViewModel MapToPhase1ViewModel(Programme p) => new()
        {
            Id = p.Id, DeliveryType = p.DeliveryType, ClientName = p.ClientName,
            ContactPerson = p.ContactPerson, ContactDetails = p.ContactDetails,
            StartDate = p.StartDate, Stipend = p.Stipend, Bursary = p.Bursary,
            Funding = p.Funding, EEServices = p.EEServices, WSPServices = p.WSPServices,
            NumberOfLearners = p.NumberOfLearners, AbsorptionAssistance = p.AbsorptionAssistance,
            DisabilityRatio = p.DisabilityRatio, TypeOfDisability = p.TypeOfDisability,
            MaleCapacity = p.MaleCapacity, FemaleCapacity = p.FemaleCapacity,
            OtherGenderCapacity = p.OtherGenderCapacity, EEComposition = p.EEComposition,
            PreferredProvince = p.PreferredProvince, AccountManagerId = p.AccountManagerId,
            ProgrammeTypeId = p.ProgrammeTypeId, ProgrammeName = p.ProgrammeName,
            TopUpSkillsTraining = p.TopUpSkillsTraining, SoftSkillsTraining = p.SoftSkillsTraining,
            ReportingFrequency = p.ReportingFrequency, LearnerRequiredAttributes = p.LearnerRequiredAttributes,
            TrainingPeriodMonths = p.TrainingPeriodMonths, HostingPeriodMonths = p.HostingPeriodMonths,
            WorkplacePeriodMonths = p.WorkplacePeriodMonths, TotalPeriodMonths = p.TotalPeriodMonths,
            AbsenteeismManagedBy = p.AbsenteeismManagedBy, ReplacementPolicy = p.ReplacementPolicy,
            ReplacementTimeframe = p.ReplacementTimeframe
        };

        private static void MapFromPhase1ViewModel(ProgrammePhase1ViewModel vm, Programme p)
        {
            p.DeliveryType = vm.DeliveryType; p.ClientName = vm.ClientName;
            p.ContactPerson = vm.ContactPerson; p.ContactDetails = vm.ContactDetails;
            p.StartDate = vm.StartDate; p.Stipend = vm.Stipend; p.Bursary = vm.Bursary;
            p.Funding = vm.Funding; p.EEServices = vm.EEServices; p.WSPServices = vm.WSPServices;
            p.NumberOfLearners = vm.NumberOfLearners; p.AbsorptionAssistance = vm.AbsorptionAssistance;
            p.DisabilityRatio = vm.DisabilityRatio; p.TypeOfDisability = vm.TypeOfDisability;
            p.MaleCapacity = vm.MaleCapacity; p.FemaleCapacity = vm.FemaleCapacity;
            p.OtherGenderCapacity = vm.OtherGenderCapacity; p.EEComposition = vm.EEComposition;
            p.PreferredProvince = vm.PreferredProvince; p.AccountManagerId = vm.AccountManagerId;
            p.ProgrammeTypeId = vm.ProgrammeTypeId; p.ProgrammeName = vm.ProgrammeName;
            p.TopUpSkillsTraining = vm.TopUpSkillsTraining; p.SoftSkillsTraining = vm.SoftSkillsTraining;
            p.ReportingFrequency = vm.ReportingFrequency; p.LearnerRequiredAttributes = vm.LearnerRequiredAttributes;
            p.TrainingPeriodMonths = vm.TrainingPeriodMonths; p.HostingPeriodMonths = vm.HostingPeriodMonths;
            p.WorkplacePeriodMonths = vm.WorkplacePeriodMonths; p.TotalPeriodMonths = vm.TotalPeriodMonths;
            p.AbsenteeismManagedBy = vm.AbsenteeismManagedBy; p.ReplacementPolicy = vm.ReplacementPolicy;
            p.ReplacementTimeframe = vm.ReplacementTimeframe;
        }

        private static ProgrammePhase2ViewModel MapToPhase2ViewModel(Programme p) => new()
        {
            Id = p.Id, ClientName = p.ClientName, ProgrammeName = p.ProgrammeName,
            CoordinatorId = p.CoordinatorId, PayrollId = p.PayrollId,
            FacilitatorId = p.FacilitatorId, TalentManagerId = p.TalentManagerId,
            SETAAdvisorName = p.SETAAdvisorName, SETAAdvisorContact = p.SETAAdvisorContact,
            LearnerPresentation1Date = p.LearnerPresentation1Date,
            LearnerPresentation2Date = p.LearnerPresentation2Date,
            LearnerRegistrationDeadline = p.LearnerRegistrationDeadline,
            PlacementDate = p.PlacementDate, SoftSkillsStartDate = p.SoftSkillsStartDate,
            ModerationReportDate = p.ModerationReportDate,
            VerificationTriggerDate = p.VerificationTriggerDate,
            ReportingIntervals = p.ReportingIntervals, Mentorship = p.Mentorship,
            SoftSkillsYN = p.SoftSkillsYN, AnyAddOn = p.AnyAddOn, AddOnComment = p.AddOnComment
        };

        private static void MapFromPhase2ViewModel(ProgrammePhase2ViewModel vm, Programme p)
        {
            p.CoordinatorId = vm.CoordinatorId; p.PayrollId = vm.PayrollId;
            p.FacilitatorId = vm.FacilitatorId; p.TalentManagerId = vm.TalentManagerId;
            p.SETAAdvisorName = vm.SETAAdvisorName; p.SETAAdvisorContact = vm.SETAAdvisorContact;
            p.LearnerPresentation1Date = vm.LearnerPresentation1Date;
            p.LearnerPresentation2Date = vm.LearnerPresentation2Date;
            p.LearnerRegistrationDeadline = vm.LearnerRegistrationDeadline;
            p.PlacementDate = vm.PlacementDate; p.SoftSkillsStartDate = vm.SoftSkillsStartDate;
            p.ModerationReportDate = vm.ModerationReportDate;
            p.VerificationTriggerDate = vm.VerificationTriggerDate;
            p.ReportingIntervals = vm.ReportingIntervals; p.Mentorship = vm.Mentorship;
            p.SoftSkillsYN = vm.SoftSkillsYN; p.AnyAddOn = vm.AnyAddOn;
            p.AddOnComment = vm.AddOnComment;
        }

        private async Task PopulatePhase1Dropdowns(ProgrammePhase1ViewModel vm)
        {
            var types = await _db.ProgrammeTypes.Where(t => t.IsActive).ToListAsync();
            var managers = await _db.Users.Where(u => u.IsActive && u.Role == Roles.AccountManager).ToListAsync();

            vm.DeliveryTypes = new SelectList(DeliveryTypes.All);
            vm.ProgrammeTypes = new SelectList(types, "Id", "Name", vm.ProgrammeTypeId);
            vm.AccountManagers = new SelectList(managers.Select(u => new { u.Id, Name = u.FullName }), "Id", "Name", vm.AccountManagerId);
            vm.Provinces = new SelectList(Provinces.All);
            vm.ReportingFrequencies = new SelectList(ReportingFrequencies.All);
        }

        private async Task PopulatePhase2Dropdowns(ProgrammePhase2ViewModel vm)
        {
            var users = await _db.Users.Where(u => u.IsActive).ToListAsync();

            vm.Coordinators = new SelectList(
                users.Where(u => u.Role == Roles.ProjectCoordinator || u.Role == Roles.Admin)
                     .Select(u => new { u.Id, Name = u.FullName }), "Id", "Name", vm.CoordinatorId);

            vm.Facilitators = new SelectList(
                users.Where(u => u.Role == Roles.Facilitator || u.Role == Roles.Admin)
                     .Select(u => new { u.Id, Name = u.FullName }), "Id", "Name", vm.FacilitatorId);

            vm.TalentManagers = new SelectList(
                users.Where(u => u.Role == Roles.TalentManager || u.Role == Roles.Admin)
                     .Select(u => new { u.Id, Name = u.FullName }), "Id", "Name", vm.TalentManagerId);

            vm.PayrollUsers = new SelectList(
                users.Where(u => u.Role == Roles.Payroll || u.Role == Roles.Admin)
                     .Select(u => new { u.Id, Name = u.FullName }), "Id", "Name", vm.PayrollId);

            vm.ReportingIntervalOptions = new SelectList(ReportingFrequencies.All);
        }
    }
}
