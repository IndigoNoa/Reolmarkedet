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
    public partial class AddRenterView : Window
    {
        public AddRenterView()
        {
            InitializeComponent();
            DataContext = new AddRenterViewModel(); // Sætter DataContext til ViewModel'en
        }

        private void AddRenterButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AddRenterViewModel;
            if (viewModel != null)
            {
                // Kald funktionen til at tilføje en lejer i ViewModel
                viewModel.AddRenter();
                // Efter at have tilføjet, kan du klare inputfelterne, hvis nødvendigt
                NameTextBox.Clear();
                AddressTextBox.Clear();
                PhoneTextBox.Clear();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Lukker vinduet
        }
    }
}
