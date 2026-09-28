using Reolmarkedet.Commands;
using Reolmarkedet.Models;
using Reolmarkedet.Repositories;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows;

namespace Reolmarkedet.ViewModels
{
    public class VM_LogIn : ViewModelBase //Gør det muligt for ViewModel'en at bruge funktionaliteten fra ViewModelBase
    {
        private string _employeeID; //Felt til ID'et medarbejderen indtaster ved login
        private string _employeePassword; //Felt til adgangskoden medarbejderen indtaster ved login
        private IEmployeesRepository _employeesRepository; //Reference til repository'et, der bruges til at hente medarbejderoplysninger

        public string EmployeeID //Giver adgang til medarbejderens indtastede ID
        {
            get => _employeeID; //Henter det indtastede ID
            set => SetProperty(ref _employeeID, value); //Opdaterer ID'et og giver ViewModelBase besked om ændringen
        }

        public string EmployeePassword //Giver adgang til medarbejderens indtastede adgangskode
        { 
            get => _employeePassword; //Henter det indtastede adgangskode
            set => SetProperty(ref _employeePassword, value);  //Opdaterer adgangskoden og giver ViewModelBase besked om ændringen
        }

        public ICommand LoginCommand { get; } //Kommandoen bruges når medarbejder trykker på "Log in" 

        public event EventHandler LoginSuccessful; //Event der aktiveres, når loginoplysningerne er korrekte 

        public VM_LogIn() //Opretter LoginCommand, når LoginViewModel'en oprettes 
        {
            _employeesRepository = new JsonEmployeesRepository(); //Opretter repository'et, der henter medarbejderoplysninger fra JSON-filen
            
            LoginCommand = new RelayCommand(ExecuteLogin); //Forbinder LoginCommand med metoden, der håndterer loginforsøget
        }

        private void ExecuteLogin() //Kontrollerer medarbejderens loginoplysninger
        { 
            List<Employees> allEmployees = _employeesRepository.GetAll(); //Henter alle eksiterende medarbejdere og deres oplysninger fra repository'et 

            foreach (Employees employee in allEmployees) //Gennemgår alle medarbejdere for at finde det indtastede ID
            {
                if (employee.EmployeeID == _employeeID) //Kontrollere om medarbejderens ID matcher med det indtastede ID
                {

                    if (employee.EmployeePassword == _employeePassword)//Kontrollere om adgangskoden matcher med den indtastede adganskode
                    {
                        LoginSuccessful?.Invoke(this, new EventArgs()); //Aktiverer eventet, når loginoplysnignerne er korrekte
                    }
                }
            }
        }
    }
}
