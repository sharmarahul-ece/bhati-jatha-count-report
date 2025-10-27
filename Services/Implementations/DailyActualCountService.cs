using System;
using System.Collections.Generic;
using System.Linq;
using bhati_jatha_count_report.Data;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;

namespace bhati_jatha_count_report.Services.Implementations
{
  public class DailyActualCountService : IDailyActualCountService
  {
    private readonly ApplicationDbContext _context;

    public DailyActualCountService(ApplicationDbContext context)
    {
      _context = context;
    }

    public IEnumerable<DailyActualCount> GetAll()
    {
      return _context.DailyActualCounts.ToList();
    }

    public IEnumerable<DailyActualCount> GetFiltered(DateTime? fromDate, DateTime? toDate, int? centerId, int? sewaTypeId)
    {
      var query = _context.DailyActualCounts.AsQueryable();

      if (fromDate.HasValue)
      {
        query = query.Where(x => x.Date >= fromDate.Value.Date);
      }

      if (toDate.HasValue)
      {
        query = query.Where(x => x.Date <= toDate.Value.Date);
      }

      if (centerId.HasValue)
      {
        query = query.Where(x => x.CenterId == centerId.Value);
      }

      if (sewaTypeId.HasValue)
      {
        query = query.Where(x => x.SewaTypeId == sewaTypeId.Value);
      }

      return query.ToList();
    }

    public DailyActualCount? GetById(int id)
    {
      return _context.DailyActualCounts.FirstOrDefault(x => x.Id == id);
    }

    public void Add(DailyActualCount dailyActualCount)
    {
      _context.DailyActualCounts.Add(dailyActualCount);
      _context.SaveChanges();
    }

    public void Update(DailyActualCount dailyActualCount)
    {
      _context.DailyActualCounts.Update(dailyActualCount);
      _context.SaveChanges();
    }

    public void Delete(int id)
    {
      var entity = _context.DailyActualCounts.FirstOrDefault(x => x.Id == id);
      if (entity != null)
      {
        _context.DailyActualCounts.Remove(entity);
        _context.SaveChanges();
      }
    }
  }
}
