using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Services.Interfaces;

public interface ISewaNominalRollService
{
  Task Import(string excelPath, string sourceFileName);

  Task<IEnumerable<SewaNominalRoll>> Query(DateTime? startDate, DateTime? endDate, string? centerName, string? sewaType);

  Task<IEnumerable<string>> GetDistinctCentersAsync();

  Task<IEnumerable<string>> GetDistinctSewaTypesAsync();

  Task<IEnumerable<string>> GetDistinctSourceFilesAsync();

  Task DeleteBySourceFileAsync(string sourceFileName);
}