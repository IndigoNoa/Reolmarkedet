using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Reolmarkedet.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;
using System.Windows;
using Reolmarkedet.Views;

namespace Reolmarkedet.ViewModels
{
    public class VM_CancelRenter : ViewModelBase
    {
		// Backing fields
		private string _renterID;
		private string _renterName;
		private string _informationNote;

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

		public string InformationNote
		{
			get => _informationNote;
			set => SetProperty(ref _informationNote, value);
		}

		public string ShelfIDsText => string.Join(", ", RenterShelves.Select(s => s.ShelfID));

		// Gør at XAML søgeboks har noget at binde sig til (bare en mere fancy måde end at benytte SearchRenterNameInputCommand)
		private string _searchQuery;
		public string SearchQuery
		{
			get => _searchQuery;
			set => SetProperty(ref _searchQuery, value);
		}

		// Lejerens reoler, og den/de reol(er) der rent faktisk opsiges
		public List<Shelves> RenterShelves { get; } = new List<Shelves>();

		// Lejerliste til søgeresultatet (Hi-Fi: liste over lejere man kan søge/vælge imellem)
		public ObservableCollection<Renter> SearchResults { get; } = new ObservableCollection<Renter>();

		// Repositories
		private readonly IRentersRepository _rentersRepository = new JsonRentersRepository();
		private readonly IShelvesRepository _shelvesRepository = new JsonShelvesRepository();

		// Commands
		public ICommand SearchRenterNameInputCommand { get; }
		public ICommand NextCommand { get; }
		public ICommand ConfirmCancellationCommand { get; }
		public ICommand HomeCommand { get; }

		public VM_CancelRenter()
		{
			SearchRenterNameInputCommand = new RelayCommand(ExecuteRenterNameSearch);
			NextCommand = new RelayCommand(param => ExecuteNext(param as Renter));
			ConfirmCancellationCommand = new RelayCommand(ExecuteConfirmCancellation);
			HomeCommand = new RelayCommand(ExecuteHome);
		}

		// Execute-metoder (tomme skeletter for nu)

		private void ExecuteRenterNameSearch()
		{
			SearchResults.Clear();

			if (string.IsNullOrWhiteSpace(SearchQuery)) return;

			var allRenters = _rentersRepository.GetAll();
			var allShelves = _shelvesRepository.GetAll();

			foreach (var renter in allRenters)
			{
				bool nameMatches = renter.RenterName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
				bool shelfMatches = allShelves.Any(s => s.RenterID == renter.RenterID && s.ShelfID == SearchQuery);

				if (nameMatches || shelfMatches)
					SearchResults.Add(renter);
			}
		}
		private void ExecuteNext(Renter selectedRenter)
		{
			if (selectedRenter == null) return;

			RenterID = selectedRenter.RenterID;
			RenterName = selectedRenter.RenterName;

			RenterShelves.Clear();
			var allShelves = _shelvesRepository.GetAll();
			foreach (var shelf in allShelves.Where(s => s.RenterID == RenterID))
				RenterShelves.Add(shelf);

			var oldWindow = Application.Current.Windows.OfType<CancelRenterView>().FirstOrDefault();
			var confirmView = new CancelRenterConfirmView { DataContext = this };
			CopyWindowPosition(oldWindow, confirmView);
			confirmView.Show();
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
		private void ExecuteConfirmCancellation()
		{
			var allShelves = _shelvesRepository.GetAll();

			foreach (var renterShelf in RenterShelves)
			{
				var matchingShelf = allShelves.FirstOrDefault(s => s.ShelfID == renterShelf.ShelfID);
				if (matchingShelf != null)
					matchingShelf.CancellationDate = DateTime.Now;
				// Status forbliver "Udlejet" indtil perioden faktisk udløber, jf. UC2
			}

			_shelvesRepository.SaveAll(allShelves);

			var oldWindow = Application.Current.Windows.OfType<CancelRenterConfirmView>().FirstOrDefault();
			var doneView = new RenterCancelledView { DataContext = this };
			CopyWindowPosition(oldWindow, doneView);
			doneView.Show();
			oldWindow?.Close();
		}
		private void ExecuteHome()
		{
			var oldWindow = Application.Current.Windows.OfType<RenterCancelledView>().FirstOrDefault();

			var menuView = new MenuView { DataContext = new VM_MenuView() };
			menuView.Show();
			oldWindow?.Close();
		}
	}
}
