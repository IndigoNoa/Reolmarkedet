using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Models
{
	public class Employees
	{
		private string _employeeID; //Felt til medarbejderens ID, som bruges ved login
		private string _employeePassword; //Felt til medarbejderens adgangskode ved login

		public string EmployeeID //Giver adgang til medarbejderens ID
		{
			get { return _employeeID; }
			set { _employeeID = value; }
		}

		public string EmployeePassword //Giver adgang til medarbejderens adgangskode
		{
			get { return _employeePassword; }
			set { _employeePassword = value; }
		}

		public string EmployeeName { get; set; } // Med til at tilføje Medarbejder navn på kvittering
	}
}