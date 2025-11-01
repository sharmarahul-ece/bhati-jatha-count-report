using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Services.Interfaces
{
  public interface IExcludedForDayService
  {
    Task<List<ExcludedForDay>> GetExcludedAsync(DateTime date);
    Task<bool> IsExcludedAsync(int centerId, int sewaTypeId, DateTime date);
    Task AddAsync(int centerId, int sewaTypeId, DateTime date);
    Task RemoveAsync(int centerId, int sewaTypeId, DateTime date);
  }
}
