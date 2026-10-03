using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Models
{
	public class MonthlyReport
	{
		public string ReportID { get; set; }
		public string RenterID { get; set; }
		public DateTime ReportPeriod { get; set; }
		public decimal Commission { get; set; }
		public decimal Rent { get; set; }
		public decimal MultipleShelvesDiscount { get; set; }
		public decimal RenterBalance { get; set; }
		public string ReportNote { get; set; }
		public bool ReportStatus { get; set; }
	}
}
