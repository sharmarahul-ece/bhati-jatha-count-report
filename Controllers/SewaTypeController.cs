using Microsoft.AspNetCore.Mvc;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Models.ViewModels;
using bhati_jatha_count_report.Services.Interfaces;

namespace bhati_jatha_count_report.Controllers;

public class SewaTypeController : Controller
{
  private readonly ISewaTypeService _sewaTypeService;
  private readonly ILogger<SewaTypeController> _logger;

  public SewaTypeController(ISewaTypeService sewaTypeService, ILogger<SewaTypeController> logger)
  {
    _sewaTypeService = sewaTypeService;
    _logger = logger;
  }

  public async Task<IActionResult> Index()
  {
    try
    {
      var sewaTypes = await _sewaTypeService.GetAllSewaTypesAsync();
      return View(sewaTypes);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while fetching sewa types");
      return View(new List<SewaType>());
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create([FromBody] SewaTypeViewModel viewModel)
  {
    try
    {
      if (ModelState.IsValid)
      {
        var sewaType = new SewaType
        {
          SewaName = viewModel.SewaName
        };

        await _sewaTypeService.CreateSewaTypeAsync(sewaType);
        return Json(new { success = true, message = "Sewa Type created successfully" });
      }
      return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }
    catch (InvalidOperationException ex)
    {
      return Json(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while creating sewa type");
      return Json(new { success = false, message = "An error occurred while creating the sewa type" });
    }
  }

  [HttpPut]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Update(int id, [FromBody] SewaTypeUpdateViewModel viewModel)
  {
    try
    {
      if (ModelState.IsValid)
      {
        var sewaType = new SewaType
        {
          Id = viewModel.Id,
          SewaName = viewModel.SewaName
        };

        var updatedSewaType = await _sewaTypeService.UpdateSewaTypeAsync(id, sewaType);
        if (updatedSewaType == null)
        {
          return Json(new { success = false, message = "Sewa Type not found" });
        }
        return Json(new { success = true, message = "Sewa Type updated successfully" });
      }
      return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }
    catch (InvalidOperationException ex)
    {
      return Json(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while updating sewa type");
      return Json(new { success = false, message = "An error occurred while updating the sewa type" });
    }
  }

  [HttpDelete]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      var result = await _sewaTypeService.DeleteSewaTypeAsync(id);
      if (!result)
      {
        return Json(new { success = false, message = "Sewa Type not found" });
      }
      return Json(new { success = true, message = "Sewa Type deleted successfully" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while deleting sewa type");
      return Json(new { success = false, message = "An error occurred while deleting the sewa type" });
    }
  }
}