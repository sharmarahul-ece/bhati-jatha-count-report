using Microsoft.EntityFrameworkCore;
using bhati_jatha_count_report.Data;
using bhati_jatha_count_report.Models.Entities;
using bhati_jatha_count_report.Services.Interfaces;

namespace bhati_jatha_count_report.Services.Implementations;

public class SewaTypeService : ISewaTypeService
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<SewaTypeService> _logger;

  public SewaTypeService(ApplicationDbContext context, ILogger<SewaTypeService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task<IEnumerable<SewaType>> GetAllSewaTypesAsync()
  {
    try
    {
      return await _context.SewaTypes
          .OrderBy(s => s.SewaName)
          .ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while retrieving all sewa types");
      throw;
    }
  }

  public async Task<SewaType?> GetSewaTypeByIdAsync(int id)
  {
    try
    {
      return await _context.SewaTypes.FindAsync(id);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while retrieving sewa type with ID: {SewaTypeId}", id);
      throw;
    }
  }

  public async Task<SewaType> CreateSewaTypeAsync(SewaType sewaType)
  {
    if (sewaType == null)
    {
      throw new ArgumentNullException(nameof(sewaType));
    }

    try
    {
      // Validate sewa name uniqueness
      if (!await IsSewaNameUniqueAsync(sewaType.SewaName))
      {
        throw new InvalidOperationException($"A sewa type with the name '{sewaType.SewaName}' already exists.");
      }

      await _context.SewaTypes.AddAsync(sewaType);
      await _context.SaveChangesAsync();

      return sewaType;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while creating new sewa type: {SewaName}", sewaType.SewaName);
      throw;
    }
  }

  public async Task<SewaType?> UpdateSewaTypeAsync(int id, SewaType sewaType)
  {
    if (sewaType == null)
    {
      throw new ArgumentNullException(nameof(sewaType));
    }

    try
    {
      if (id != sewaType.Id)
      {
        return null;
      }

      // Validate sewa name uniqueness (excluding current sewa type)
      if (!await IsSewaNameUniqueAsync(sewaType.SewaName, sewaType.Id))
      {
        throw new InvalidOperationException($"A sewa type with the name '{sewaType.SewaName}' already exists.");
      }

      _context.Entry(sewaType).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return sewaType;
    }
    catch (DbUpdateConcurrencyException ex)
    {
      if (!await SewaTypeExistsAsync(id))
      {
        return null;
      }
      _logger.LogError(ex, "Concurrency error occurred while updating sewa type with ID: {SewaTypeId}", id);
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while updating sewa type with ID: {SewaTypeId}", id);
      throw;
    }
  }

  public async Task<bool> DeleteSewaTypeAsync(int id)
  {
    try
    {
      var sewaType = await _context.SewaTypes.FindAsync(id);
      if (sewaType == null)
      {
        return false;
      }

      _context.SewaTypes.Remove(sewaType);
      await _context.SaveChangesAsync();

      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while deleting sewa type with ID: {SewaTypeId}", id);
      throw;
    }
  }

  public async Task<bool> SewaTypeExistsAsync(int id)
  {
    return await _context.SewaTypes.AnyAsync(s => s.Id == id);
  }

  public async Task<bool> IsSewaNameUniqueAsync(string sewaName, int? excludeId = null)
  {
    var query = _context.SewaTypes.Where(s => s.SewaName == sewaName);

    if (excludeId.HasValue)
    {
      query = query.Where(s => s.Id != excludeId);
    }

    return !await query.AnyAsync();
  }
}