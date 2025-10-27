using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Services.Interfaces;

public interface ISewaTypeService
{
  Task<IEnumerable<SewaType>> GetAllSewaTypesAsync();
  Task<SewaType?> GetSewaTypeByIdAsync(int id);
  Task<SewaType> CreateSewaTypeAsync(SewaType sewaType);
  Task<SewaType?> UpdateSewaTypeAsync(int id, SewaType sewaType);
  Task<bool> DeleteSewaTypeAsync(int id);
  Task<bool> SewaTypeExistsAsync(int id);
  Task<bool> IsSewaNameUniqueAsync(string sewaName, int? excludeId = null);
}