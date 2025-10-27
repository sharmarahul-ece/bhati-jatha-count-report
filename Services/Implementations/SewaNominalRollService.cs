using System.ComponentModel.DataAnnotations;
using ClosedXML.Excel;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Data;
using bhati_jatha_count_report.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IO;

public class SewaNominalRollService : ISewaNominalRollService
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<SewaNominalRollService> _logger;

  public SewaNominalRollService(ApplicationDbContext context, ILogger<SewaNominalRollService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Import(string excelPath, string sourceFileName)
  {
    using var workbook = new XLWorkbook(excelPath);
    var worksheet = workbook.Worksheets.First();
    var firstDataRow = 3;
    var lastRow = worksheet.LastRowUsed();
    if (lastRow == null)
      throw new InvalidOperationException("No data rows found in worksheet.");
    var lastDataRow = lastRow.RowNumber();

    var validationErrors = new List<string>();

    for (int rowIdx = firstDataRow; rowIdx <= lastDataRow; rowIdx++)
    {
      var row = worksheet.Row(rowIdx);

      var nominalRollToken = row.Cell(1).GetString().Trim();
      var sewaDate = ParseDate(row.Cell(8).GetString());
      var sewaName = row.Cell(3).GetString() ?? string.Empty;
      // Only import rows matching the desired SewaName
      if (!string.Equals(sewaName?.Trim(), "MAJOR CENTRE - BHATI DELHI", StringComparison.OrdinalIgnoreCase))
      {
        continue;
      }
      var department = row.Cell(4).GetString();
      var zone = row.Cell(5).GetString();
      var centreName = row.Cell(6).GetString();
      var journeyDate = ParseDate(row.Cell(7).GetString());
      var sewaDuration = ParseInt(row.Cell(9).GetString());
      var males = ParseInt(row.Cell(10).GetString());
      var females = ParseInt(row.Cell(11).GetString());
      var totalSewadars = ParseInt(row.Cell(12).GetString());
      var sewaType = row.Cell(13).GetString();
      var sewaStartTime = row.Cell(14).GetString();
      var inchargeName = row.Cell(15).GetString();
      var inchargeContact = row.Cell(16).GetString();
      var remarks = row.Cell(17).GetString();

      // Create new entity without RowId (it's an identity column)
      var entity = new SewaNominalRoll
      {
        NominalRollToken = nominalRollToken,
        SewaDate = sewaDate,
        SewaName = sewaName ?? string.Empty,
        Department = department ?? string.Empty,
        Zone = zone ?? string.Empty,
        CentreName = centreName ?? string.Empty,
        JourneyDate = journeyDate,
        SewaDuration = sewaDuration,
        Males = males,
        Females = females,
        TotalSewadars = totalSewadars,
        SewaType = sewaType ?? string.Empty,
        SewaStartTime = sewaStartTime ?? string.Empty,
        InchargeName = inchargeName ?? string.Empty,
        InchargeContact = inchargeContact ?? string.Empty,
        Remarks = remarks ?? string.Empty
        // CreatedAt will be set automatically by the database
      };

      // attach source file name passed from controller (use the uploaded filename)
      entity.SourceFileName = sourceFileName ?? string.Empty;

      // Validate entity
      var context = new ValidationContext(entity);
      var results = new List<ValidationResult>();
      bool isValid = Validator.TryValidateObject(entity, context, results, true);

      if (!isValid)
      {
        foreach (var error in results)
        {
          // Log row number and error message
          validationErrors.Add($"Row {rowIdx}: {error.ErrorMessage}");
        }
        continue; // Skip invalid row
      }

      // Upsert logic
      var dbEntity = _context.Set<SewaNominalRoll>()
        .SingleOrDefault(x => x.NominalRollToken == nominalRollToken && x.SewaDate == sewaDate);

      if (dbEntity == null)
      {
        _context.Set<SewaNominalRoll>().Add(entity);
      }
      else
      {
        // Update relevant fields, using null-coalescing for strings
        dbEntity.SewaName = sewaName ?? string.Empty;
        dbEntity.Department = department ?? string.Empty;
        dbEntity.Zone = zone ?? string.Empty;
        dbEntity.CentreName = centreName ?? string.Empty;
        dbEntity.JourneyDate = journeyDate;
        dbEntity.SewaDuration = sewaDuration;
        dbEntity.Males = males;
        dbEntity.Females = females;
        dbEntity.TotalSewadars = totalSewadars;
        dbEntity.SewaType = sewaType ?? string.Empty;
        dbEntity.SewaStartTime = sewaStartTime ?? string.Empty;
        dbEntity.InchargeName = inchargeName ?? string.Empty;
        dbEntity.InchargeContact = inchargeContact ?? string.Empty;
        dbEntity.Remarks = remarks ?? string.Empty;
        // update source file name if reprocessed
        dbEntity.SourceFileName = sourceFileName ?? string.Empty;
      }
    }

    // Optionally, log or return these errors for user feedback
    if (validationErrors.Any())
    {
      foreach (var error in validationErrors)
      {
        _logger.LogWarning(error);
      }
    }
    await _context.SaveChangesAsync();
  }

  public async Task<IEnumerable<SewaNominalRoll>> Query(DateTime? startDate, DateTime? endDate, string? centerName, string? sewaType)
  {
    var query = _context.SewaNominalRolls.AsQueryable();

    if (startDate.HasValue)
    {
      query = query.Where(x => x.SewaDate >= startDate.Value.Date);
    }
    if (endDate.HasValue)
    {
      // include entire day
      query = query.Where(x => x.SewaDate <= endDate.Value.Date.AddDays(1).AddTicks(-1));
    }
    if (!string.IsNullOrWhiteSpace(centerName) && centerName != "__all__")
    {
      query = query.Where(x => EF.Functions.Like(x.CentreName, $"%{centerName}%"));
    }
    if (!string.IsNullOrWhiteSpace(sewaType) && sewaType != "__all__")
    {
      query = query.Where(x => EF.Functions.Like(x.SewaType, $"%{sewaType}%"));
    }

    return await query.OrderByDescending(x => x.SewaDate).ToListAsync();
  }

  public async Task<IEnumerable<string>> GetDistinctSourceFilesAsync()
  {
    return await _context.SewaNominalRolls
      .Select(x => x.SourceFileName)
      .Where(x => x != null && x != string.Empty)
      .Distinct()
      .OrderBy(x => x)
      .ToListAsync();
  }

  public async Task DeleteBySourceFileAsync(string sourceFileName)
  {
    if (string.IsNullOrWhiteSpace(sourceFileName)) return;

    var items = await _context.SewaNominalRolls
      .Where(x => x.SourceFileName == sourceFileName)
      .ToListAsync();

    if (items.Any())
    {
      _context.SewaNominalRolls.RemoveRange(items);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<IEnumerable<string>> GetDistinctCentersAsync()
  {
    return await _context.SewaNominalRolls
      .Select(x => x.CentreName)
      .Where(x => x != null && x != string.Empty)
      .Distinct()
      .OrderBy(x => x)
      .ToListAsync();
  }

  public async Task<IEnumerable<string>> GetDistinctSewaTypesAsync()
  {
    return await _context.SewaNominalRolls
      .Select(x => x.SewaType)
      .Where(x => x != null && x != string.Empty)
      .Distinct()
      .OrderBy(x => x)
      .ToListAsync();
  }

  private static DateTime ParseDate(string value)
  {
    DateTime result;
    if (!DateTime.TryParse(value, out result))
    {
      result = DateTime.MinValue;
    }
    return result;
  }

  private static int ParseInt(string value)
  {
    int result;
    if (!int.TryParse(value, out result))
    {
      result = 0;
    }
    return result;
  }
}
