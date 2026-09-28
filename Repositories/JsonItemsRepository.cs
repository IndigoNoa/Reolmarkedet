using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class JsonItemsRepository : IItemsRepository
	{
		private readonly string _filepath;   // Sti til JSON-filen

		public JsonItemsRepository() : this("Data/items.json") { }   // Standard-sti

		public JsonItemsRepository(string filepath)
		{
			_filepath = filepath;
		}

		public List<Items> GetAll()
		{
			if (!File.Exists(_filepath))
				return new List<Items>();

			string json = File.ReadAllText(_filepath);
			return JsonSerializer.Deserialize<List<Items>>(json) ?? new List<Items>();
		}

		public void SaveAll(IEnumerable<Items> items)
		{
			string json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(_filepath, json);
		}
	}
}