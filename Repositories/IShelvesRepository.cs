using System.Collections.Generic;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public interface IShelvesRepository
	{
		List<Shelves> GetAll();
		void SaveAll(IEnumerable<Shelves> shelves);
	}
}