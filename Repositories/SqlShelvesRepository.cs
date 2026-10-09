using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class SqlShelvesRepository : IShelvesRepository
	{
		public List<Shelves> GetAll()
		{
			var shelves = new List<Shelves>();

			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();

			using var command = new SqlCommand(
				"SELECT ShelfID, RenterID, ShelfType, ShelfStatus, RentalStartDate, CancellationDate FROM Shelves",
				connection);
			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				shelves.Add(new Shelves
				{
					ShelfID = reader.GetString(0),
					RenterID = reader.IsDBNull(1) ? null : reader.GetString(1),
					ShelfType = TypeNumberToText(reader.GetInt32(2)),   // Tal i databasen, tekst i koden
					ShelfStatus = reader.GetString(3),
					RentalStartDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
					CancellationDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
				});
			}

			return shelves
			.OrderBy(s => int.TryParse(s.ShelfID, out int n) ? n : int.MaxValue) // Sikrer korrekt rækkefølge af nummerering af I reoloversigt
			.ToList();
		}

		public void SaveAll(IEnumerable<Shelves> shelves)
		{
			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();
			using var transaction = connection.BeginTransaction();

			foreach (var shelf in shelves)
			{
				using var command = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM Shelves WHERE ShelfID = @ShelfID)
                        UPDATE Shelves SET RenterID = @RenterID, ShelfType = @ShelfType,
                            ShelfStatus = @ShelfStatus, RentalStartDate = @RentalStartDate,
                            CancellationDate = @CancellationDate
                        WHERE ShelfID = @ShelfID
                    ELSE
                        INSERT INTO Shelves (ShelfID, RenterID, ShelfType, ShelfStatus, RentalStartDate, CancellationDate)
                        VALUES (@ShelfID, @RenterID, @ShelfType, @ShelfStatus, @RentalStartDate, @CancellationDate)",
					connection, transaction);

				command.Parameters.AddWithValue("@ShelfID", shelf.ShelfID ?? string.Empty);
				// En tom lejer skal gemmes som NULL, ellers fejler fremmednøglen til Renter
				command.Parameters.AddWithValue("@RenterID",
					string.IsNullOrEmpty(shelf.RenterID) ? DBNull.Value : (object)shelf.RenterID);
				command.Parameters.AddWithValue("@ShelfType", TypeTextToNumber(shelf.ShelfType));
				command.Parameters.AddWithValue("@ShelfStatus", shelf.ShelfStatus ?? string.Empty);
				command.Parameters.AddWithValue("@RentalStartDate", (object)shelf.RentalStartDate ?? DBNull.Value);
				command.Parameters.AddWithValue("@CancellationDate", (object)shelf.CancellationDate ?? DBNull.Value);
				command.ExecuteNonQuery();
			}

			transaction.Commit();
		}

		// 1 = Standard. Ukendte tal vises som selve tallet
		private static string TypeNumberToText(int number)
		{
			return number == 1 ? "Standard" : number.ToString();
		}

		private static int TypeTextToNumber(string text)
		{
			if (text == "Standard") return 1;
			return int.TryParse(text, out int number) ? number : 1;
		}
	}
}