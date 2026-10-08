using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;

namespace Reolmarkedet.ViewModels
{
	public class RenterReportStatusConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not string renterID) return "Ikke opgjort";
			var reportsRepository = new SqlMonthlyReportsRepository(); // SQL
			/*var reportsRepository = new JsonMonthlyReportsRepository();*/ // JSON
			bool isReported = reportsRepository.GetAll().Any(r => r.RenterID == renterID
				&& r.ReportPeriod.Month == DateTime.Now.Month
				&& r.ReportPeriod.Year == DateTime.Now.Year);

			return isReported ? "Opgjort" : "Ikke opgjort";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}