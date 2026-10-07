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
    /// Interaction logic for CancelRenterView.xaml
    /// </summary>
    public partial class CancelRenterView : Window
    {
        public CancelRenterView()
        {
            InitializeComponent();
        }

		private void RenterItem_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (sender is System.Windows.Controls.ListBoxItem item && item.DataContext is Reolmarkedet.Models.Renter renter)
			{
				var vm = (Reolmarkedet.ViewModels.VM_CancelRenter)DataContext;
				vm.NextCommand.Execute(renter);
			}
		}
	}
}
