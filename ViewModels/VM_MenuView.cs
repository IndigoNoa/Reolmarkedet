using Reolmarkedet.Commands;
using Reolmarkedet.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Reolmarkedet.ViewModels
{
    class VM_MenuView : ViewModelBase
    {
        // Commands til de forskellige funktioner, medarbejderen kan vælge fra menuen
        public ICommand AddRenterCommand { get; }
        public ICommand ShelvesOverviewCommand { get; }
        public ICommand CancelRenterCommand {  get; }
        public ICommand AddItemCommand { get; }
        public ICommand MonthlyReportCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand AddAdditionalShelvesCommand { get; }
        public ICommand LogOutCommand { get; }

        public event EventHandler LogOutSuccessful;

        public VM_MenuView() 
        {
            // Forbinder hver command med den metode, der udfører den valgte funktion
            AddRenterCommand = new RelayCommand(ExecuteAddRenter);
            ShelvesOverviewCommand = new RelayCommand(ExecuteShelvesOverview);
            CancelRenterCommand = new RelayCommand(ExecuteCancelRenter);
            AddItemCommand = new RelayCommand(ExecuteAddItem);
            MonthlyReportCommand = new RelayCommand(ExecuteMonthlyReport);
            CheckoutCommand = new RelayCommand(ExecuteCheckout);
            AddAdditionalShelvesCommand = new RelayCommand(ExecuteAddAdditionalShelves);
            LogOutCommand = new RelayCommand(ExecuteLogOut);
        }

        private void ExecuteAddRenter()
        {
            //Åbner vinduet til oprettelse af ny lejer
            AddRenterView addRenterView = new AddRenterView();
            addRenterView.Show();
        }

        private void ExecuteShelvesOverview()
        { 
            //Åbner vinduet til reoloversigt
            ShelvesOverviewView shelvesOverviewView = new ShelvesOverviewView();
            shelvesOverviewView.Show();
        }

        private void ExecuteCancelRenter()
        {
            //Åbner vinduet til opsig lejer/reol
            CancelRenterView cancelRenterView = new CancelRenterView();
            cancelRenterView.Show();
        }

        private void ExecuteAddItem()
        {
            //Åbner vinduet til opret varer
            AddItemView addItemView = new AddItemView();
            addItemView.Show();
        }

        private void ExecuteMonthlyReport()
        {
            //Åbner vinduet til månedsopgørelse
            MonthlyReportView monthlyReportView = new MonthlyReportView();
            monthlyReportView.Show();
        }

        private void ExecuteCheckout()
        {
            //Åbner vinduet til kasse
            CheckoutView checkoutView = new CheckoutView();
            checkoutView.Show();
        }

        private void ExecuteAddAdditionalShelves()
        {
            //Åbner vinduet til opret yderligere reol
            AddAdditonalShelvesView addAdditionalShelvesView = new AddAdditonalShelvesView();
            addAdditionalShelvesView.Show();
        }

        private void ExecuteLogOut()
        {
            // Fortæller MenuView, at medarbejderen har valgt at logge ud
            LogOutSuccessful?.Invoke(this, EventArgs.Empty);
        }
    }
}
