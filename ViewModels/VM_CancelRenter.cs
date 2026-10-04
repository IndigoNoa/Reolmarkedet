using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Reolmarkedet.ViewModels
{
    public class CancelRenterViewModel : BaseViewModel
    {
        public ObservableCollection<CancelRenter> RenterList { get; set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand CancelSelectedRenterCommand { get; private set; }
        public ICommand CancelSelectedReolCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        private CancelRenter _selectedRenter;
        public CancelRenter SelectedRenter
        {
            get => _selectedRenter;
            set
            {
                _selectedRenter = value;
                OnPropertyChanged(nameof(SelectedRenter));
            }
        }

        public CancelRenterViewModel()
        {
            RenterList = new ObservableCollection<CancelRenter>();
            LoadRenters();
            SearchCommand = new RelayCommand(SearchRenters);
            CancelSelectedRenterCommand = new RelayCommand(CancelSelectedRenter, CanCancelSelectedRenter);
            CancelSelectedReolCommand = new RelayCommand(CancelSelectedReol);
            CloseCommand = new RelayCommand(Close);
        }

        private void LoadRenters()
        {
            // Her skal du hente lejer fra databasen eller en liste
            RenterList.Add(new CancelRenter { Id = 1, Name = "Ola Nordmann", Address = "Adresse 1", Phone = "12345678" });
            RenterList.Add(new CancelRenter { Id = 2, Name = "Kari Nordmann", Address = "Adresse 2", Phone = "87654321" });
            // Tilføj flere lejere som nødvendigt
        }

        private void SearchRenters()
        {
            // Implementer søgelogik
        }

        private void CancelSelectedRenter()
        {
            if (SelectedRenter != null)
            {
                // Logik til at opsige lejeren
                SelectedRenter.IsCancelled = true;
                RenterList.Remove(SelectedRenter);
            }
        }

        private bool CanCancelSelectedRenter()
        {
            return SelectedRenter != null;
        }

        private void CancelSelectedReol()
        {
            // Logik til at opsige reolen
        }

        private void Close()
        {
            // Luk vinduet
        }
    }
}
