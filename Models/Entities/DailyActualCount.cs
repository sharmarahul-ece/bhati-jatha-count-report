using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bhati_jatha_count_report.Models.Entities
{
  public class DailyActualCount
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }


    [Required]
    [ForeignKey("Center")]
    public int CenterId { get; set; }
    public Center? Center { get; set; }

    [Required]
    [ForeignKey("SewaType")]
    public int SewaTypeId { get; set; }
    public SewaType? SewaType { get; set; }

    [Required]
    public int Count { get; set; }

    [Required]
    public string? NominalRollToken { get; set; }

    // Allows manual override of sewadars count if nominal roll not found
    public int? ManualSewadarCount { get; set; }
  }
}
