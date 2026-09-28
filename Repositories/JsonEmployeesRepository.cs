using System.Collections.Generic;      // List<T> og IEnumerable<T>
using System.IO;                       // File-klassen
using System.Text.Json;                // JSON-serialisering
using Reolmarkedet.Models;             // Employees-klassen

namespace Reolmarkedet.Repositories
{
	public class JsonEmployeesRepository : IEmployeesRepository
	{
		private readonly string _filepath;   // Sti til JSON-filen

		public JsonEmployeesRepository() : this("Data/employees.json") { }   // Standard-sti

		public JsonEmployeesRepository(string filepath)   // Egen sti (nyttig til test)
		{
			_filepath = filepath;
		}

		public List<Employees> GetAll()
		{
			if (!File.Exists(_filepath))                   // Ingen fil endnu?
				return new List<Employees>();              // Så returneres en tom liste

			string json = File.ReadAllText(_filepath);     // Læser filens tekst
			return JsonSerializer.Deserialize<List<Employees>>(json) ?? new List<Employees>();   // JSON -> objekter
		}

		public void SaveAll(IEnumerable<Employees> employees)
		{
			string json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });   // Objekter -> pæn JSON
			File.WriteAllText(_filepath, json);            // Overskriver filen
		}
	}
}