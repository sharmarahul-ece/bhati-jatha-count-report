using System.ComponentModel.DataAnnotations;

namespace bhati_jatha_count_report.Models.ViewModels;

public class SewaTypeViewModel
{
  [Required(ErrorMessage = "Sewa Name is required")]
  public string SewaName { get; set; } = string.Empty;
}

public class SewaTypeUpdateViewModel : SewaTypeViewModel
{
  public int Id { get; set; }
}