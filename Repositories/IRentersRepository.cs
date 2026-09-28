using System;
using System.Collections.Generic;
using System.Text;
using Reolmarkedet.Models;              // Så vi kan bruge Renter-klassen


namespace Reolmarkedet.Repositories
{
	public interface IRentersRepository
	{
		List<Renter> GetAll();                          // Henter alle lejere
		void SaveAll(IEnumerable<Renter> renters);       // Gemmer hele listen af lejere ned i JSON-filen
	}
}
