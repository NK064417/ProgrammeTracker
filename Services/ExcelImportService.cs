using ExcelDataReader;
using ProgrammeTracker.Data;
using ProgrammeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ProgrammeTracker.Services
{
    public class ExcelImportService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ExcelImportService> _logger;
        private const string ImportFlagKey = "ExcelImportComplete";

        public ExcelImportService(ApplicationDbContext db, ILogger<ExcelImportService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task RunIfNeededAsync(string filePath)
        {
            var flag = await _db.AppSettings.FindAsync(ImportFlagKey);
            if (flag != null) return; // Already ran

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Import file not found at {Path} — skipping import", filePath);
                return;
            }

            _logger.LogInformation("Starting Excel import from {Path}", filePath);
            int imported = 0, errors = 0;

            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var dataset = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                });

                var defaultType = await _db.ProgrammeTypes.FirstOrDefaultAsync(t => t.Name == "Learnership");
                int typeId = defaultType?.Id ?? 1;

                var adminUser = await _db.Users.FirstOrDefaultAsync();
                int createdBy = adminUser?.Id ?? 1;

                foreach (System.Data.DataTable sheet in dataset.Tables)
                {
                    string deliveryType = sheet.TableName.ToLower().Contains("online") ? "Online"
                        : sheet.TableName.ToLower().Contains("inhouse") ? "Inhouse"
                        : "Campus";

                    foreach (System.Data.DataRow row in sheet.Rows)
                    {
                        try
                        {
                            string? clientName = row.ItemArray.Length > 0 ? row[0]?.ToString() : null;
                            if (string.IsNullOrWhiteSpace(clientName)) continue;

                            string? programmeName = row.ItemArray.Length > 1 ? row[1]?.ToString() : null;

                            _ = DateTime.TryParse(row.ItemArray.Length > 2 ? row[2]?.ToString() : null, out var startDate);

                            var programme = new Programme
                            {
                                ClientName = clientName,
                                ProgrammeName = programmeName ?? "Unknown",
                                DeliveryType = deliveryType,
                                ProgrammeTypeId = typeId,
                                StartDate = startDate == default ? null : startDate,
                                Status = "Active",
                                Funding = "No",
                                IsLegacyImport = true,
                                CreatedByUserId = createdBy,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            _db.Programmes.Add(programme);
                            imported++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error importing row");
                            errors++;
                        }
                    }
                }

                await _db.SaveChangesAsync();

                // Mark as done
                _db.AppSettings.Add(new AppSettings { Key = ImportFlagKey, Value = DateTime.UtcNow.ToString("O") });
                await _db.SaveChangesAsync();

                _logger.LogInformation("Import complete. Imported: {Imported}, Errors: {Errors}", imported, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel import failed");
            }
        }
    }
}
