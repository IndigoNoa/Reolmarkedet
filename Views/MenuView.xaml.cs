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
using Reolmarkedet.ViewModels;

namespace Reolmarkedet.Views
{
	/// <summary>
	/// Interaction logic for MenuView.xaml
	/// </summary>
	public partial class MenuView : Window
	{
		private VM_MenuView _viewModel; // Reference til MenuViewModel'en
		public MenuView()
		{
			InitializeComponent();

			_viewModel = new VM_MenuView(); // Opretter MenuViewModel'en
			DataContext = _viewModel; // Forbinder MenuView med dens ViewModel

			_viewModel.LogOutSuccessful += OnLogOutSuccessful; // Forbinder logout-eventet
		}

		private void OnLogOutSuccessful(object sender, EventArgs e)
		{
			// Åbner LoginView og lukker MenuView efter logout
			LogInView logInView = new LogInView();
			logInView.Show();
			Close();
		}
	}
}