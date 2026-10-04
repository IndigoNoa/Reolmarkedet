using System.Windows;
using Reolmarkedet.ViewModels;

namespace Reolmarkedet.Views
{
	public partial class AdditionalShelvesView : Window
	{
		public AdditionalShelvesView()
		{
			InitializeComponent();

			// Lukker vinduet, når man går tilbage til menuen eller bekræfter
			DataContext = new VM_AdditionalShelves(
				onHome: Close,
				onConfirmed: Close);   // TODO: erstat med åbning af betalingsvisningen
		}
	}
}