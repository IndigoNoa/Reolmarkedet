using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Models
{
    public class Sales
    {
		public string SaleID { get; set; }       
		public string ItemID { get; set; }
		public string ItemName { get; set; }
		public string ShelfID { get; set; }       // Bruges til at finde frem til lejeren
		public decimal ItemPrice { get; set; }
		public string PaymentID { get; set; }
		public DateTime SaleDate { get; set; }
		public string EmployeeID { get; set; }
	}
}
