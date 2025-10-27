using Microsoft.AspNetCore.Mvc;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;

namespace bhati_jatha_count_report.Controllers
{
  public class DailyActualCountController : Controller
  {
    private readonly IDailyActualCountService _service;
    private readonly ICenterService _centerService;
    private readonly ISewaTypeService _sewaTypeService;

    public DailyActualCountController(IDailyActualCountService service, ICenterService centerService, ISewaTypeService sewaTypeService)
    {
      _service = service;
      _centerService = centerService;
      _sewaTypeService = sewaTypeService;
    }

    public async Task<IActionResult> Index()
    {
      var vm = new Models.ViewModels.DailyActualCountPageViewModel
      {
        DailyActualCounts = _service.GetAll().ToList(),
        Centers = (await _centerService.GetAllCentersAsync()).ToList(),
        SewaTypes = (await _sewaTypeService.GetAllSewaTypesAsync()).ToList()
      };
      return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save([FromBody] Models.ViewModels.DailyActualCountViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
      }

      if (model.Id == 0)
      {
        var entity = new DailyActualCount
        {
          Date = model.Date,
          CenterId = model.CenterId,
          SewaTypeId = model.SewaTypeId,
          Count = model.Count,
          NominalRollToken = model.NominalRollToken
        };
        _service.Add(entity);
        return Json(new { success = true, message = "Created successfully" });
      }
      else
      {
        var entity = _service.GetById(model.Id);
        if (entity == null)
          return Json(new { success = false, message = "Not found" });
        entity.Date = model.Date;
        entity.CenterId = model.CenterId;
        entity.SewaTypeId = model.SewaTypeId;
        entity.Count = model.Count;
        entity.NominalRollToken = model.NominalRollToken;
        _service.Update(entity);
        return Json(new { success = true, message = "Updated successfully" });
      }
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
      var entity = _service.GetById(id);
      if (entity == null)
        return Json(new { success = false, message = "Not found" });
      _service.Delete(id);
      return Json(new { success = true, message = "Deleted successfully" });
    }

    [HttpGet]
    public IActionResult Get(int id)
    {
      var entity = _service.GetById(id);
      if (entity == null)
        return Json(new { success = false, message = "Not found" });
      return Json(new { success = true, data = entity });
    }

    [HttpGet]
    public IActionResult GetFiltered(DateTime? fromDate, DateTime? toDate, int? centerId, int? sewaTypeId)
    {
      var filteredData = _service.GetFiltered(fromDate, toDate, centerId, sewaTypeId);
      return Json(new { success = true, data = filteredData });
    }
  }
}
