using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using System.Linq;
using System.Windows;
using Reolmarkedet.Views;
using Reolmarkedet.Repositories;


namespace Reolmarkedet.ViewModels
{
	public class VM_Checkout : ViewModelBase
	{
		// Backing fields
		private string _itemID;
		private string _barCode;
		private string _itemName;
		private string _shelfID;
		private decimal _itemPrice;
		private decimal _amountToPay;
		private string _paymentID;
		private PaymentMethod _paymentMethod;
		private string _thankYouMessage;
		private DateTime _saleDate;
		private string _servedBy;
		private readonly IItemsRepository _itemsRepository = new JsonItemsRepository();

		public string ItemID
		{
			get => _itemID;
			set => SetProperty(ref _itemID, value);
		}

		public string BarCode
		{
			get => _barCode;
			set => SetProperty(ref _barCode, value);
		}

		public string ItemName
		{
			get => _itemName;
			set => SetProperty(ref _itemName, value);
		}

		public string ShelfID
		{
			get => _shelfID;
			set => SetProperty(ref _shelfID, value);
		}

		public decimal ItemPrice
		{
			get => _itemPrice;
			set => SetProperty(ref _itemPrice, value);
		}

		public decimal AmountToPay
		{
			get => _amountToPay;
			set => SetProperty(ref _amountToPay, value);
		}

		public string PaymentID
		{
			get => _paymentID;
			set => SetProperty(ref _paymentID, value);
		}

		public PaymentMethod PaymentMethod
		{
			get => _paymentMethod;
			set => SetProperty(ref _paymentMethod, value);
		}

		public string ThankYouMessage
		{
			get => _thankYouMessage;
			set => SetProperty(ref _thankYouMessage, value);

		}

		public DateTime SaleDate
		{
			get => _saleDate;
			set => SetProperty(ref _saleDate, value);
		}

		public string ServedBy
		{
			get => _servedBy;
			set => SetProperty(ref _servedBy, value);
		}

		// Kurven: de varer der er scannet/indtastet til dette køb
		public ObservableCollection<Items> ScannedCheckoutItems { get; } = new ObservableCollection<Items>(); // Observable collection, da en normal List ikke vil opdatere UI'et automatisk


		// Commands (Knapper)
		public ICommand GoToPaymentMethodCommand { get; }
		public ICommand PayCommand { get; }
		public ICommand PrintReceiptCommand { get; }
		public ICommand ReturnToCheckoutCommand { get; }
		public ICommand ScanBarcodeCommand { get; }
		public ICommand SelectPaymentMethodCommand { get; }


		public VM_Checkout()
		{
			GoToPaymentMethodCommand = new RelayCommand(ExecuteGoToPaymentMethod);
			PayCommand = new RelayCommand(ExecutePayment);
			PrintReceiptCommand = new RelayCommand(ExecutePrintReceipt);
			ReturnToCheckoutCommand = new RelayCommand(ExecuteReturnToCheckout);
			ScanBarcodeCommand = new RelayCommand(ExecuteScanBarcode);
			SelectPaymentMethodCommand = new RelayCommand(param => ExecuteSelectPaymentMethod(param as string));
		}

		// Execute-metoder (tomme skeletter for nu)
		private void ExecuteGoToPaymentMethod()
		{
			var oldWindow = Application.Current.Windows.OfType<CheckoutView>().FirstOrDefault();
			var paymentView = new PaymentMethodView { DataContext = this };
			CopyWindowPosition(oldWindow, paymentView);
			paymentView.Show();
			oldWindow?.Close();
		}
		private void ExecutePayment()
		{
			var oldWindow = Application.Current.Windows.OfType<PaymentMethodView>().FirstOrDefault();
			var receiptView = new ReceiptView { DataContext = this };
			CopyWindowPosition(oldWindow, receiptView);
			receiptView.Show();
			oldWindow?.Close();
		}
		private void ExecutePrintReceipt() { }
		private void ExecuteReturnToCheckout()
		{
			// Bruges både af "Tilbage" (fra Betaling) og "Afslut uden kvittering" (fra Kvittering)
			var oldWindow = Application.Current.Windows.OfType<Window>()
				.FirstOrDefault(w => w is PaymentMethodView || w is ReceiptView);
			var checkoutView = new CheckoutView { DataContext = this };
			CopyWindowPosition(oldWindow, checkoutView);
			checkoutView.Show();
			oldWindow?.Close();
		}

		// Hjælpemetode: kopierer position/størrelse, så skiftet føles som et sideskift (samme trick som i går)
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
		private void ExecuteScanBarcode()
		{
			if (!int.TryParse(BarCode, out int barcodeNumber))
				return;   // Ugyldigt input, gør ingenting (kunne evt. vise en fejlbesked senere)

			var allItems = _itemsRepository.GetAll();
			var foundItem = allItems.FirstOrDefault(i => i.Barcode == barcodeNumber);

			if (foundItem == null)
				return;   // Varen findes ikke, gør ingenting (kunne evt. vise en fejlbesked senere)

			ScannedCheckoutItems.Add(foundItem);
			AmountToPay = ScannedCheckoutItems.Sum(i => i.ItemPrice);   // Genberegner total
			BarCode = string.Empty;   // Rydder feltet, klar til næste scan
		}

		private void ExecuteSelectPaymentMethod(string method)
		{
			if (method == "Kontant")
				PaymentMethod = PaymentMethod.Kontant;
			else if (method == "MobilePay")
				PaymentMethod = PaymentMethod.MobilePay;

			ExecutePayment();   // Går direkte videre til kvitteringen
								// Vi behøver ikke at vise selve betalingsprocessen. Ovenstående metode går bare videre til kvittering (altså at vi "registrerer betalingen")
		}
	}

	public enum PaymentMethod // Valgmuligheder for betalinger (vi kan evt. tilføje mere, hvis vi mener det er nødvendigt)
	{
		Kontant, 
		MobilePay
	}
}
