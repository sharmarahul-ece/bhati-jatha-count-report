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
    private readonly ISewaNominalRollService _sewaNominalRollService;

    public DailyActualCountController(IDailyActualCountService service, ICenterService centerService, ISewaTypeService sewaTypeService, ISewaNominalRollService sewaNominalRollService)
    {
      _service = service;
      _centerService = centerService;
      _sewaTypeService = sewaTypeService;
      _sewaNominalRollService = sewaNominalRollService;
    }

    public async Task<IActionResult> Index()
    {
      var allCounts = _service.GetAll().ToList();
      var centersList = (await _centerService.GetAllCentersAsync()).ToList();
      var sewaTypesList = (await _sewaTypeService.GetAllSewaTypesAsync()).ToList();
      var nominalRolls = (await _sewaNominalRollService.Query(null, null, null, null)).ToList();
      var vm = new Models.ViewModels.DailyActualCountPageViewModel
      {
        DailyActualCounts = allCounts.Select(dac =>
        {
          var match = nominalRolls.FirstOrDefault(nr => nr.NominalRollToken == dac.NominalRollToken);
          var centerName = centersList.FirstOrDefault(c => c.Id == dac.CenterId)?.CenterName;
          var sewaTypeName = sewaTypesList.FirstOrDefault(s => s.Id == dac.SewaTypeId)?.SewaName;
          return new Models.ViewModels.DailyActualCountViewModel
          {
            Id = dac.Id,
            Date = dac.Date,
            CenterId = dac.CenterId,
            SewaTypeId = dac.SewaTypeId,
            Count = dac.Count,
            NominalRollToken = dac.NominalRollToken,
            NominalRollFound = match != null,
            NominalRollSewadarCount = match?.TotalSewadars,
            ManualSewadarCount = dac.ManualSewadarCount,
            CenterName = centerName,
            SewaTypeName = sewaTypeName
          };
        }).ToList(),
        Centers = centersList,
        SewaTypes = sewaTypesList
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

      // Try to find nominal roll for this token
      var nominalRoll = _sewaNominalRollService
        .Query(null, null, null, null)
        .Result
        .FirstOrDefault(r => r.NominalRollToken == model.NominalRollToken);

      if (model.Id == 0)
      {
        var entity = new DailyActualCount
        {
          Date = model.Date,
          CenterId = model.CenterId,
          SewaTypeId = model.SewaTypeId,
          Count = model.Count,
          NominalRollToken = model.NominalRollToken,
          ManualSewadarCount = model.ManualSewadarCount // Always save the manual count, use for display only if nominal roll not found
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
        entity.ManualSewadarCount = model.ManualSewadarCount; // Always save the manual count
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
    public async Task<IActionResult> GetFiltered(DateTime? fromDate, DateTime? toDate, int? centerId, int? sewaTypeId)
    {
      var filteredData = _service.GetFiltered(fromDate, toDate, centerId, sewaTypeId).ToList();
      var centersList = (await _centerService.GetAllCentersAsync()).ToList();
      var sewaTypesList = (await _sewaTypeService.GetAllSewaTypesAsync()).ToList();
      var nominalRolls = (await _sewaNominalRollService.Query(null, null, null, null)).ToList();
      var result = filteredData.Select(dac =>
      {
        var match = nominalRolls.FirstOrDefault(nr => nr.NominalRollToken == dac.NominalRollToken);
        var centerName = centersList.FirstOrDefault(c => c.Id == dac.CenterId)?.CenterName;
        var sewaTypeName = sewaTypesList.FirstOrDefault(s => s.Id == dac.SewaTypeId)?.SewaName;
        return new Models.ViewModels.DailyActualCountViewModel
        {
          Id = dac.Id,
          Date = dac.Date,
          CenterId = dac.CenterId,
          SewaTypeId = dac.SewaTypeId,
          Count = dac.Count,
          NominalRollToken = dac.NominalRollToken,
          NominalRollFound = match != null,
          NominalRollSewadarCount = match?.TotalSewadars,
          ManualSewadarCount = dac.ManualSewadarCount,
          CenterName = centerName,
          SewaTypeName = sewaTypeName
        };
      }).ToList();
      return Json(new { success = true, data = result });
    }
  }
}
