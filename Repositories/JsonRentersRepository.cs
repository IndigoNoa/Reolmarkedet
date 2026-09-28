using System.Collections.Generic;         // Giver adgang til List<T> og IEnumerable<T>
using System.IO;                          // Giver adgang til File-klassen (læs/skriv filer)
using System.Text.Json;                   // Giver adgang til JSON-serialisering (indbygget i .NET)
using Reolmarkedet.Models;                 // Så vi kan bruge Renter-klassen

namespace Reolmarkedet.Repositories
{
	public class JsonRentersRepository : IRentersRepository   // Implementerer interfacet, så den kan bruges hvor IRentersRepository forventes
	{
		private readonly string _filepath;   // Stien til JSON-filen, denne repository læser/skriver til

		public JsonRentersRepository() : this("Data/renters.json") // Parameterløs constructor - bruger en standard-sti
		{
		}

		public JsonRentersRepository(string filepath)  // Constructor hvor man selv kan angive stien (nyttig til test)
		{
			_filepath = filepath;                        // Gemmer stien til senere brug
		}

		public List<Renter> GetAll()                     // Henter alle lejere fra JSON-filen
		{
			if (!File.Exists(_filepath))                  // Tjekker om filen overhovedet findes endnu
				return new List<Renter>();                // Findes den ikke, returneres en tom liste i stedet for at crashe

			string json = File.ReadAllText(_filepath);     // Læser hele filens indhold som tekst
			return JsonSerializer.Deserialize<List<Renter>>(json) ?? new List<Renter>();
			// Omdanner JSON-teksten til en liste af Renter-objekter; hvis resultatet er null, bruges en tom liste i stedet
		}

		public void SaveAll(IEnumerable<Renter> renters)   // Gemmer en hel liste af lejere ned i JSON-filen
		{
			string json = JsonSerializer.Serialize(renters, new JsonSerializerOptions { WriteIndented = true });
			// Omdanner listen til JSON-tekst; WriteIndented gør filen læsbar for mennesker (pæn formatering)

			File.WriteAllText(_filepath, json);            // Overskriver filen med den nye JSON-tekst
		}
	}
}