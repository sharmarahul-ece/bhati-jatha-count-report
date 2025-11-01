using System;

namespace bhati_jatha_count_report.Models.Entities
{
  public class ExcludedForDay
  {
    public int CenterId { get; set; }
    public int SewaTypeId { get; set; }
    public DateTime Date { get; set; }

    // Navigation properties (optional, uncomment if needed)
    // public Center Center { get; set; }
    // public SewaType SewaType { get; set; }
  }
}