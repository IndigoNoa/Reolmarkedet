using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Models
{
	public class Items
	{
		public string ItemID { get; set; }
		public string ItemName { get; set; }
		public string ShelfID { get; set; }
		public int Barcode { get; set; }
		public decimal ItemPrice { get; set; }
		public string ItemDescription { get; set; }
		public bool IsSold { get; set; }
	}
}
