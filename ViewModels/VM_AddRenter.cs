using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using Reolmarkedet.Views;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;

namespace Reolmarkedet.ViewModels
{
    public class VM_AddRenter : ViewModelBase
    {
		// Backing fields
		private string _renterID;
		private string _renterName;
		private string _renterEmail;
		private string _renterAddress;
		private int _renterShelvesAmount;
		private string _renterPhone;
		private string _shelfType;
		private string _shelfStatus;
		private List<Shelves> _shelfList;
		private decimal _amountToPay;
		private int _renterShelfAmount;
		private PaymentMethod _paymentMethod;
		private DateTime _addRenterDate;

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

		public string RenterEmail
		{
			get => _renterEmail;
			set => SetProperty(ref _renterEmail, value);
		}

		public string RenterAddress
		{
			get => _renterAddress;
			set => SetProperty(ref _renterAddress, value);
		}

		public int RenterShelvesAmount
		{
			get => _renterShelvesAmount;
			set => SetProperty(ref _renterShelvesAmount, value);
		}

		public string RenterPhone
		{
			get => _renterPhone;
			set => SetProperty(ref _renterPhone, value);
		}

		public string ShelfType
		{
			get => _shelfType;
			set => SetProperty(ref _shelfType, value);
		}

		public string ShelfStatus
		{
			get => _shelfStatus;
			set => SetProperty(ref _shelfStatus, value);
		}

		public List<Shelves> ShelfList
		{
			get => _shelfList;
			set => SetProperty(ref _shelfList, value);
		}

		public decimal AmountToPay
		{
			get => _amountToPay;
			set => SetProperty(ref _amountToPay, value);
		}

		public int RenterShelfAmount
		{
			get => _renterShelfAmount;
			set => SetProperty(ref _renterShelfAmount, value);
		}

		public PaymentMethod PaymentMethod
		{
			get => _paymentMethod;
			set => SetProperty(ref _paymentMethod, value);
		}

		public DateTime AddRenterDate
		{
			get => _addRenterDate;
			set => SetProperty(ref _addRenterDate, value);
		}

		public string ShelfIDsText => ShelfList != null ? string.Join(", ", ShelfList.Select(s => s.ShelfID)) : "";

		// JSON Repository til test af gemmelogik
		/*private readonly IRentersRepository _rentersRepository = new JsonRentersRepository();*/
		// SQL Repository
		private readonly IRentersRepository _rentersRepository = new SqlRentersRepository();

		// Commands
		public ICommand RenterNameInputCommand { get; }
		public ICommand RenterEmailInputCommand { get; }
		public ICommand RenterPhoneInputCommand { get; }
		public ICommand RenterAddressInputCommand { get; }
		public ICommand RenterShelvesAmountDropdownSelectionCommand { get; }
		public ICommand NextCommand { get; }
		public ICommand ConfirmCommand { get; }
		public ICommand HomeCommand { get; }
		public ICommand SelectPaymentMethodCommand { get; }

		public VM_AddRenter()
		{
			RenterNameInputCommand = new RelayCommand(ExecuteRenterNameInput);
			RenterEmailInputCommand = new RelayCommand(ExecuteRenterEmailInput);
			RenterPhoneInputCommand = new RelayCommand(ExecuteRenterPhoneInput);
			RenterAddressInputCommand = new RelayCommand(ExecuteRenterAddressInput);
			RenterShelvesAmountDropdownSelectionCommand = new RelayCommand(ExecuteRenterShelvesAmountDropdownSelection);
			NextCommand = new RelayCommand(ExecuteNext);
			ConfirmCommand = new RelayCommand(ExecuteConfirm);
			HomeCommand = new RelayCommand(ExecuteHome);
			SelectPaymentMethodCommand = new RelayCommand(param => ExecuteSelectPaymentMethod(param as string));
		}

		private void ExecuteRenterNameInput() { }
		private void ExecuteRenterEmailInput() { }
		private void ExecuteRenterPhoneInput() { }
		private void ExecuteRenterAddressInput() { }
		private void ExecuteRenterShelvesAmountDropdownSelection() { }
		private void ExecuteNext()
		{
			var oldWindow = Application.Current.Windows.OfType<AddRenterView>().FirstOrDefault();
			var selectShelfView = new SelectShelfForRenterView { DataContext = new VM_SelectShelfForRenter(this) };
			CopyWindowPosition(oldWindow, selectShelfView);
			selectShelfView.Show();
			oldWindow?.Close();
		}

		private void ExecuteSelectPaymentMethod(string method)
		{
			if (method == "Kontant")
				PaymentMethod = PaymentMethod.Kontant;
			else if (method == "MobilePay")
				PaymentMethod = PaymentMethod.MobilePay;

			ExecuteConfirm();   // Går direkte videre, ligesom i Checkout-flowet
		}



		// Kaldes fra VM_SelectShelfForRenter, når en reol er valgt (bygges færdig om lidt)
		public void GoToPaymentScreen()
		{
			RenterShelfAmount = ShelfList.Count;

			decimal pricePerShelf;
			if (RenterShelfAmount == 1)
				pricePerShelf = 850;
			else if (RenterShelfAmount <= 3)
				pricePerShelf = 825;
			else
				pricePerShelf = 800;

			AmountToPay = pricePerShelf * RenterShelfAmount;

			var oldWindow = Application.Current.Windows.OfType<SelectShelfForRenterView>().FirstOrDefault();
			var paymentView = new AddRenterPaymentView { DataContext = this };
			CopyWindowPosition(oldWindow, paymentView);
			paymentView.Show();
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


		private void ExecuteConfirm()
		{
			// Opretter lejeren
			var allRenters = _rentersRepository.GetAll();
			string newRenterID = "R_" + (allRenters.Count + 1).ToString("D2");

			var newRenter = new Renter
			{
				RenterID = newRenterID,
				RenterName = RenterName,
				RenterEmail = RenterEmail,
				RenterAddress = RenterAddress,
				RenterPhone = RenterPhone
			};

			allRenters.Add(newRenter);
			_rentersRepository.SaveAll(allRenters);

			RenterID = newRenterID;
			AddRenterDate = DateTime.Now;

			// Markerer de valgte reoler som udlejet til den nye lejer
			/*var shelvesRepository = new JsonShelvesRepository();*/ // Json
			var shelvesRepository = new SqlShelvesRepository(); // SQL
			var allShelves = shelvesRepository.GetAll();

			foreach (var selectedShelf in ShelfList)
			{
				var matchingShelf = allShelves.FirstOrDefault(s => s.ShelfID == selectedShelf.ShelfID);
				if (matchingShelf != null)
				{
					matchingShelf.RenterID = newRenterID;
					matchingShelf.ShelfStatus = "Udlejet";
					matchingShelf.RentalStartDate = DateTime.Now;
				}
			}

			shelvesRepository.SaveAll(allShelves);

			// Navigerer videre til bekræftelsesskærmen
			var oldWindow = Application.Current.Windows.OfType<AddRenterPaymentView>().FirstOrDefault();
			var confirmedView = new RenterCreatedView { DataContext = this };
			CopyWindowPosition(oldWindow, confirmedView);
			confirmedView.Show();
			oldWindow?.Close();
		}
		private void ExecuteHome()
		{
			var oldWindow = Application.Current.Windows.OfType<RenterCreatedView>().FirstOrDefault();

			var menuView = new MenuView { DataContext = new VM_MenuView() };
			menuView.Show();
			oldWindow?.Close();
		}
	}
}
