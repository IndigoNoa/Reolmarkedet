using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.ViewModels
{
	// Én række i lejerlisten
	public class RenterListItem
	{
		public string RenterID { get; set; }
		public string RenterName { get; set; }
		public string ShelfNumbers { get; set; }

		// Teksten der vises i listen. Uden reoler vises kun navnet
		public string Display => string.IsNullOrEmpty(ShelfNumbers)
			? RenterName
			: $"{RenterName} – Reol {ShelfNumbers}";
	}
}