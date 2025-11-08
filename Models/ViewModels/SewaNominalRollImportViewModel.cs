using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Models.ViewModels;

public class SewaNominalRollImportViewModel
{
  public IEnumerable<SewaNominalRoll> Results { get; set; } = Enumerable.Empty<SewaNominalRoll>();
  public List<string> Centers { get; set; } = new List<string>();
  public List<string> SewaTypes { get; set; } = new List<string>();
  public List<string> SourceFiles { get; set; } = new List<string>();

  // List of NominalRollTokens to highlight (present in DailyActualCount for same date)
  public HashSet<string> HighlightedTokens { get; set; } = new HashSet<string>();

  // Filters
  public string? CenterFilter { get; set; }
  public string? SewaTypeFilter { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
}
