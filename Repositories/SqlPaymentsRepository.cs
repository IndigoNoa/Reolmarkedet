using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class SqlPaymentsRepository : IPaymentsRepository
	{
		public List<Payment> GetAll()
		{
			var payments = new List<Payment>();

			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();

			using var command = new SqlCommand(
				"SELECT PaymentID, PaymentMethod, AmountPaid, PaymentDate FROM Payments", connection);
			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				payments.Add(new Payment
				{
					PaymentID = reader.GetString(0),
					PaymentMethod = Enum.TryParse<PaymentMethod>(reader.GetString(1), out var method)
						? method : PaymentMethod.Kontant,   // Tekst i databasen, enum i koden
					AmountPaid = reader.GetDecimal(2),
					PaymentDate = reader.GetDateTime(3)
				});
			}

			return payments;
		}

		public void SaveAll(IEnumerable<Payment> payments)
		{
			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();
			using var transaction = connection.BeginTransaction();

			foreach (var payment in payments)
			{
				using var command = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM Payments WHERE PaymentID = @PaymentID)
                        UPDATE Payments SET PaymentMethod = @PaymentMethod, AmountPaid = @AmountPaid,
                            PaymentDate = @PaymentDate
                        WHERE PaymentID = @PaymentID
                    ELSE
                        INSERT INTO Payments (PaymentID, PaymentMethod, AmountPaid, PaymentDate)
                        VALUES (@PaymentID, @PaymentMethod, @AmountPaid, @PaymentDate)",
					connection, transaction);

				command.Parameters.AddWithValue("@PaymentID", payment.PaymentID ?? string.Empty);
				command.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod.ToString());
				command.Parameters.AddWithValue("@AmountPaid", payment.AmountPaid);
				command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
				command.ExecuteNonQuery();
			}

			transaction.Commit();
		}
	}
}