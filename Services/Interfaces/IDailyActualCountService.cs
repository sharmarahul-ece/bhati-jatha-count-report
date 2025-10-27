using System;
using System.Collections.Generic;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Services.Interfaces
{
  public interface IDailyActualCountService
  {
    IEnumerable<DailyActualCount> GetAll();
    IEnumerable<DailyActualCount> GetFiltered(DateTime? fromDate, DateTime? toDate, int? centerId, int? sewaTypeId);
    DailyActualCount? GetById(int id);
    void Add(DailyActualCount dailyActualCount);
    void Update(DailyActualCount dailyActualCount);
    void Delete(int id);
  }
}
