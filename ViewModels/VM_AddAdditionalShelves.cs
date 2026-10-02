using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;

namespace Reolmarkedet.ViewModels
{
    public class VM_AdditionalShelves : ViewModelBase
    {
        // Lejepriser pr. reol pr. måned (fra casen)
        private const decimal PriceOneShelf = 850m;          // 1 reol
        private const decimal PriceTwoToThreeShelves = 825m; // 2-3 reoler
        private const decimal PriceFourOrMoreShelves = 800m; // 4+ reoler

        // Private felter, som værdierne gemmes i
        private int _currentStep = 1;
        private string _searchText;
        private List<RenterListItem> _renters = new List<RenterListItem>();
        private RenterListItem _selectedRenter;
        private List<Shelves> _shelfList = new List<Shelves>();
        private Shelves _selectedShelf;
        private decimal _pricePerShelf;
        private decimal _multipleShelvesDiscount;

        // Alle lejere, før der søges. Renters er den filtrerede liste, Viewet viser
        private List<RenterListItem> _allRenters = new List<RenterListItem>();

        // Repositories til at hente og gemme data
        private readonly IShelvesRepository _shelvesRepository = new JsonShelvesRepository();
        // Nedenstående aktiveres ved merge
       // private readonly IRentersRepository _rentersRepository = new JsonRentersRepository();

        // Navigation: sættes udefra, så ViewModel'en ikke kender vinduerne
        private readonly Action _onHome;
        private readonly Action _onConfirmed;

        // Properties som Viewet binder til

        // 1 = vælg lejer, 2 = vælg reol, 3 = bekræft
        public int CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public List<RenterListItem> Renters
        {
            get => _renters;
            set => SetProperty(ref _renters, value);
        }

        public RenterListItem SelectedRenter
        {
            get => _selectedRenter;
            set => SetProperty(ref _selectedRenter, value);
        }

        public List<Shelves> ShelfList
        {
            get => _shelfList;
            set => SetProperty(ref _shelfList, value);
        }

        public Shelves SelectedShelf
        {
            get => _selectedShelf;
            set => SetProperty(ref _selectedShelf, value);
        }

        public decimal PricePerShelf
        {
            get => _pricePerShelf;
            set => SetProperty(ref _pricePerShelf, value);
        }

        public decimal MultipleShelvesDiscount
        {
            get => _multipleShelvesDiscount;
            set => SetProperty(ref _multipleShelvesDiscount, value);
        }

        // Commands som knapperne i Viewet binder til
        public ICommand HomeCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SearchRenterNameInputCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand ConfirmAndAddCommand { get; }

        public VM_AdditionalShelves(Action onHome = null, Action onConfirmed = null)
        {
            _onHome = onHome;
            _onConfirmed = onConfirmed;

            HomeCommand = new RelayCommand(ExecuteHome);
            BackCommand = new RelayCommand(ExecuteBack);
            SearchRenterNameInputCommand = new RelayCommand(ExecuteRenterNameSearch);
            NextCommand = new RelayCommand(ExecuteNext, CanExecuteNext);
            ConfirmAndAddCommand = new RelayCommand(ExecuteConfirmAndAdd);

            ExecuteLoadRenter();
        }

        // TRIN 1: Vælg lejer

        // Indlæser lejere og de reoler, de har i dag
        // MIDLERTIDIG: laver lejerlisten ud fra reolerne indtil merge
        // Navnet er foreløbig "Lejer <RenterID>", og lejere uden reoler vises ikke
        private void ExecuteLoadRenter()
        {
            _allRenters = _shelvesRepository.GetAll()
                .Where(s => !string.IsNullOrEmpty(s.RenterID))
                .GroupBy(s => s.RenterID)
                .Select(g => new RenterListItem
                {
                    RenterID = g.Key,
                    RenterName = "Lejer " + g.Key,
                    ShelfNumbers = string.Join(", ", g.Select(s => s.ShelfID))
                })
                .ToList();

            Renters = _allRenters;
        }

        // Filtrerer lejerlisten på navn
        private void ExecuteRenterNameSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Renters = _allRenters;
                return;
            }

            Renters = _allRenters
                .Where(r => r.RenterName.IndexOf(SearchText.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        // TRIN 2: Vælg ledig reol

        // Indlæser alle reoler. Kun "Ledig" kan vælges i Viewet
        private void ExecuteLoadShelves()
        {
            ShelfList = _shelvesRepository.GetAll();
            SelectedShelf = null;
        }

        // TRIN 3: Bekræft

        // Pris pr. reol inkl. mængderabat (vises på bekræftelsen).
        // Antallet tæller lejerens nuværende reoler + den nye
        public void CalculatePrice()
        {
            int currentCount = string.IsNullOrEmpty(SelectedRenter?.ShelfNumbers)
                ? 0
                : SelectedRenter.ShelfNumbers.Split(',').Length;

            int totalCount = currentCount + 1;

            if (totalCount >= 4)
                PricePerShelf = PriceFourOrMoreShelves;
            else if (totalCount >= 2)
                PricePerShelf = PriceTwoToThreeShelves;
            else
                PricePerShelf = PriceOneShelf;

            MultipleShelvesDiscount = PriceOneShelf - PricePerShelf;
        }

        // Gemmer den nye reol på lejeren og går videre til betaling
        private void ExecuteConfirmAndAdd()
        {
            if (SelectedRenter == null || SelectedShelf == null)
                return;

            var all = _shelvesRepository.GetAll();
            var shelf = all.Find(s => s.ShelfID == SelectedShelf.ShelfID);

            // Reolen kan være blevet udlejet imellem
            if (shelf == null || shelf.ShelfStatus != "Ledig")
                return;

            shelf.RenterID = SelectedRenter.RenterID;
            shelf.ShelfStatus = "Udlejet";
            shelf.RentalStartDate = DateTime.Today;
            shelf.CancellationDate = null;

            _shelvesRepository.SaveAll(all);

            // TODO: åbn betalingsvisningen her
            _onConfirmed?.Invoke();
        }

        // NAVIGATION

        // Næste er kun aktiv, når der er valgt noget i det aktuelle trin
        private bool CanExecuteNext()
        {
            return (CurrentStep == 1 && SelectedRenter != null)
                || (CurrentStep == 2 && SelectedShelf != null);
        }

        // Går et trin frem
        private void ExecuteNext()
        {
            if (CurrentStep == 1 && SelectedRenter != null)
            {
                ExecuteLoadShelves();
                CurrentStep = 2;
            }
            else if (CurrentStep == 2 && SelectedShelf != null)
            {
                CalculatePrice();
                CurrentStep = 3;
            }
        }

        // Et trin tilbage, eller tilbage til menuen fra trin 1
        private void ExecuteBack()
        {
            if (CurrentStep > 1)
                CurrentStep--;
            else
                ExecuteHome();
        }

        // Går tilbage til menuen
        private void ExecuteHome()
        {
            _onHome?.Invoke();
        }
    }
}