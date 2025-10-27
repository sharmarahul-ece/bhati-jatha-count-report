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
    public string NominalRollToken { get; set; }
  }
}
