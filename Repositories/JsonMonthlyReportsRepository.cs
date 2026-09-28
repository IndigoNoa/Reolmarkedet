using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class JsonMonthlyReportsRepository : IMonthlyReportsRepository
	{
		private readonly string _filepath;   // Sti til JSON-filen

		public JsonMonthlyReportsRepository() : this("Data/monthlyreports.json") { }   // Standard-sti

		public JsonMonthlyReportsRepository(string filepath)
		{
			_filepath = filepath;
		}

		public List<MonthlyReport> GetAll()
		{
			if (!File.Exists(_filepath))
				return new List<MonthlyReport>();

			string json = File.ReadAllText(_filepath);
			return JsonSerializer.Deserialize<List<MonthlyReport>>(json) ?? new List<MonthlyReport>();
		}

		public void SaveAll(IEnumerable<MonthlyReport> monthlyReports)
		{
			string json = JsonSerializer.Serialize(monthlyReports, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(_filepath, json);
		}
	}
}