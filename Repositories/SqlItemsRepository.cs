using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class SqlItemsRepository : IItemsRepository
	{
		public List<Items> GetAll()
		{
			var items = new List<Items>();

			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();

			using var command = new SqlCommand(
				"SELECT ItemID, ItemName, ItemPrice, ItemDescription, IsSold, Barcode, ShelfID FROM Items",
				connection);
			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				items.Add(new Items
				{
					ItemID = reader.GetString(0),
					ItemName = reader.GetString(1),
					ItemPrice = reader.GetDecimal(2),
					ItemDescription = reader.GetString(3),
					IsSold = reader.GetBoolean(4),
					Barcode = reader.GetInt32(5),
					ShelfID = reader.GetString(6)
				});
			}

			return items;
		}

		public void SaveAll(IEnumerable<Items> items)
		{
			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();
			using var transaction = connection.BeginTransaction();

			foreach (var item in items)
			{
				using var command = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM Items WHERE ItemID = @ItemID)
                        UPDATE Items SET ItemName = @ItemName, ItemPrice = @ItemPrice,
                            ItemDescription = @ItemDescription, IsSold = @IsSold,
                            Barcode = @Barcode, ShelfID = @ShelfID
                        WHERE ItemID = @ItemID
                    ELSE
                        INSERT INTO Items (ItemID, ItemName, ItemPrice, ItemDescription, IsSold, Barcode, ShelfID)
                        VALUES (@ItemID, @ItemName, @ItemPrice, @ItemDescription, @IsSold, @Barcode, @ShelfID)",
					connection, transaction);

				command.Parameters.AddWithValue("@ItemID", item.ItemID ?? string.Empty);
				command.Parameters.AddWithValue("@ItemName", item.ItemName ?? string.Empty);
				command.Parameters.AddWithValue("@ItemPrice", item.ItemPrice);
				command.Parameters.AddWithValue("@ItemDescription", item.ItemDescription ?? string.Empty);   // Kolonnen må ikke være tom (NULL)
				command.Parameters.AddWithValue("@IsSold", item.IsSold);
				command.Parameters.AddWithValue("@Barcode", item.Barcode);
				command.Parameters.AddWithValue("@ShelfID", item.ShelfID ?? string.Empty);
				command.ExecuteNonQuery();
			}

			transaction.Commit();
		}
	}
}