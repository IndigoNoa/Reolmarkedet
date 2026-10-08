using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Reolmarkedet.Models;

namespace Reolmarkedet.Repositories
{
	public class SqlMonthlyReportsRepository : IMonthlyReportsRepository
	{
		public List<MonthlyReport> GetAll()
		{
			var reports = new List<MonthlyReport>();

			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();

			using var command = new SqlCommand(@"
                SELECT ReportID, RenterID, ReportPeriod, Commission, Rent,
                       MultipleShelvesDiscount, RenterBalance, ReportNote, ReportStatus
                FROM MonthlyReports", connection);
			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				reports.Add(new MonthlyReport
				{
					ReportID = reader.GetString(0),
					RenterID = reader.GetString(1),
					ReportPeriod = reader.GetDateTime(2),
					Commission = reader.GetDecimal(3),
					Rent = reader.GetDecimal(4),
					MultipleShelvesDiscount = reader.GetDecimal(5),
					RenterBalance = reader.GetDecimal(6),
					ReportNote = reader.IsDBNull(7) ? null : reader.GetString(7),   // Notatet er valgfrit
					ReportStatus = reader.GetBoolean(8)
				});
			}

			return reports;
		}

		public void SaveAll(IEnumerable<MonthlyReport> reports)
		{
			using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
			connection.Open();
			using var transaction = connection.BeginTransaction();

			foreach (var report in reports)
			{
				using var command = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM MonthlyReports WHERE ReportID = @ReportID)
                        UPDATE MonthlyReports SET RenterID = @RenterID, ReportPeriod = @ReportPeriod,
                            Commission = @Commission, Rent = @Rent,
                            MultipleShelvesDiscount = @MultipleShelvesDiscount,
                            RenterBalance = @RenterBalance, ReportNote = @ReportNote,
                            ReportStatus = @ReportStatus
                        WHERE ReportID = @ReportID
                    ELSE
                        INSERT INTO MonthlyReports (ReportID, RenterID, ReportPeriod, Commission, Rent,
                            MultipleShelvesDiscount, RenterBalance, ReportNote, ReportStatus)
                        VALUES (@ReportID, @RenterID, @ReportPeriod, @Commission, @Rent,
                            @MultipleShelvesDiscount, @RenterBalance, @ReportNote, @ReportStatus)",
					connection, transaction);

				command.Parameters.AddWithValue("@ReportID", report.ReportID ?? string.Empty);
				command.Parameters.AddWithValue("@RenterID", report.RenterID ?? string.Empty);
				command.Parameters.AddWithValue("@ReportPeriod", report.ReportPeriod);
				command.Parameters.AddWithValue("@Commission", report.Commission);
				command.Parameters.AddWithValue("@Rent", report.Rent);
				command.Parameters.AddWithValue("@MultipleShelvesDiscount", report.MultipleShelvesDiscount);
				command.Parameters.AddWithValue("@RenterBalance", report.RenterBalance);
				command.Parameters.AddWithValue("@ReportNote", (object)report.ReportNote ?? DBNull.Value);
				command.Parameters.AddWithValue("@ReportStatus", report.ReportStatus);
				command.ExecuteNonQuery();
			}

			transaction.Commit();
		}
	}
}