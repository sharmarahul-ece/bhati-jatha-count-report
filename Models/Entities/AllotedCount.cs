using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bhati_jatha_count_report.Models.Entities
{
  public enum WeekDay
  {
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
  }

  public class AllotedCount
  {
    [Required]
    public WeekDay WeekDay { get; set; }

    [Required]
    public int CenterId { get; set; }

    [Required]
    public int SewaTypeId { get; set; }

    [Required]
    public int Count { get; set; }

    [ForeignKey("CenterId")]
    public Center Center { get; set; }

    [ForeignKey("SewaTypeId")]
    public SewaType SewaType { get; set; }
  }
}