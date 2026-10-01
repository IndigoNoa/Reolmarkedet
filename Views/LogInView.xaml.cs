using Reolmarkedet.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Reolmarkedet.Views
{
    /// <summary>
    /// Interaction logic for LogInView.xaml
    /// </summary>
    public partial class LogInView : Window
    {

        private VM_LogIn _viewModel; //Reference til LoginViewModel'en som View'et arbejder sammen med

        public LogInView()
        {
            InitializeComponent();

            _viewModel = new VM_LogIn(); //Opretter LoginViewModel'en som håndterer loginlogikken

            DataContext = _viewModel; //Fortæller View'et, at det skal bruge LoginViewModel'en til sine "Bindings" 

            _viewModel.LoginSuccessful += OnLoginSuccessful; //Forbinder View'et med eventet, så det reagerer, når login lykkedes
        }

        private void OnLoginSuccessful(object sender, EventArgs e) //Håndterer hvad der sker når login lykkedes
        {
            MenuView menuView = new MenuView(); //Initialiserer MenuView, som vises efter et vellykket login
            menuView.Show(); //Viser MenuView efter et vellykket login
            Close(); //Lukker LoginView efter et vellykket login
        }

        private void LogindButton_Click(object sender, RoutedEventArgs e)
        {
            // Sender adgangskoden fra PasswordBox til ViewModel'en og starter login-kontrollen
            _viewModel.EmployeePassword = PasswordInput.Password;
            _viewModel.LoginCommand.Execute(null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        { 
        Application.Current.Shutdown(); //Lukker hele programmet
        }
    }
}
