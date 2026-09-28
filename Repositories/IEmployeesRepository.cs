using Reolmarkedet.Models;
using System.Collections.Generic;

namespace Reolmarkedet.Repositories
{
	public interface IEmployeesRepository
	{
		List<Employees> GetAll();
		void SaveAll(IEnumerable<Employees> employees);
	}
}