using System.ComponentModel.DataAnnotations;
using ClosedXML.Excel;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Data;
using bhati_jatha_count_report.Services.Interfaces;

public class SewaNominalRollService : ISewaNominalRollService
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<SewaNominalRollService> _logger;

  public SewaNominalRollService(ApplicationDbContext context, ILogger<SewaNominalRollService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Import(string excelPath)
  {
    using var workbook = new XLWorkbook(excelPath);
    var worksheet = workbook.Worksheets.First();
    var firstDataRow = 3;
    var lastDataRow = worksheet.LastRowUsed().RowNumber();

    var validationErrors = new List<string>();

    for (int rowIdx = firstDataRow; rowIdx <= lastDataRow; rowIdx++)
    {
      var row = worksheet.Row(rowIdx);

      var nominalRollToken = row.Cell(1).GetString().Trim();
      var sewaDate = ParseDate(row.Cell(8).GetString());
      var sewaName = row.Cell(3).GetString();
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
        SewaName = sewaName,
        Department = department,
        Zone = zone,
        CentreName = centreName,
        JourneyDate = journeyDate,
        SewaDuration = sewaDuration,
        Males = males,
        Females = females,
        TotalSewadars = totalSewadars,
        SewaType = sewaType,
        SewaStartTime = sewaStartTime,
        InchargeName = inchargeName,
        InchargeContact = inchargeContact,
        Remarks = remarks
        // CreatedAt will be set automatically by the database
      };

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
        // Update relevant fields
        dbEntity.SewaName = sewaName;
        dbEntity.Department = department;
        dbEntity.Zone = zone;
        dbEntity.CentreName = centreName;
        dbEntity.JourneyDate = journeyDate;
        dbEntity.SewaDuration = sewaDuration;
        dbEntity.Males = males;
        dbEntity.Females = females;
        dbEntity.TotalSewadars = totalSewadars;
        dbEntity.SewaType = sewaType;
        dbEntity.SewaStartTime = sewaStartTime;
        dbEntity.InchargeName = inchargeName;
        dbEntity.InchargeContact = inchargeContact;
        dbEntity.Remarks = remarks;
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
