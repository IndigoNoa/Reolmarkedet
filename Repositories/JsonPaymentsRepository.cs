using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class JsonPaymentsRepository : IPaymentsRepository
	{
		private readonly string _filepath;   // Sti til JSON-filen

		public JsonPaymentsRepository() : this("Data/payments.json") { }   // Standard-sti

		public JsonPaymentsRepository(string filepath)
		{
			_filepath = filepath;
		}

		public List<Payment> GetAll()
		{
			if (!File.Exists(_filepath))
				return new List<Payment>();

			string json = File.ReadAllText(_filepath);
			return JsonSerializer.Deserialize<List<Payment>>(json) ?? new List<Payment>();
		}

		public void SaveAll(IEnumerable<Payment> payments)
		{
			string json = JsonSerializer.Serialize(payments, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(_filepath, json);
		}
	}
}