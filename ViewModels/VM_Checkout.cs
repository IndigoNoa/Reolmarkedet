using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using System.Collections.ObjectModel;
using Reolmarkedet.Models;

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
		private void ExecuteGoToPaymentMethod() { }
		private void ExecutePayment() { }
		private void ExecutePrintReceipt() { }
		private void ExecuteReturnToCheckout() { }
		private void ExecuteScanBarcode() { }

		private void ExecuteSelectPaymentMethod(string method) // Til valg af betalingsmetode i PaymentMethodView.xaml
		{
			if (method == "Kontant")
				PaymentMethod = PaymentMethod.Kontant;
			else if (method == "MobilePay")
				PaymentMethod = PaymentMethod.MobilePay;
		}
	}

	public enum PaymentMethod // Valgmuligheder for betalinger (vi kan evt. tilføje mere, hvis vi mener det er nødvendigt)
	{
		Kontant, 
		MobilePay
	}
}
