using System;
using System.ComponentModel.DataAnnotations;

namespace bhati_jatha_count_report.Models.ViewModels
{
  public class DailyActualCountViewModel
  {
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int CenterId { get; set; }

    [Required]
    public int SewaTypeId { get; set; }

    [Required]
    public int Count { get; set; }

    [Required]
    public string? NominalRollToken { get; set; }

    // New fields for nominal roll integration and manual override
    public bool NominalRollFound { get; set; }
    public int? NominalRollSewadarCount { get; set; }
    public int? ManualSewadarCount { get; set; }
    public int DisplaySewadarCount => NominalRollFound ? (NominalRollSewadarCount ?? 0) : (ManualSewadarCount ?? 0);
  }
}
