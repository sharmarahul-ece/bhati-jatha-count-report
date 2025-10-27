using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Services.Interfaces;

public interface ISewaNominalRollService
{
  Task Import(string excelPath);
}