using System.Collections.Generic;
using System.Linq;
using bhati_jatha_count_report.Data;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bhati_jatha_count_report.Services.Implementations
{
  public class AllotedCountService : IAllotedCountService
  {
    private readonly ApplicationDbContext _context;

    public AllotedCountService(ApplicationDbContext context)
    {
      _context = context;
    }

    public IEnumerable<AllotedCount> GetAll()
    {
      return _context.AllotedCounts
        .Include(x => x.Center)
        .Include(x => x.SewaType)
        .ToList();
    }

    public AllotedCount? GetById(WeekDay weekDay, int centerId, int sewaTypeId)
    {
      return _context.AllotedCounts
        .Include(x => x.Center)
        .Include(x => x.SewaType)
        .FirstOrDefault(x => x.WeekDay == weekDay && x.CenterId == centerId && x.SewaTypeId == sewaTypeId);
    }

    public void Add(AllotedCount allotedCount)
    {
      _context.AllotedCounts.Add(allotedCount);
      _context.SaveChanges();
    }

    public void Update(AllotedCount allotedCount)
    {
      _context.AllotedCounts.Update(allotedCount);
      _context.SaveChanges();
    }

    public void Delete(WeekDay weekDay, int centerId, int sewaTypeId)
    {
      var entity = _context.AllotedCounts.FirstOrDefault(
        x => x.WeekDay == weekDay && x.CenterId == centerId && x.SewaTypeId == sewaTypeId);
      if (entity != null)
      {
        _context.AllotedCounts.Remove(entity);
        _context.SaveChanges();
      }
    }
  }
}