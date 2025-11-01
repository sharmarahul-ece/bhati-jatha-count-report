using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;
using bhati_jatha_count_report.Data;
using Microsoft.EntityFrameworkCore;

namespace bhati_jatha_count_report.Services.Implementations
{
  public class ExcludedForDayService : IExcludedForDayService
  {
    private readonly ApplicationDbContext _context;
    public ExcludedForDayService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<ExcludedForDay>> GetExcludedAsync(DateTime date)
    {
      var d = date.Date;
      return await _context.ExcludedForDays.Where(x => x.Date == d).ToListAsync();
    }

    public async Task<bool> IsExcludedAsync(int centerId, int sewaTypeId, DateTime date)
    {
      var d = date.Date;
      return await _context.ExcludedForDays.AnyAsync(x => x.CenterId == centerId && x.SewaTypeId == sewaTypeId && x.Date == d);
    }

    public async Task AddAsync(int centerId, int sewaTypeId, DateTime date)
    {
      var d = date.Date;
      if (!await IsExcludedAsync(centerId, sewaTypeId, d))
      {
        _context.ExcludedForDays.Add(new ExcludedForDay { CenterId = centerId, SewaTypeId = sewaTypeId, Date = d });
        await _context.SaveChangesAsync();
      }
    }

    public async Task RemoveAsync(int centerId, int sewaTypeId, DateTime date)
    {
      var d = date.Date;
      var entity = await _context.ExcludedForDays.FirstOrDefaultAsync(x => x.CenterId == centerId && x.SewaTypeId == sewaTypeId && x.Date == d);
      if (entity != null)
      {
        _context.ExcludedForDays.Remove(entity);
        await _context.SaveChangesAsync();
      }
    }
  }
}
