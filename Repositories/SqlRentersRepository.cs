using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class SqlRentersRepository : IRentersRepository
	{
		public List<Renter> GetAll()
		{
			var renters = new List<Renter>();

			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();

			using var command = new SqlCommand(
				"SELECT RenterID, RenterName, RenterEmail, RenterPhone, RenterAddress FROM Renter", connection);
			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				renters.Add(new Renter
				{
					RenterID = reader.GetString(0),
					RenterName = reader.GetString(1),
					RenterEmail = reader.GetString(2),
					RenterPhone = reader.GetString(3),
					RenterAddress = reader.IsDBNull(4) ? null : reader.GetString(4)   // Adresse må være tom i databasen
				});
			}

			return renters;
		}

		public void SaveAll(IEnumerable<Renter> renters)
		{
			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();
			using var transaction = connection.BeginTransaction();   // Alt gemmes, eller ingenting

			foreach (var renter in renters)
			{
				// Opdaterer lejeren, hvis den findes, ellers oprettes den
				using var command = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM Renter WHERE RenterID = @RenterID)
                        UPDATE Renter SET RenterName = @RenterName, RenterEmail = @RenterEmail,
                            RenterPhone = @RenterPhone, RenterAddress = @RenterAddress
                        WHERE RenterID = @RenterID
                    ELSE
                        INSERT INTO Renter (RenterID, RenterName, RenterEmail, RenterPhone, RenterAddress)
                        VALUES (@RenterID, @RenterName, @RenterEmail, @RenterPhone, @RenterAddress)",
					connection, transaction);

				command.Parameters.AddWithValue("@RenterID", renter.RenterID ?? string.Empty);
				command.Parameters.AddWithValue("@RenterName", renter.RenterName ?? string.Empty);
				command.Parameters.AddWithValue("@RenterEmail", renter.RenterEmail ?? string.Empty);
				command.Parameters.AddWithValue("@RenterPhone", renter.RenterPhone ?? string.Empty);
				command.Parameters.AddWithValue("@RenterAddress", (object)renter.RenterAddress ?? DBNull.Value);
				command.ExecuteNonQuery();
			}

			transaction.Commit();
		}
	}
}