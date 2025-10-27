using System.Collections.Generic;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Models.ViewModels
{
  public class AllotedCountPageViewModel
  {
    public List<AllotedCount> AllotedCounts { get; set; } = new();
    public List<Center> Centers { get; set; } = new();
    public List<SewaType> SewaTypes { get; set; } = new();
  }

  public class AllotedCountViewModel
  {
    public WeekDay WeekDay { get; set; }
    public int CenterId { get; set; }
    public int SewaTypeId { get; set; }
    public int Count { get; set; }
  }
}