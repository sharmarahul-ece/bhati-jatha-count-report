using bhati_jatha_count_report.Data;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;
using ClosedXML.Excel;

namespace bhati_jatha_count_report.Services.Implementations;

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

    // Skip the first line (info), read headers from second line.
    // Data starts at row 3 (1-based index).
    var firstDataRow = 3;
    var lastDataRow = worksheet.LastRowUsed().RowNumber();

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

      // Upsert logic
      var entity = _context.Set<SewaNominalRoll>()
          .SingleOrDefault(x => x.NominalRollToken == nominalRollToken && x.SewaDate == sewaDate);
      if (entity == null)
      {
        entity = new SewaNominalRoll
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
          Remarks = remarks,
          CreatedAt = DateTime.UtcNow
        };
        _context.Set<SewaNominalRoll>().Add(entity);
      }
      else
      {
        entity.SewaName = sewaName;
        entity.Department = department;
        entity.Zone = zone;
        entity.CentreName = centreName;
        entity.JourneyDate = journeyDate;
        entity.SewaDuration = sewaDuration;
        entity.Males = males;
        entity.Females = females;
        entity.TotalSewadars = totalSewadars;
        entity.SewaType = sewaType;
        entity.SewaStartTime = sewaStartTime;
        entity.InchargeName = inchargeName;
        entity.InchargeContact = inchargeContact;
        entity.Remarks = remarks;
      }
    }
    await _context.SaveChangesAsync();
  }

  private static DateTime ParseDate(string value)
  {
    DateTime result;
    // Try standard ISO or custom formats; fallback to DateTime.MinValue if invalid
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



public class SewaNominalRollImportService
{

}
