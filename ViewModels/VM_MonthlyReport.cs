using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;

namespace Reolmarkedet.ViewModels
{
	public class VM_MonthlyReport : ViewModelBase
	{
		// Backing fields
		private string _renterID;
		private string _renterName;
		private bool _reportStatus;
		private DateTime _reportPeriod;
		private string _itemName;
		private string _itemID;
		private decimal _itemPrice;
		private string _paymentID;
		private decimal _commission;
		private decimal _rent;
		private string _reportNote;
		private decimal _renterBalance;
		private string _reportConfirmation;
		private string _reportID;
		private decimal _multipleShelvesDiscount;

		public string RenterID
		{
			get => _renterID;
			set => SetProperty(ref _renterID, value);
		}

		public string RenterName
		{
			get => _renterName;
			set => SetProperty(ref _renterName, value);
		}

		public bool ReportStatus
		{
			get => _reportStatus;
			set => SetProperty(ref _reportStatus, value);
		}

		public DateTime ReportPeriod
		{
			get => _reportPeriod;
			set => SetProperty(ref _reportPeriod, value);
		}

		public string ItemName
		{
			get => _itemName;
			set => SetProperty(ref _itemName, value);
		}

		public string ItemID
		{
			get => _itemID;
			set => SetProperty(ref _itemID, value);
		}

		public decimal ItemPrice
		{
			get => _itemPrice;
			set => SetProperty(ref _itemPrice, value);
		}

		public string PaymentID
		{
			get => _paymentID;
			set => SetProperty(ref _paymentID, value);
		}

		public decimal Commission
		{
			get => _commission;
			set => SetProperty(ref _commission, value);
		}

		public decimal Rent
		{
			get => _rent;
			set => SetProperty(ref _rent, value);
		}

		public string ReportNote
		{
			get => _reportNote;
			set => SetProperty(ref _reportNote, value);
		}

		public decimal RenterBalance
		{
			get => _renterBalance;
			set => SetProperty(ref _renterBalance, value);
		}

		public string ReportConfirmation
		{
			get => _reportConfirmation;
			set => SetProperty(ref _reportConfirmation, value);
		}

		public string ReportID
		{
			get => _reportID;
			set => SetProperty(ref _reportID, value);
		}

		public decimal MultipleShelvesDiscount
		{
			get => _multipleShelvesDiscount;
			set => SetProperty(ref _multipleShelvesDiscount, value);
		}

		// Lejerliste til oversigtsskærmen (Lejer, Status)
		public ObservableCollection<Renter> AllRenters { get; } = new ObservableCollection<Renter>();

		// Salg for perioden, vist i opgørelsen for den valgte lejer
		public ObservableCollection<Sales> PeriodSales { get; } = new ObservableCollection<Sales>();

		// Repositories: bruges af hjælpemetoderne til at hente/gemme data
		private readonly IRentersRepository _rentersRepository = new JsonRentersRepository();
		private readonly ISalesRepository _salesRepository = new JsonSalesRepository();
		private readonly IItemsRepository _itemsRepository = new JsonItemsRepository();
		private readonly IMonthlyReportsRepository _monthlyReportsRepository = new JsonMonthlyReportsRepository();

		// Commands
		public ICommand CreateReportCommand { get; }
		public ICommand ShowReportCommand { get; }
		public ICommand HomeCommand { get; }
		public ICommand NextCommand { get; }
		public ICommand ConfirmReportCommand { get; }
		public ICommand ReportOverviewCommand { get; }

		public VM_MonthlyReport()
		{
			CreateReportCommand = new RelayCommand(ExecuteCreateReport);
			ShowReportCommand = new RelayCommand(ExecuteShowReport);
			HomeCommand = new RelayCommand(ExecuteHome);
			NextCommand = new RelayCommand(ExecuteNext);
			ConfirmReportCommand = new RelayCommand(ExecuteConfirmReport);
			ReportOverviewCommand = new RelayCommand(ExecuteReportOverview);
		}

		// Execute-metoder til commands (tomme skeletter for nu)
		private void ExecuteCreateReport() { }
		private void ExecuteShowReport() { }
		private void ExecuteHome() { }
		private void ExecuteNext() { }
		private void ExecuteConfirmReport() { }
		private void ExecuteReportOverview() { }

		// Hjælpemetoder: indlæser data fra repositories
		private void ExecuteLoadRenter()
		{
			AllRenters.Clear();
			foreach (var renter in _rentersRepository.GetAll())
				AllRenters.Add(renter);
		}

		private void ExecuteLoadSales()
		{
			PeriodSales.Clear();
			foreach (var sale in _salesRepository.GetAll())
				PeriodSales.Add(sale);
		}

		private List<Items> _allItems = new List<Items>();

		private void ExecuteLoadItems()
		{
			_allItems = _itemsRepository.GetAll();
		}

		private string GetItemName(string itemID)
		{
			var item = _allItems.FirstOrDefault(i => i.ItemID == itemID);
			return item?.ItemName ?? "Ukendt vare";
		}

		private void ExecuteMonthlyReport()
		{
			// Selve beregningslogikken (kommission, leje, rabat, saldo) bygges i CreateMonthlyReport()
		}

		private void ExecuteSaveReport()
		{
			var allReports = _monthlyReportsRepository.GetAll();
			allReports.Add(CreateMonthlyReport());
			_monthlyReportsRepository.SaveAll(allReports);
		}

		// De to sidste metoder fra DCD'et (ingen "Execute"-præfiks i forvejen)
		private MonthlyReport CreateMonthlyReport()
		{
			var allReports = _monthlyReportsRepository.GetAll();
			string newReportID = "MR_" + (allReports.Count + 1).ToString("D2");

			return new MonthlyReport
			{
				ReportID = newReportID,
				RenterID = RenterID,
				ReportPeriod = ReportPeriod,
				Commission = Commission,
				Rent = Rent,
				MultipleShelvesDiscount = MultipleShelvesDiscount,
				RenterBalance = RenterBalance,   // Bliver 0 indtil CalculateRenterBalance er bygget færdig
				ReportNote = ReportNote,
				ReportStatus = true   // Sættes til opgjort, når rapporten gemmes
			};
		}

		private void CalculateRenterBalance()
		{
			// Finder alle salg for den valgte lejers reol(er) i perioden
			var renterSales = PeriodSales.Where(s => s.ShelfID == RenterID).ToList();
			// OBS: Dette antager RenterID == ShelfID, hvilket sjældent er rigtigt - se note nedenfor

			decimal totalSales = renterSales.Sum(s => s.ItemPrice);
			Commission = totalSales * 0.10m;   // 10% kommission, jf. jeres Hi-Fi

			RenterBalance = totalSales - Commission - Rent + MultipleShelvesDiscount;
		}

	}

}
