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
        private DateTime _period;

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

        public DateTime Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }

        // Commands som knapperne i Viewet binder til
        public ICommand HomeCommand { get; }
        public ICommand CloseCommand { get; }

        // Constructor: kobler commands og starter reoloversigten
        public VM_ShelvesOverview()
        {
            HomeCommand = new RelayCommand(ExecuteHome);
            CloseCommand = new RelayCommand(ExecuteClose);

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

        /*
        // Finder lejere, som er tilknyttet en reol
        // Udkommenteret indtil Renter-modellen er færdig
        public List<Renter> FindAssociatedRenters()
        {
            var renters = _rentersRepository.GetAll();

            return renters.FindAll(renter =>
                _shelvesRepository.GetAll()
                    .Exists(shelf => shelf.RenterID == renter.RenterID));
        }
        */

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

        // Lukker reoloversigten
        private void ExecuteClose()
        {
        }
    }
}