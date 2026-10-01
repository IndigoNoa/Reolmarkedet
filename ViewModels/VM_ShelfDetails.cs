using System;
using System.Windows.Input;
using Reolmarkedet.Commands;
using Reolmarkedet.Models;

namespace Reolmarkedet.ViewModels
{
    public class VM_ShelfDetails : ViewModelBase
    {
        // Fra reolen
        public string ShelfTitle { get; set; }
        public string ShelfStatus { get; set; }
        public string RentalStartDate { get; set; }
        public string CancellationDate { get; set; }

        // Lejerfelter, fyldes ud når Renter-modellen er færdig
        public string RenterName { get; set; }
        public string RenterPhone { get; set; }
        public string RenterEmail { get; set; }

        public ICommand CloseCommand { get; }

        public VM_ShelfDetails(Shelves shelf, Action onClose)
        {
            ShelfTitle = $"Reol {shelf.ShelfID}";
            ShelfStatus = shelf.ShelfStatus;

            // Datoerne vises kun, hvis reolen er lejet ud
            if (shelf.RentalStartDate != null)
            {
                RentalStartDate = shelf.RentalStartDate.Value.ToString("dd-MM-yy");
                CancellationDate = shelf.CancellationDate?.ToString("dd-MM-yy") ?? "Ikke opsagt";
            }

            CloseCommand = new RelayCommand(() => onClose());
        }
    }
}