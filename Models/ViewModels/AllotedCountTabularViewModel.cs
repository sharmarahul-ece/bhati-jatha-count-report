using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Models.ViewModels
{
  public class AllotedCountTabularViewModel
  {
    public List<Center> Centers { get; set; } = new();
    public List<SewaType> SewaTypes { get; set; } = new();
    public List<AllotedCount> AllotedCounts { get; set; } = new();
    public int SelectedWeekDay { get; set; }
    public List<SelectListItem> WeekDays { get; set; } = new();

    public int? GetAllotedCount(int centerId, int sewaTypeId)
    {
      var ac = AllotedCounts.FirstOrDefault(x => (int)x.WeekDay == SelectedWeekDay && x.CenterId == centerId && x.SewaTypeId == sewaTypeId);
      return ac?.Count;
    }
  }
}
