using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Models
{
	// Holder styr på hvilken medarbejder der er logget ind, så alle ViewModels kan tilgå det
	public static class CurrentSession
	{
		public static string EmployeeName { get; set; }
	}
}
