using System;
using System.ComponentModel;              // Giver adgang til INotifyPropertyChanged og PropertyChangedEventArgs
using System.Runtime.CompilerServices;     // Giver adgang til [CallerMemberName]-attributten

namespace Reolmarkedet.Commands
{
	public abstract class ViewModelBase : INotifyPropertyChanged // "abstract" betyder klassen ikke kan oprettes direkte - kun arves fra
	{
		public event PropertyChangedEventHandler PropertyChanged; // Selve eventet, som UI'et lytter til for at vide hvornår det skal opdatere sig

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => // Kaldes når en property ændrer sig
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));      // Affyrer eventet, hvis nogen lytter (UI'et)
																							// [CallerMemberName] gør at propertyName automatisk udfyldes med navnet på den property, der kaldte metoden

		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null) // Hjælpemetode: sætter en værdi og giver besked om ændringen
		{
			if (Equals(field, value)) return false;   // Hvis værdien er uændret, gør ingenting og returner false (sparer unødvendige opdateringer)
			field = value;                             // Opdater selve feltet med den nye værdi
			OnPropertyChanged(propertyName);           // Giv UI'et besked om at property'en har ændret sig
			return true;                               // Returner true, fordi værdien faktisk blev ændret
		}
	}
}
