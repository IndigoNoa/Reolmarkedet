using System.Collections.Generic;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public interface IMonthlyReportsRepository
	{
		List<MonthlyReport> GetAll();
		void SaveAll(IEnumerable<MonthlyReport> monthlyReports);
	}
}