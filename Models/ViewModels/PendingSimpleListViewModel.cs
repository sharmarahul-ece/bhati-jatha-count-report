using System;
using System.Collections.Generic;
using System.Linq;
using bhati_jatha_count_report.Models.Entities;

namespace bhati_jatha_count_report.Models.ViewModels
{
  public class PendingSimpleListViewModel
  {
    public DateTime SelectedDate { get; set; }
    public List<PendingItem> Pending { get; set; } = new();

    public class PendingItem
    {
      public string CenterName { get; set; } = string.Empty;
      public string SewaTypeName { get; set; } = string.Empty;
    }
  }
}
