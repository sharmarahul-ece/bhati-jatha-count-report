using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Services.Interfaces;

public interface ICenterService
{
  Task<IEnumerable<Center>> GetAllCentersAsync();
  Task<Center?> GetCenterByIdAsync(int id);
  Task<Center> CreateCenterAsync(Center center);
  Task<Center?> UpdateCenterAsync(int id, Center center);
  Task<bool> DeleteCenterAsync(int id);
  Task<bool> CenterExistsAsync(int id);
  Task<bool> IsCenterNameUniqueAsync(string centerName, int? excludeId = null);
  Task<IEnumerable<Center>> GetCentersByTypeAsync(string centerType);
}