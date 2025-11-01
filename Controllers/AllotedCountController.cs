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

        // Log for debugging
        Console.WriteLine($"Selected Date: {selectedDate:yyyy-MM-dd}");
        Console.WriteLine($"Total Centers: {centers.Count}");
        Console.WriteLine($"Total Sewa Types: {sewaTypes.Count}");
        Console.WriteLine($"Total Actual Counts for date: {actualCounts.Count}");

        // Filter centers that have at least one actual count for the date
        var centersWithData = centers.Where(c =>
          actualCounts.Any(ac => ac.CenterId == c.Id)).ToList();

        Console.WriteLine($"Centers with data: {centersWithData.Count}");

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

          Console.WriteLine($"Worksheet name: {worksheet.Name}, Row count: {worksheet.RowsUsed().Count()}");

          // Fill the date in cell C2
          worksheet.Cell("C2").Value = selectedDate.ToString("dd/MM/yyyy");
          Console.WriteLine($"Set date in C2: {worksheet.Cell("C2").Value}");

          // Fill Sewa Type headers starting from E3
          int sewaTypeColumn = 5; // Column E
          foreach (var sewaType in sewaTypes)
          {
            worksheet.Cell(3, sewaTypeColumn).Value = sewaType.SewaName;
            sewaTypeColumn++;
          }

          // Fill center data starting from row 7, inserting a new row for each center
          int insertRow = 7;
          int serialNumber = 1;

          foreach (var center in centersWithData)
          {
            // Insert a new row at the current position (insertRow)
            worksheet.Row(insertRow).InsertRowsAbove(1);

            // In columns R and S (18, 19), set formula =SUM(E:Q) for that row
            worksheet.Cell(insertRow, 18).FormulaA1 = $"SUM(E{insertRow}:Q{insertRow})";
            worksheet.Cell(insertRow, 19).FormulaA1 = $"SUM(E{insertRow}:Q{insertRow})";
            // In column T (20), set formula =R-S for that row
            worksheet.Cell(insertRow, 20).FormulaA1 = $"R{insertRow}-S{insertRow}";

            // Write data to the newly inserted row
            worksheet.Cell(insertRow, 2).Value = serialNumber;
            worksheet.Cell(insertRow, 3).Value = center.CenterName;
            worksheet.Cell(insertRow, 4).Value = center.CenterType;

            // Fill actual counts for each sewa type (starting from column E)
            int dataColumn = 5; // Column E
            foreach (var sewaType in sewaTypes)
            {
              var actualCount = actualCounts.FirstOrDefault(ac =>
                ac.CenterId == center.Id && ac.SewaTypeId == sewaType.Id);

              if (actualCount != null)
              {
                worksheet.Cell(insertRow, dataColumn).Value = actualCount.Count;
                Console.WriteLine($"Row {insertRow}, Col {dataColumn}: Center {center.CenterName}, Sewa {sewaType.SewaName}, Count {actualCount.Count}");
              }
              else
              {
                worksheet.Cell(insertRow, dataColumn).Value = string.Empty;
              }
              dataColumn++;
            }

            // Apply border to all cells in the new row (columns 2 to last dataColumn-1)
            for (int col = 2; col < dataColumn; col++)
            {
              var cell = worksheet.Cell(insertRow, col);
              var border = cell.Style.Border;
              border.TopBorder = XLBorderStyleValues.Thin;
              border.BottomBorder = XLBorderStyleValues.Thin;
              border.LeftBorder = XLBorderStyleValues.Thin;
              border.RightBorder = XLBorderStyleValues.Thin;
            }

            insertRow++;
            serialNumber++;
          }

          // After writing all data, delete the next two rows after the last inserted row
          worksheet.Row(insertRow).Delete();
          worksheet.Row(insertRow).Delete();

          // Then delete the original sample rows at row 5 and 6
          worksheet.Row(5).Delete();
          worksheet.Row(5).Delete();

          Console.WriteLine($"Total rows filled: {insertRow - 7}");
          Console.WriteLine($"Verifying C2 value before save: {worksheet.Cell("C2").Value}");
          Console.WriteLine($"Verifying B5 value before save: {worksheet.Cell("B5").Value}");

          // Save to memory stream and return as file
          using (var outputStream = new MemoryStream())
          {
            workbook.SaveAs(outputStream);
            outputStream.Position = 0; // Ensure stream is at the beginning before reading
            var fileBytes = outputStream.ToArray();

            Console.WriteLine($"Excel file generated successfully. Size: {fileBytes.Length} bytes");

            var fileName = $"actual_counts_{selectedDate:yyyy-MM-dd}.xlsx";
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