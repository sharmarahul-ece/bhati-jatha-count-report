using Microsoft.AspNetCore.Mvc;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Models.ViewModels;
using bhati_jatha_count_report.Services.Interfaces;

namespace bhati_jatha_count_report.Controllers;

public class CenterController : Controller
{
  private readonly ICenterService _centerService;
  private readonly ILogger<CenterController> _logger;

  public CenterController(ICenterService centerService, ILogger<CenterController> logger)
  {
    _centerService = centerService;
    _logger = logger;
  }

  public async Task<IActionResult> Index()
  {
    try
    {
      var centers = await _centerService.GetAllCentersAsync();
      return View(centers);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while fetching centers");
      return View(new List<Center>());
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([FromBody] CenterViewModel viewModel)
  {
    try
    {
      if (ModelState.IsValid)
      {
        var center = new Center
        {
          CenterName = viewModel.CenterName,
          CenterType = viewModel.CenterType
        };

        await _centerService.CreateCenterAsync(center);
        return Json(new { success = true, message = "Center created successfully" });
      }
      return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }
    catch (InvalidOperationException ex)
    {
      return Json(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while creating center");
      return Json(new { success = false, message = "An error occurred while creating the center" });
    }
  }

  [HttpPut]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Update(int id, [FromBody] CenterUpdateViewModel viewModel)
  {
    try
    {
      if (ModelState.IsValid)
      {
        var center = new Center
        {
          Id = viewModel.Id,
          CenterName = viewModel.CenterName,
          CenterType = viewModel.CenterType
        };

        var updatedCenter = await _centerService.UpdateCenterAsync(id, center);
        if (updatedCenter == null)
        {
          return Json(new { success = false, message = "Center not found" });
        }
        return Json(new { success = true, message = "Center updated successfully" });
      }
      return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }
    catch (InvalidOperationException ex)
    {
      return Json(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while updating center");
      return Json(new { success = false, message = "An error occurred while updating the center" });
    }
  }

  [HttpDelete]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      var result = await _centerService.DeleteCenterAsync(id);
      if (!result)
      {
        return Json(new { success = false, message = "Center not found" });
      }
      return Json(new { success = true, message = "Center deleted successfully" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while deleting center");
      return Json(new { success = false, message = "An error occurred while deleting the center" });
    }
  }
}