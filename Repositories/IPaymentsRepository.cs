using System.Collections.Generic;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public interface IPaymentsRepository
	{
		List<Payment> GetAll();
		void SaveAll(IEnumerable<Payment> payments);
	}
}