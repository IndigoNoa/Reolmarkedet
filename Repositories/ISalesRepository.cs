using System.Collections.Generic;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public interface ISalesRepository
	{
		List<Sales> GetAll();
		void SaveAll(IEnumerable<Sales> sales);
	}
}