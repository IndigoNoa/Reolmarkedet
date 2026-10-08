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
using System.IO;
using System.Windows.Media.Imaging;
using BarcodeStandard; 


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
		private readonly ISalesRepository _salesRepository = new JsonSalesRepository();
		private readonly IPaymentsRepository _paymentsRepository = new JsonPaymentsRepository();

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
			// Opretter betalingen
			var allPayments = _paymentsRepository.GetAll();
			string newPaymentID = "P_" + (allPayments.Count + 1).ToString("D2");

			var payment = new Payment
			{
				PaymentID = newPaymentID,
				PaymentMethod = PaymentMethod,
				AmountPaid = AmountToPay,
				PaymentDate = DateTime.Now,
			};

			allPayments.Add(payment);
			_paymentsRepository.SaveAll(allPayments);

			// Markerer de solgte varer som IsSold i items.json
			var allItems = _itemsRepository.GetAll();
			foreach (var soldItem in ScannedCheckoutItems)
			{
				var matchingItem = allItems.FirstOrDefault(i => i.ItemID == soldItem.ItemID);
				if (matchingItem != null)
					matchingItem.IsSold = true;
			}
			_itemsRepository.SaveAll(allItems);

			// Opretter ét salg per vare i kurven, hver med sit eget SaleID
			var allSales = _salesRepository.GetAll();

			foreach (var item in ScannedCheckoutItems)
			{
				string newSaleID = "S_" + (allSales.Count + 1).ToString("D2");

				allSales.Add(new Sales
				{
					SaleID = newSaleID,
					ItemID = item.ItemID,
					ItemName = item.ItemName,
					ShelfID = item.ShelfID,
					ItemPrice = item.ItemPrice,
					PaymentID = newPaymentID,
					SaleDate = DateTime.Now,
					EmployeeID = Reolmarkedet.Models.CurrentSession.EmployeeID
				});
			}

			_salesRepository.SaveAll(allSales);

			_salesRepository.SaveAll(allSales);

			// Gemmer felter til kvitteringen
			PaymentID = newPaymentID;
			SaleDate = DateTime.Now;
			ServedBy = Reolmarkedet.Models.CurrentSession.EmployeeName;

			// Navigerer videre til kvitteringen (uændret fra før)
			var oldWindow = Application.Current.Windows.OfType<PaymentMethodView>().FirstOrDefault();
			var receiptView = new ReceiptView { DataContext = this };
			CopyWindowPosition(oldWindow, receiptView);
			receiptView.Show();
			oldWindow?.Close();
		}
		private void ExecutePrintReceipt() { } // Måske død kode
		private void ExecuteReturnToCheckout()
		{
			var oldWindow = Application.Current.Windows.OfType<Window>()
				.FirstOrDefault(w => w is PaymentMethodView || w is ReceiptView);

			// Rydder kurven og nulstiller felterne, klar til et nyt køb efter den visuelle kvittering bliver vist
			ScannedCheckoutItems.Clear();
			AmountToPay = 0;
			BarCode = string.Empty;
			BarcodeImage = null;

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
			GenerateBarcodeImage(barcodeNumber);   // Viser stregkoden for den senest scannede vare
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

		private BitmapImage _barcodeImage;
		public BitmapImage BarcodeImage
		{
			get => _barcodeImage;
			set => SetProperty(ref _barcodeImage, value);
		}

		private void GenerateBarcodeImage(int barcodeNumber)
		{
			var barcode = new Barcode();
			var image = barcode.Encode(BarcodeStandard.Type.Code128, barcodeNumber.ToString(), SkiaSharp.SKColors.Black, SkiaSharp.SKColors.White, 200, 60);

			using var memoryStream = new MemoryStream();
			using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
			data.SaveTo(memoryStream);
			memoryStream.Position = 0;

			var bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = memoryStream;
			bitmapImage.EndInit();
			bitmapImage.Freeze();

			BarcodeImage = bitmapImage;
		}

	}

}
