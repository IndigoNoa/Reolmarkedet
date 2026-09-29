using System;
using System.Collections.Generic;
using System.Text;
using Reolmarkedet.Commands;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Reolmarkedet.Views;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;


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
            get => _shelfID;                                            // Returnerer den gemte værdi
            set => SetProperty(ref _shelfID, value);                    // Gemmer værdien, og giver UI'et besked om ændringen
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
		private void ExecuteHome() { }
		private void ExecutePrintLabel() { }
		private void ExecuteSearchRenterName() { }
		private void ExecuteInputItemName() { }
		private void ExecuteItemPriceInput() { }
		private void ExecuteItemDescriptionNote() { }
		public void ExecuteShowRenterName(string shelfID) { }



	}

}