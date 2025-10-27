using System.ComponentModel.DataAnnotations;

namespace bhati_jatha_count_report.Models.ViewModels;

public class CenterViewModel
{
  [Required(ErrorMessage = "Center Name is required")]
  public string CenterName { get; set; } = string.Empty;

  [Required(ErrorMessage = "Center Type is required")]
  public string CenterType { get; set; } = string.Empty;
}

public class CenterUpdateViewModel : CenterViewModel
{
  public int Id { get; set; }
}