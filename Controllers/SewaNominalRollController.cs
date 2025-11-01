using Microsoft.AspNetCore.Mvc;
using bhati_jatha_count_report.Services.Interfaces;

namespace bhati_jatha_count_report.Controllers;

public class SewaNominalRollController : Controller
{
  private readonly ISewaNominalRollService _sewaNominalRollService;
  private readonly ILogger<SewaNominalRollController> _logger;

  public SewaNominalRollController(
      ISewaNominalRollService sewaNominalRollService,
      ILogger<SewaNominalRollController> logger)
  {
    _sewaNominalRollService = sewaNominalRollService;
    _logger = logger;
  }

  public async Task<IActionResult> Import(string? centerFilter, string? sewaTypeFilter, DateTime? startDate, DateTime? endDate)
  {
    // Default to today's date when no range provided
    if (!startDate.HasValue && !endDate.HasValue)
    {
      startDate = DateTime.Today;
      endDate = DateTime.Today;
    }

    var centers = (await _sewaNominalRollService.GetDistinctCentersAsync()).ToList();
    var sewaTypes = (await _sewaNominalRollService.GetDistinctSewaTypesAsync()).ToList();
    var sourceFiles = (await _sewaNominalRollService.GetDistinctSourceFilesAsync()).ToList();

    var results = await _sewaNominalRollService.Query(startDate, endDate, centerFilter, sewaTypeFilter);

    var model = new bhati_jatha_count_report.Models.ViewModels.SewaNominalRollImportViewModel
    {
      Centers = centers,
      SewaTypes = sewaTypes,
      CenterFilter = centerFilter,
      SewaTypeFilter = sewaTypeFilter,
      StartDate = startDate,
      EndDate = endDate,
      Results = results
    };

    model.SourceFiles = sourceFiles;

    return View(model);
  }

  [HttpPost]
  public async Task<IActionResult> Import(IFormFile file)
  {
    if (file == null || file.Length == 0)
    {
      TempData["Error"] = "Please select a file to import";
      return RedirectToAction(nameof(Import));
    }

    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
    {
      TempData["Error"] = "Please upload an Excel file (.xlsx)";
      return RedirectToAction(nameof(Import));
    }

    try
    {
      // Create a temporary file to store the upload
      var tempFile = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid().ToString() + ".xlsx");
      using (var stream = new FileStream(tempFile, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      // Import the data; pass original filename so rows are tagged with it
      await _sewaNominalRollService.Import(tempFile, file.FileName);

      // Clean up the temporary file
      System.IO.File.Delete(tempFile);

      TempData["Success"] = "Data imported successfully";
      return RedirectToAction(nameof(Import));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error importing Excel file");
      TempData["Error"] = "Error importing data: " + ex.Message;
      return RedirectToAction(nameof(Import));
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteFileData(string filename)
  {
    if (string.IsNullOrWhiteSpace(filename))
    {
      TempData["Error"] = "Invalid filename";
      return RedirectToAction(nameof(Import));
    }

    try
    {
      await _sewaNominalRollService.DeleteBySourceFileAsync(filename);
      TempData["Success"] = $"All rows imported from '{filename}' have been deleted.";
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting rows for file {filename}", filename);
      TempData["Error"] = "Error deleting rows: " + ex.Message;
    }

    return RedirectToAction(nameof(Import));
  }
}