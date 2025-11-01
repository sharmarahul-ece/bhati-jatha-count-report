using Microsoft.AspNetCore.Mvc;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;
using System.Threading.Tasks;
using ClosedXML.Excel;

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

    [HttpGet]
    public async Task<IActionResult> ExportActualCountsExcel(DateTime? date)
    {
      try
      {
        // Get the selected date
        DateTime selectedDate = date ?? DateTime.Today;

        // Get all required data
        var centers = (await _centerService.GetAllCentersAsync()).OrderBy(c => c.CenterName).ToList();
        var sewaTypes = (await _sewaTypeService.GetAllSewaTypesAsync()).OrderBy(s => s.Id).ToList();
        var actualCountService = HttpContext.RequestServices.GetService(typeof(IDailyActualCountService)) as IDailyActualCountService;
        var actualCounts = actualCountService?.GetAll().Where(x => x.Date.Date == selectedDate.Date).ToList()
          ?? new List<DailyActualCount>();

        // Filter centers that have at least one actual count for the date
        var centersWithData = centers.Where(c =>
          actualCounts.Any(ac => ac.CenterId == c.Id)).ToList();

        // Path to the Excel template
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "actual_counts_template.xlsx");

        if (!System.IO.File.Exists(templatePath))
        {
          return NotFound("Excel template not found. Please ensure 'actual_counts_template.xlsx' exists in wwwroot/templates/");
        }

        // Load the template into memory first
        byte[] templateBytes = System.IO.File.ReadAllBytes(templatePath);
        using (var templateStream = new MemoryStream(templateBytes))
        using (var workbook = new XLWorkbook(templateStream))
        {
          var worksheet = workbook.Worksheet(1); // Get the first worksheet

          // Fill the date in cell B2
          worksheet.Cell("B2").Value = selectedDate.ToString("dd/MM/yyyy");

          // Fill Sewa Type headers starting from E3
          int sewaTypeColumn = 5; // Column E
          foreach (var sewaType in sewaTypes)
          {
            worksheet.Cell(3, sewaTypeColumn).Value = sewaType.SewaName;
            sewaTypeColumn++;
          }

          // Start inserting data after the two sample rows (row 7)
          int insertRow = 7;
          int serialNumber = 1;

          // For each center with data, insert a new row and populate it
          foreach (var center in centersWithData)
          {
            // Insert a new row at the current position (insertRow)
            worksheet.Row(insertRow).InsertRowsAbove(1);

            // --- Set formulas for summary columns ---
            // Columns R and S (18, 19): SUM of E:Q for this row
            worksheet.Cell(insertRow, 18).FormulaA1 = $"SUM(E{insertRow}:Q{insertRow})";
            worksheet.Cell(insertRow, 19).FormulaA1 = $"SUM(E{insertRow}:Q{insertRow})";
            // Column T (20): Difference between R and S for this row
            worksheet.Cell(insertRow, 20).FormulaA1 = $"R{insertRow}-S{insertRow}";

            // --- Fill in center details ---
            worksheet.Cell(insertRow, 2).Value = serialNumber; // Serial Number
            worksheet.Cell(insertRow, 3).Value = center.CenterName; // Center Name
            worksheet.Cell(insertRow, 4).Value = center.CenterType; // Center Type

            // --- Fill actual counts for each sewa type (columns E to Q) ---
            int dataColumn = 5; // Start at column E
            foreach (var sewaType in sewaTypes)
            {
              var actualCount = actualCounts.FirstOrDefault(ac =>
                ac.CenterId == center.Id && ac.SewaTypeId == sewaType.Id);

              if (actualCount != null)
              {
                worksheet.Cell(insertRow, dataColumn).Value = actualCount.Count;
              }
              else
              {
                worksheet.Cell(insertRow, dataColumn).Value = string.Empty;
              }
              dataColumn++;
            }

            // --- Apply border to all cells in the new row (columns 2 to last dataColumn-1) ---
            // for (int col = 2; col < dataColumn; col++)
            // {
            //   var cell = worksheet.Cell(insertRow, col);
            //   var border = cell.Style.Border;
            //   border.TopBorder = XLBorderStyleValues.Thin;
            //   border.BottomBorder = XLBorderStyleValues.Thin;
            //   border.LeftBorder = XLBorderStyleValues.Thin;
            //   border.RightBorder = XLBorderStyleValues.Thin;
            // }

            // Move to the next row for the next center
            insertRow++;
            serialNumber++;
          }

          // --- Clean up template/sample rows ---
          // After writing all data, delete the next two rows after the last inserted row (these are template/sample rows)
          worksheet.Row(insertRow).Delete();
          worksheet.Row(insertRow).Delete();

          // Then delete the original sample rows at row 5 and 6
          worksheet.Row(5).Delete();
          worksheet.Row(5).Delete();

          // Save to memory stream and return as file
          using (var outputStream = new MemoryStream())
          {
            workbook.SaveAs(outputStream);
            outputStream.Position = 0; // Ensure stream is at the beginning before reading
            var fileBytes = outputStream.ToArray();

            Console.WriteLine($"Excel file generated successfully. Size: {fileBytes.Length} bytes");

            var fileName = $"Bhati_Jatha_Actual_Report_{selectedDate:yyyy_MM_dd}.xlsx";
            return File(fileBytes,
              "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
              fileName);
          }
        }

        // (Removed duplicate and misplaced code block)
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        return StatusCode(500, $"Error generating Excel file: {ex.Message}");
      }
    }
  }
}