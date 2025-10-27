using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bhati_jatha_count_report.Models.Entities;

public class SewaNominalRoll
{
  [Key]
  public string NominalRollToken { get; set; } = string.Empty;

  [Required]
  public DateTime SewaDate { get; set; }

  [Required]
  public string SewaName { get; set; } = string.Empty;
  public string Department { get; set; } = string.Empty;
  public string Zone { get; set; } = string.Empty;
  public string CentreName { get; set; } = string.Empty;
  public DateTime JourneyDate { get; set; }
  public int SewaDuration { get; set; }
  public int Males { get; set; }
  public int Females { get; set; }
  public int TotalSewadars { get; set; }
  public string SewaType { get; set; } = string.Empty;
  public string SewaStartTime { get; set; } = string.Empty;
  public string InchargeName { get; set; } = string.Empty;
  public string InchargeContact { get; set; } = string.Empty;
  public string Remarks { get; set; } = string.Empty;

  [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
  public DateTime CreatedAt { get; set; }
}
