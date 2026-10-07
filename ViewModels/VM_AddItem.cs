using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;
using Reolmarkedet.Views;
using System.IO;
using System.Windows.Media.Imaging;
using BarcodeStandard;


namespace Reolmarkedet.ViewModels
{
    public class VM_AddItem : ViewModelBase
    {
        // private felter, som værdierne gemmes i

        private string _shelfID;
        private string _renterName;
        private string _renterID;
        private string _itemID;
        private string _itemName;
        private decimal _itemPrice;
        private int _barcode;
        private string _itemDescription;
		private readonly IItemsRepository _itemsRepository = new JsonItemsRepository();

		public string ShelfID
		{
			get => _shelfID;
			set
			{
				SetProperty(ref _shelfID, value);
				ExecuteSearchRenterName();   // Slår automatisk lejeren op, hver gang reolnummeret ændres
			}
		}

		public string RenterName
        {
            get => _renterName;
            set => SetProperty(ref _renterName, value);
        }

        public string RenterID
        {
            get => _renterID;
            set => SetProperty(ref _renterID, value);
        }

        public string ItemID
        {
            get => _itemID;
            set => SetProperty(ref _itemID, value);
        }

        public string ItemName
        {
            get => _itemName;
            set => SetProperty(ref _itemName, value);
        }

        public decimal ItemPrice
        {
            get => _itemPrice;
            set => SetProperty(ref _itemPrice, value);
        }

        public int Barcode
        {
            get => _barcode;
            set => SetProperty(ref _barcode, value);
        }

        public string ItemDescription
        {
            get => _itemDescription;
            set => SetProperty(ref _itemDescription, value);
        }

		// Commands (public, som i DCD'et): dem knapperne i Viewet binder til
		public ICommand NextCommand { get; }
		public ICommand HomeCommand { get; }
		public ICommand PrintLabelCommand { get; }
		public ICommand SearchRenterNameInputCommand { get; }
		public ICommand ItemNameInputCommand { get; }
		public ICommand ItemPriceInputCommand { get; }
		public ICommand ItemDescriptionNoteInputCommand { get; }
		public ICommand ShowRenterNameCommand { get; }

		public VM_AddItem()   // Constructor: kobler hvert command til sin Execute-metode
		{
			NextCommand = new RelayCommand(ExecuteNext);
			HomeCommand = new RelayCommand(ExecuteHome);
			PrintLabelCommand = new RelayCommand(ExecutePrintLabel);
			SearchRenterNameInputCommand = new RelayCommand(ExecuteSearchRenterName);
			ItemNameInputCommand = new RelayCommand(ExecuteInputItemName);
			ItemPriceInputCommand = new RelayCommand(ExecuteItemPriceInput);
			ItemDescriptionNoteInputCommand = new RelayCommand(ExecuteItemDescriptionNote);
			ShowRenterNameCommand = new RelayCommand(_ => ExecuteShowRenterName(ShelfID));
		}

		// Execute-metoder: private, som i DCD'et, undtagen ExecuteShowRenterName som er public
		private void ExecuteNext()
		{
			var allItems = _itemsRepository.GetAll();

			// Fortløbende ID baseret på hvor mange varer der allerede findes
			int nextNumber = allItems.Count + 1;
			string newItemID = "I_" + nextNumber.ToString("D2");   // D2 giver to cifre: 01, 02, 03...

			// Midlertidig simpel stregkode, indtil en rigtig stregkode-løsning er på plads
			int newBarcode = 100000 + nextNumber;

			var newItem = new Items
			{
				ItemID = newItemID,
				ItemName = ItemName,
				ShelfID = ShelfID,
				ItemPrice = ItemPrice,
				ItemDescription = ItemDescription,
				Barcode = newBarcode
			};

			allItems.Add(newItem);
			_itemsRepository.SaveAll(allItems);

			ItemID = newItem.ItemID;
			Barcode = newItem.Barcode;
			GenerateBarcodeImage();


			var oldWindow = Application.Current.Windows.OfType<AddItemView>().FirstOrDefault();
			var itemCreatedView = new ItemCreatedView { DataContext = this };

			if (oldWindow != null)
			{
				itemCreatedView.WindowStartupLocation = WindowStartupLocation.Manual;
				itemCreatedView.Left = oldWindow.Left;
				itemCreatedView.Top = oldWindow.Top;
				itemCreatedView.Width = oldWindow.Width;
				itemCreatedView.Height = oldWindow.Height;
				itemCreatedView.WindowState = oldWindow.WindowState;
			}

			itemCreatedView.Show();
			oldWindow?.Close();
		}
		private void ExecuteHome()
		{
			var oldWindow = Application.Current.Windows.OfType<Window>()
				.FirstOrDefault(w => w is AddItemView || w is ItemCreatedView);

			var menuView = new MenuView { DataContext = new VM_MenuView() };
			menuView.Show();
			oldWindow?.Close();
		}
		private void ExecutePrintLabel() { }

		// Læser gennem det valgte Repo
		private readonly IShelvesRepository _shelvesRepository = new JsonShelvesRepository();
		private readonly IRentersRepository _rentersRepository = new JsonRentersRepository();

		private void ExecuteSearchRenterName()
		{
			var shelf = _shelvesRepository.GetAll().FirstOrDefault(s => s.ShelfID == ShelfID);
			if (shelf == null)
			{
				RenterName = "Reol ikke fundet";
				return;
			}

			var renter = _rentersRepository.GetAll().FirstOrDefault(r => r.RenterID == shelf.RenterID);
			RenterName = renter != null ? renter.RenterName : "Ingen lejer tilknyttet";
		}

		private BitmapImage _barcodeImage;
		public BitmapImage BarcodeImage
		{
			get => _barcodeImage;
			set => SetProperty(ref _barcodeImage, value);
		}

		private void GenerateBarcodeImage() // Kosmetisk Stregkode når vi opretter en vare - Koden er tilhørende BarLib (fandt det på google)
		{
			var barcode = new Barcode();
			var image = barcode.Encode(BarcodeStandard.Type.Code128, Barcode.ToString(), SkiaSharp.SKColors.Black, SkiaSharp.SKColors.White, 300, 100);

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

		private void ExecuteInputItemName() { } // Overflødig
		private void ExecuteItemPriceInput() { } // Overflødig
		private void ExecuteItemDescriptionNote() { } // Overflødig
		public void ExecuteShowRenterName(string shelfID) { }



	}

}