using Microsoft.AspNetCore.Mvc;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;
using System.Threading.Tasks;

namespace bhati_jatha_count_report.Controllers
{
  public class AllotedCountController : Controller
  {
    private readonly IAllotedCountService _service;
    private readonly ICenterService _centerService;
    private readonly ISewaTypeService _sewaTypeService;

    public AllotedCountController(IAllotedCountService service, ICenterService centerService, ISewaTypeService sewaTypeService)
    {
      _service = service;
      _centerService = centerService;
      _sewaTypeService = sewaTypeService;
    }

    public async Task<IActionResult> Index()
    {
      var vm = new Models.ViewModels.AllotedCountPageViewModel
      {
        AllotedCounts = _service.GetAll().ToList(),
        Centers = (await _centerService.GetAllCentersAsync()).ToList(),
        SewaTypes = (await _sewaTypeService.GetAllSewaTypesAsync()).ToList()
      };
      return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save([FromBody] Models.ViewModels.AllotedCountViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
      }

      var existingEntity = _service.GetById(model.WeekDay, model.CenterId, model.SewaTypeId);

      if (existingEntity == null)
      {
        var entity = new AllotedCount
        {
          WeekDay = model.WeekDay,
          CenterId = model.CenterId,
          SewaTypeId = model.SewaTypeId,
          Count = model.Count
        };
        _service.Add(entity);
        return Json(new { success = true, message = "Created successfully" });
      }
      else
      {
        existingEntity.Count = model.Count;
        _service.Update(existingEntity);
        return Json(new { success = true, message = "Updated successfully" });
      }
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(WeekDay weekDay, int centerId, int sewaTypeId)
    {
      var entity = _service.GetById(weekDay, centerId, sewaTypeId);
      if (entity == null)
        return Json(new { success = false, message = "Not found" });

      _service.Delete(weekDay, centerId, sewaTypeId);
      return Json(new { success = true, message = "Deleted successfully" });
    }

    [HttpGet]
    public IActionResult Get(WeekDay weekDay, int centerId, int sewaTypeId)
    {
      var entity = _service.GetById(weekDay, centerId, sewaTypeId);
      if (entity == null)
        return Json(new { success = false, message = "Not found" });

      return Json(new { success = true, data = entity });
    }
    [HttpGet]
    public async Task<IActionResult> Tabular(int? weekDay)
    {
      var centers = (await _centerService.GetAllCentersAsync()).ToList();
      var sewaTypes = (await _sewaTypeService.GetAllSewaTypesAsync()).ToList();
      var allotedCounts = _service.GetAll().ToList();
      int selectedWeekDay = weekDay ?? (int)DateTime.Now.DayOfWeek;
      var weekDays = System.Enum.GetValues(typeof(WeekDay))
        .Cast<WeekDay>()
        .Select(wd => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
          Value = ((int)wd).ToString(),
          Text = wd.ToString()
        }).ToList();

      var vm = new Models.ViewModels.AllotedCountTabularViewModel
      {
        Centers = centers,
        SewaTypes = sewaTypes,
        AllotedCounts = allotedCounts.Where(x => (int)x.WeekDay == selectedWeekDay).ToList(),
        SelectedWeekDay = selectedWeekDay,
        WeekDays = weekDays
      };
      return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> PendingActual(DateTime? date, int? weekDay)
    {
      var centers = (await _centerService.GetAllCentersAsync()).ToList();
      var sewaTypes = (await _sewaTypeService.GetAllSewaTypesAsync()).ToList();
      var allotedCounts = _service.GetAll().ToList();
      var actualCounts = HttpContext.RequestServices.GetService(typeof(IDailyActualCountService)) is IDailyActualCountService actualService
        ? actualService.GetAll().ToList() : new List<bhati_jatha_count_report.Models.Entities.DailyActualCount>();
      DateTime selectedDate = date ?? DateTime.Today;
      int selectedWeekDay = weekDay ?? (int)selectedDate.DayOfWeek;
      var weekDays = System.Enum.GetValues(typeof(WeekDay))
        .Cast<WeekDay>()
        .Select(wd => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
          Value = ((int)wd).ToString(),
          Text = wd.ToString()
        }).ToList();

      var vm = new Models.ViewModels.PendingActualCountViewModel
      {
        Centers = centers,
        SewaTypes = sewaTypes,
        AllotedCounts = allotedCounts.Where(x => (int)x.WeekDay == selectedWeekDay).ToList(),
        DailyActualCounts = actualCounts.Where(x => x.Date.Date == selectedDate.Date).ToList(),
        SelectedWeekDay = selectedWeekDay,
        SelectedDate = selectedDate,
        WeekDays = weekDays
      };
      return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> PendingSimple(DateTime? date)
    {
      var centers = (await _centerService.GetAllCentersAsync()).ToList();
      var sewaTypes = (await _sewaTypeService.GetAllSewaTypesAsync()).ToList();
      var allotedCounts = _service.GetAll().ToList();
      var actualCounts = HttpContext.RequestServices.GetService(typeof(IDailyActualCountService)) is IDailyActualCountService actualService
        ? actualService.GetAll().ToList() : new List<bhati_jatha_count_report.Models.Entities.DailyActualCount>();
      var excludedService = HttpContext.RequestServices.GetService(typeof(IExcludedForDayService)) as IExcludedForDayService;
      DateTime selectedDate = date ?? DateTime.Today;
      int selectedWeekDay = (int)selectedDate.DayOfWeek;

      var excluded = excludedService != null ? await excludedService.GetExcludedAsync(selectedDate) : new List<ExcludedForDay>();

      var pending = allotedCounts
        .Where(x => (int)x.WeekDay == selectedWeekDay)
        .Where(x => !actualCounts.Any(a => a.Date.Date == selectedDate.Date && a.CenterId == x.CenterId && a.SewaTypeId == x.SewaTypeId))
        .Select(x => new Models.ViewModels.PendingSimpleListViewModel.PendingItem
        {
          CenterId = x.CenterId,
          SewaTypeId = x.SewaTypeId,
          CenterName = centers.FirstOrDefault(c => c.Id == x.CenterId)?.CenterName ?? $"Center {x.CenterId}",
          SewaTypeName = sewaTypes.FirstOrDefault(s => s.Id == x.SewaTypeId)?.SewaName ?? $"SewaType {x.SewaTypeId}",
          IsExcluded = excluded.Any(e => e.CenterId == x.CenterId && e.SewaTypeId == x.SewaTypeId)
        })
        .ToList();

      var vm = new Models.ViewModels.PendingSimpleListViewModel
      {
        SelectedDate = selectedDate,
        Pending = pending
      };
      return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleExclude(int centerId, int sewaTypeId, DateTime date)
    {
      var excludedService = HttpContext.RequestServices.GetService(typeof(IExcludedForDayService)) as IExcludedForDayService;
      if (excludedService == null)
        return RedirectToAction("PendingSimple", new { date = date.ToString("yyyy-MM-dd") });
      bool isExcluded = await excludedService.IsExcludedAsync(centerId, sewaTypeId, date);
      if (isExcluded)
      {
        await excludedService.RemoveAsync(centerId, sewaTypeId, date);
      }
      else
      {
        await excludedService.AddAsync(centerId, sewaTypeId, date);
      }
      return RedirectToAction("PendingSimple", new { date = date.ToString("yyyy-MM-dd") });
    }
  }
}