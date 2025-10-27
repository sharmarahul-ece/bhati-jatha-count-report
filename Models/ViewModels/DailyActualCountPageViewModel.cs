using System.Collections.Generic;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Models.ViewModels
{
  public class DailyActualCountPageViewModel
  {
    public List<DailyActualCount> DailyActualCounts { get; set; } = new();
    public List<Center> Centers { get; set; } = new();
    public List<SewaType> SewaTypes { get; set; } = new();
  }
}
