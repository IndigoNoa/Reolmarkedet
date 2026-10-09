using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;

namespace Reolmarkedet.ViewModels
{
	// En reol i gitteret, med markering og om den kan vælges
	public class ShelfChoice : ViewModelBase
	{
		public Shelves Shelf { get; }
		public bool IsAvailable => Shelf.ShelfStatus == "Ledig";

		private bool _isSelected;
		public bool IsSelected
		{
			get => _isSelected;
			set => SetProperty(ref _isSelected, value);
		}

		public ShelfChoice(Shelves shelf)
		{
			Shelf = shelf;
		}
	}

	public class VM_SelectShelfForRenter : ViewModelBase
	{
		private readonly IShelvesRepository _shelvesRepository = new SqlShelvesRepository();
		private readonly VM_AddRenter _addRenterViewModel;
		private readonly List<Shelves> _selectedShelves = new List<Shelves>();

		public ObservableCollection<ShelfChoice> AllShelves { get; } = new ObservableCollection<ShelfChoice>();

		public ICommand SelectShelfCommand { get; }
		public ICommand NextCommand { get; }

		public VM_SelectShelfForRenter(VM_AddRenter addRenterViewModel)
		{
			_addRenterViewModel = addRenterViewModel;

			SelectShelfCommand = new RelayCommand(param => ToggleShelfSelection(param as ShelfChoice));
			NextCommand = new RelayCommand(ExecuteNext);

			LoadShelves();
		}

		// Henter alle reoler, sorteret efter reolnummer
		private void LoadShelves()
		{
			AllShelves.Clear();
			var sorted = _shelvesRepository.GetAll()
				.OrderBy(s => int.TryParse(s.ShelfID, out int n) ? n : int.MaxValue);

			foreach (var shelf in sorted)
				AllShelves.Add(new ShelfChoice(shelf));
		}

		// Vælger eller fravælger en ledig reol
		private void ToggleShelfSelection(ShelfChoice choice)
		{
			if (choice == null || !choice.IsAvailable) return;

			if (choice.IsSelected)
			{
				choice.IsSelected = false;
				_selectedShelves.Remove(choice.Shelf);
			}
			else
			{
				choice.IsSelected = true;
				_selectedShelves.Add(choice.Shelf);
			}
		}

		private void ExecuteNext()
		{
			if (_selectedShelves.Count == 0) return;

			_addRenterViewModel.ShelfList = _selectedShelves.ToList();
			_addRenterViewModel.GoToPaymentScreen();
		}
	}
}