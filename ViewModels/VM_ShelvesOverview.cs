using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;

namespace Reolmarkedet.ViewModels
{
	public class VM_ShelvesOverview : ViewModelBase
	{
		// Private felter, som værdierne gemmes i
		private string _renterID;
		private string _shelfID;
		private string _shelfType;
		private string _shelfStatus;
		private List<Shelves> _shelfList;
		private VM_ShelfDetails _shelfDetails;

		// Repository til at hente reoldata
		private readonly IShelvesRepository _shelvesRepository = new JsonShelvesRepository();

		// Repository til at hente lejerdata
		private readonly IRentersRepository _rentersRepository = new JsonRentersRepository();

		// Properties som Viewet binder til
		public string RenterID
		{
			get => _renterID;
			set => SetProperty(ref _renterID, value);
		}

		public string ShelfID
		{
			get => _shelfID;
			set => SetProperty(ref _shelfID, value);
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

		// Popup'en med reoldetaljer. Er den null, vises ingen popup
		public VM_ShelfDetails ShelfDetails
		{
			get => _shelfDetails;
			set => SetProperty(ref _shelfDetails, value);
		}

		// Commands som knapperne i Viewet binder til
		public ICommand HomeCommand { get; }
		public ICommand SelectShelfCommand { get; }

		// Constructor: kobler commands og starter reoloversigten
		public VM_ShelvesOverview()
		{
			HomeCommand = new RelayCommand(ExecuteHome);
			SelectShelfCommand = new RelayCommand(parameter => ExecuteSelectShelf(parameter));

			ShelfList = _shelvesRepository.GetAll();
		}

		// Finder alle ledige reoler
		public List<Shelves> FindAvailableShelf()
		{
			return _shelvesRepository.GetAll()
				.FindAll(shelf => shelf.ShelfStatus == "Ledig");
		}

		// Finder status på den valgte reol
		public string GetShelfStatus()
		{
			var shelves = _shelvesRepository.GetAll();
			var shelf = shelves.Find(s => s.ShelfID == ShelfID);

			return shelf?.ShelfStatus;
		}

		// Åbner oplysninger om den valgte reol
		private void ExecuteSelectShelf(object parameter)
		{
			var selectedShelf = parameter as Shelves;

			if (selectedShelf == null)
				return;

			ShelfID = selectedShelf.ShelfID;
			RenterID = selectedShelf.RenterID;
			ShelfType = selectedShelf.ShelfType;
			ShelfStatus = selectedShelf.ShelfStatus;

			// Opretter popup'en. Når den lukkes, sættes ShelfDetails til null
			var details = new VM_ShelfDetails(selectedShelf, () => ShelfDetails = null);

			
            // Aktiveres, når Renter-modellen er færdig
            var renter = _rentersRepository.GetAll()
                .Find(r => r.RenterID == selectedShelf.RenterID);

            if (renter != null)
            {
                details.RenterName = renter.RenterName;
                details.RenterPhone = renter.RenterPhone;
                details.RenterEmail = renter.RenterEmail;
            }
            

			ShelfDetails = details;
		}

		
        // Finder lejere, som er tilknyttet en reol
        // Udkommenteret indtil Renter-modellen er færdig
        public List<Renter> FindAssociatedRenters()
        {
            var renters = _rentersRepository.GetAll();

            return renters.FindAll(renter =>
                _shelvesRepository.GetAll()
                    .Exists(shelf => shelf.RenterID == renter.RenterID));
        }
        

		/*
        // Indlæser de lejere, der er tilknyttet reolerne
        // Kommer på plads, når Renter-delen er færdig
        private void ExecuteLoadAssociatedRenter()
        {
        }
        */

		/*
        // Indlæser månedsopgørelsen
        // Kommer på plads, når MonthlyReport-delen er færdig
        private void ExecuteLoadMonthlyReport()
        {
        }
        */

		// Går tilbage til forsiden
		private void ExecuteHome()
		{
		}
	}
}