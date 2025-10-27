using System.Collections.Generic;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Services.Interfaces
{
  public interface IAllotedCountService
  {
    IEnumerable<AllotedCount> GetAll();
    AllotedCount? GetById(WeekDay weekDay, int centerId, int sewaTypeId);
    void Add(AllotedCount allotedCount);
    void Update(AllotedCount allotedCount);
    void Delete(WeekDay weekDay, int centerId, int sewaTypeId);
  }
}