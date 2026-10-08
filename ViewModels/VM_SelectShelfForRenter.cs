using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;
using Reolmarkedet.Views;

namespace Reolmarkedet.ViewModels
{
	public class VM_SelectShelfForRenter : ViewModelBase
	{
		/*private readonly IShelvesRepository _shelvesRepository = new JsonShelvesRepository();*/ // Json 
		private readonly IShelvesRepository _shelvesRepository = new SqlShelvesRepository(); // SQL
		private readonly VM_AddRenter _addRenterViewModel;   // Den ViewModel vi skal sende resultatet tilbage til

		public ObservableCollection<Shelves> AvailableShelves { get; } = new ObservableCollection<Shelves>();
		public ObservableCollection<Shelves> SelectedShelves { get; } = new ObservableCollection<Shelves>();

		public ICommand SelectShelfCommand { get; }
		public ICommand NextCommand { get; }

		public VM_SelectShelfForRenter(VM_AddRenter addRenterViewModel)
		{
			_addRenterViewModel = addRenterViewModel;

			SelectShelfCommand = new RelayCommand(param => ToggleShelfSelection(param as Shelves));
			NextCommand = new RelayCommand(ExecuteNext);

			LoadAvailableShelves();
		}

		private void LoadAvailableShelves()
		{
			AvailableShelves.Clear();
			var ledige = _shelvesRepository.GetAll().Where(s => s.ShelfStatus == "Ledig");
			foreach (var shelf in ledige)
				AvailableShelves.Add(shelf);
		}

		// Tilføjer eller fjerner en reol fra valget, ved klik
		private void ToggleShelfSelection(Shelves shelf)
		{
			if (shelf == null) return;

			if (SelectedShelves.Contains(shelf))
				SelectedShelves.Remove(shelf);
			else
				SelectedShelves.Add(shelf);
		}

		private void ExecuteNext()
		{
			if (SelectedShelves.Count == 0) return;

			// Sender de valgte reolnumre tilbage til AddRenter-flowet
			_addRenterViewModel.ShelfList = SelectedShelves.ToList();
			_addRenterViewModel.GoToPaymentScreen();
		}
	}
}