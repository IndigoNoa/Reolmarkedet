using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class SqlEmployeesRepository : IEmployeesRepository
	{
		public List<Employees> GetAll()
		{
			var employees = new List<Employees>();

			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();

			using var command = new SqlCommand(
				"SELECT EmployeeID, EmployeePassword, EmployeeName FROM Employees", connection);
			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				employees.Add(new Employees
				{
					EmployeeID = reader.GetString(0),
					EmployeePassword = reader.GetString(1),
					EmployeeName = reader.IsDBNull(2) ? null : reader.GetString(2)   // Navn må være tomt i databasen
				});
			}

			return employees;
		}

		public void SaveAll(IEnumerable<Employees> employees)
		{
			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();
			using var transaction = connection.BeginTransaction();

			foreach (var employee in employees)
			{
				using var command = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM Employees WHERE EmployeeID = @EmployeeID)
                        UPDATE Employees SET EmployeePassword = @EmployeePassword, EmployeeName = @EmployeeName
                        WHERE EmployeeID = @EmployeeID
                    ELSE
                        INSERT INTO Employees (EmployeeID, EmployeePassword, EmployeeName)
                        VALUES (@EmployeeID, @EmployeePassword, @EmployeeName)",
					connection, transaction);

				command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID ?? string.Empty);
				command.Parameters.AddWithValue("@EmployeePassword", employee.EmployeePassword ?? string.Empty);
				command.Parameters.AddWithValue("@EmployeeName", (object)employee.EmployeeName ?? DBNull.Value);
				command.ExecuteNonQuery();
			}

			transaction.Commit();
		}
	}
}