using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class SqlSalesRepository : ISalesRepository
	{
		public List<Sales> GetAll()
		{
			var sales = new List<Sales>();

			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();

			// Reolen findes via varen, som hører til salget
			using var command = new SqlCommand(@"
                SELECT s.SaleID, s.ItemID, s.ItemName, s.ItemPrice, s.PaymentID,
                       s.SaleDate, s.EmployeeID, i.ShelfID
                FROM Sales s
                LEFT JOIN Items i ON s.ItemID = i.ItemID", connection);
			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				sales.Add(new Sales
				{
					SaleID = reader.GetString(0),
					ItemID = reader.GetString(1),
					ItemName = reader.GetString(2),
					ItemPrice = reader.GetDecimal(3),
					PaymentID = reader.GetString(4),
					SaleDate = reader.GetDateTime(5),
					EmployeeID = reader.GetString(6),
					ShelfID = reader.IsDBNull(7) ? null : reader.GetString(7)
				});
			}

			return sales;
		}

		public void SaveAll(IEnumerable<Sales> sales)
		{
			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();
			using var transaction = connection.BeginTransaction();

			foreach (var sale in sales)
			{
				using var command = new SqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM Sales WHERE SaleID = @SaleID)
                        INSERT INTO Sales (SaleID, ItemID, ItemName, ItemPrice, PaymentID, SaleDate, EmployeeID)
                        VALUES (@SaleID, @ItemID, @ItemName, @ItemPrice, @PaymentID, @SaleDate, @EmployeeID);

                    IF @ShelfID <> '' AND NOT EXISTS
                        (SELECT 1 FROM SalesShelves WHERE SaleID = @SaleID AND ShelfID = @ShelfID)
                        INSERT INTO SalesShelves (ShelfID, SaleID) VALUES (@ShelfID, @SaleID);",
					connection, transaction);

				command.Parameters.AddWithValue("@SaleID", sale.SaleID ?? string.Empty);
				command.Parameters.AddWithValue("@ItemID", sale.ItemID ?? string.Empty);
				command.Parameters.AddWithValue("@ItemName", sale.ItemName ?? string.Empty);
				command.Parameters.AddWithValue("@ItemPrice", sale.ItemPrice);
				command.Parameters.AddWithValue("@PaymentID", sale.PaymentID ?? string.Empty);
				command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
				command.Parameters.AddWithValue("@EmployeeID", sale.EmployeeID ?? string.Empty);
				command.Parameters.AddWithValue("@ShelfID", sale.ShelfID ?? string.Empty);
				command.ExecuteNonQuery();
			}

			transaction.Commit();
		}
	}
}