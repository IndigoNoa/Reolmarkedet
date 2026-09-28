using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class JsonShelvesRepository : IShelvesRepository
	{
		private readonly string _filepath;   // Sti til JSON-filen

		public JsonShelvesRepository() : this("Data/shelves.json") { }   // Standard-sti

		public JsonShelvesRepository(string filepath)
		{
			_filepath = filepath;
		}

		public List<Shelves> GetAll()
		{
			if (!File.Exists(_filepath))
				return new List<Shelves>();

			string json = File.ReadAllText(_filepath);
			return JsonSerializer.Deserialize<List<Shelves>>(json) ?? new List<Shelves>();
		}

		public void SaveAll(IEnumerable<Shelves> shelves)
		{
			string json = JsonSerializer.Serialize(shelves, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(_filepath, json);
		}
	}
}