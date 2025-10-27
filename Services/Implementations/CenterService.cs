using Microsoft.EntityFrameworkCore;
using bhati_jatha_count_report.Data;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;

namespace bhati_jatha_count_report.Services.Implementations;

public class CenterService : ICenterService
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<CenterService> _logger;

  public CenterService(ApplicationDbContext context, ILogger<CenterService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<Center>> GetAllCentersAsync()
  {
    try
    {
      return await _context.Centers
          .OrderBy(c => c.CenterName)
          .ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while retrieving all centers");
      throw;
    }
  }

  public async Task<Center?> GetCenterByIdAsync(int id)
  {
    try
    {
      return await _context.Centers.FindAsync(id);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while retrieving center with ID: {CenterId}", id);
      throw;
    }
  }

  public async Task<Center> CreateCenterAsync(Center center)
  {
    if (center == null)
    {
      throw new ArgumentNullException(nameof(center));
    }

    try
    {
      // Validate center name uniqueness
      if (!await IsCenterNameUniqueAsync(center.CenterName))
      {
        throw new InvalidOperationException($"A center with the name '{center.CenterName}' already exists.");
      }

      await _context.Centers.AddAsync(center);
      await _context.SaveChangesAsync();

      return center;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while creating new center: {CenterName}", center.CenterName);
      throw;
    }
  }

  public async Task<Center?> UpdateCenterAsync(int id, Center center)
  {
    if (center == null)
    {
      throw new ArgumentNullException(nameof(center));
    }

    try
    {
      if (id != center.Id)
      {
        return null;
      }

      // Validate center name uniqueness (excluding current center)
      if (!await IsCenterNameUniqueAsync(center.CenterName, center.Id))
      {
        throw new InvalidOperationException($"A center with the name '{center.CenterName}' already exists.");
      }

      _context.Entry(center).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return center;
    }
    catch (DbUpdateConcurrencyException ex)
    {
      if (!await CenterExistsAsync(id))
      {
        return null;
      }
      _logger.LogError(ex, "Concurrency error occurred while updating center with ID: {CenterId}", id);
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while updating center with ID: {CenterId}", id);
      throw;
    }
  }

  public async Task<bool> DeleteCenterAsync(int id)
  {
    try
    {
      var center = await _context.Centers.FindAsync(id);
      if (center == null)
      {
        return false;
      }

      _context.Centers.Remove(center);
      await _context.SaveChangesAsync();

      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while deleting center with ID: {CenterId}", id);
      throw;
    }
  }

  public async Task<bool> CenterExistsAsync(int id)
  {
    return await _context.Centers.AnyAsync(c => c.Id == id);
  }

  public async Task<bool> IsCenterNameUniqueAsync(string centerName, int? excludeId = null)
  {
    var query = _context.Centers.Where(c => c.CenterName == centerName);

    if (excludeId.HasValue)
    {
      query = query.Where(c => c.Id != excludeId);
    }

    return !await query.AnyAsync();
  }

  public async Task<IEnumerable<Center>> GetCentersByTypeAsync(string centerType)
  {
    try
    {
      return await _context.Centers
          .Where(c => c.CenterType == centerType)
          .OrderBy(c => c.CenterName)
          .ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while retrieving centers of type: {CenterType}", centerType);
      throw;
    }
  }
}