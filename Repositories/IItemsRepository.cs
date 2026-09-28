using System.Collections.Generic;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public interface IItemsRepository
	{
		List<Items> GetAll();
		void SaveAll(IEnumerable<Items> items);
	}
}