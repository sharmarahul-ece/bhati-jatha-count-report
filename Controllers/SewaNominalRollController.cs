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

  public IActionResult Import()
  {
    return View();
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

      // Import the data
      await _sewaNominalRollService.Import(tempFile);

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
}