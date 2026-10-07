using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;
using System.Linq;
using System.Windows;
using Reolmarkedet.Views;

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
			CreateReportCommand = new RelayCommand(param => ExecuteCreateReport(param as Renter));
			ShowReportCommand = new RelayCommand(ExecuteShowReport);
			HomeCommand = new RelayCommand(ExecuteHome);
			NextCommand = new RelayCommand(ExecuteNext);
			ConfirmReportCommand = new RelayCommand(ExecuteConfirmReport);
			ReportOverviewCommand = new RelayCommand(ExecuteReportOverview);
			ExecuteLoadRenter();
		}

		// Execute-metoder til commands 
		private void ExecuteCreateReport(Renter selectedRenter)
		{
			if (selectedRenter == null) return;

			RenterID = selectedRenter.RenterID;
			RenterName = selectedRenter.RenterName;
			ReportPeriod = DateTime.Now;

			ExecuteLoadSales();
			ExecuteLoadItems();
			CalculateRenterBalance();
			var oldWindow = Application.Current.Windows.OfType<MonthlyReportView>().FirstOrDefault();
			var detailView = new MonthlyReportDetailView { DataContext = this };
			CopyWindowPosition(oldWindow, detailView);
			detailView.Show();
			oldWindow?.Close();
		}

		private void ExecuteShowReport() { }
		private void ExecuteHome()
		{
			var oldWindow = Application.Current.Windows.OfType<Window>()
				.FirstOrDefault(w => w is MonthlyReportView || w is MonthlyReportDetailView || w is MonthlyReportPreviewView || w is MonthlyReportConfirmedView);

			var menuView = new MenuView { DataContext = new VM_MenuView() };
			menuView.Show();
			oldWindow?.Close();
		}
		private void ExecuteNext()
		{
			var oldWindow = Application.Current.Windows.OfType<MonthlyReportDetailView>().FirstOrDefault();
			var previewView = new MonthlyReportPreviewView { DataContext = this };
			CopyWindowPosition(oldWindow, previewView);
			previewView.Show();
			oldWindow?.Close();
		}
		private void ExecuteConfirmReport()
		{
			ExecuteSaveReport();

			var oldWindow = Application.Current.Windows.OfType<MonthlyReportPreviewView>().FirstOrDefault();
			var confirmedView = new MonthlyReportConfirmedView { DataContext = this };
			CopyWindowPosition(oldWindow, confirmedView);
			confirmedView.Show();
			oldWindow?.Close();
		}
		private void ExecuteReportOverview()
		{
			GoBackToReportList();
		}

		private void GoBackToReportList()
		{
			var oldWindow = Application.Current.Windows.OfType<Window>()
				.FirstOrDefault(w => w is MonthlyReportDetailView || w is MonthlyReportPreviewView || w is MonthlyReportConfirmedView);

			ExecuteLoadRenter();

			var reportListView = new MonthlyReportView { DataContext = this };
			CopyWindowPosition(oldWindow, reportListView);
			reportListView.Show();
			oldWindow?.Close();
		}

		private void CopyWindowPosition(Window oldWindow, Window newWindow)
		{
			if (oldWindow == null) return;
			newWindow.WindowStartupLocation = WindowStartupLocation.Manual;
			newWindow.Left = oldWindow.Left;
			newWindow.Top = oldWindow.Top;
			newWindow.Width = oldWindow.Width;
			newWindow.Height = oldWindow.Height;
			newWindow.WindowState = oldWindow.WindowState;
		}

		// Hjælpemetoder: indlæser data fra repositorie
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

		/*
		private string GetItemName(string itemID)
		{
			var item = _allItems.FirstOrDefault(i => i.ItemID == itemID);									// Død kode
			return item?.ItemName ?? "Ukendt vare";
		}

		private void ExecuteMonthlyReport()
		{
			// Selve beregningslogikken (kommission, leje, rabat, saldo) bygges i CreateMonthlyReport()		// Død Kode
		} */

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
			// Finder lejerens reoler
			var shelvesRepository = new JsonShelvesRepository();
			var renterShelfIDs = shelvesRepository.GetAll()
				.Where(s => s.RenterID == RenterID)
				.Select(s => s.ShelfID)
				.ToList();

			// Finder alle salg, der er sket på en af de reoler
			var renterSales = PeriodSales.Where(s => renterShelfIDs.Contains(s.ShelfID)).ToList();

			decimal totalSales = renterSales.Sum(s => s.ItemPrice);
			Commission = totalSales * 0.10m;   // 10%, jf. jeres Hi-Fi

			// Leje ud fra rabattrappen, baseret på antal reoler
			int shelfCount = renterShelfIDs.Count;
			decimal pricePerShelf = shelfCount == 1 ? 850
								   : shelfCount <= 3 ? 825
								   : 800;

			Rent = pricePerShelf * shelfCount;
			MultipleShelvesDiscount = shelfCount > 1 ? (850 - pricePerShelf) * shelfCount : 0;

			RenterBalance = totalSales - Commission - Rent;
		}
		
		// Metode, der tjekker vores "Opgjort/Ikke Opgjort logik", dog ville den primært blive brugt til efterudivkling, så dette er primært visuelt for at vise flow
		private bool IsRenterReported(string renterID)
		{
			var allReports = _monthlyReportsRepository.GetAll();
			return allReports.Any(r => r.RenterID == renterID
				&& r.ReportPeriod.Month == DateTime.Now.Month
				&& r.ReportPeriod.Year == DateTime.Now.Year);
		}

		// Beregnet property: bruges af XAML til at vise rigtig status-tekst
		public Func<string, string> GetReportStatusText => renterID => IsRenterReported(renterID) ? "Opgjort" : "Ikke opgjort";

	}

}
