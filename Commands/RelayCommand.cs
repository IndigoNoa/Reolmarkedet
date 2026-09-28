using System;                          // Giver adgang til Action, Func og ArgumentNullException
using System.Windows.Input;            // Giver adgang til ICommand og CommandManager (WPF)

namespace Reolmarkedet.Commands
{
	public class RelayCommand : ICommand   // RelayCommand implementerer ICommand-interfacet, så den kan bindes til knapper i XAML
	{
		private readonly Action<object> _execute;        // Den metode der skal køres, når commandet udføres (tager en parameter)
		private readonly Func<object, bool> _canExecute;  // Den metode der afgør om commandet må udføres lige nu (returnerer true/false)

		public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null) // Constructor: tager execute-logik og valgfri canExecute-logik
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute)); // Gemmer execute-metoden, kaster fejl hvis den er null
			_canExecute = canExecute;                                              // Gemmer canExecute-metoden (kan være null, betyder "altid tilladt")
		}

		// Bekvem overload til commands uden parameter
		public RelayCommand(Action execute, Func<bool> canExecute = null) // Alternativ constructor: bruges når commandet ikke skal have en parameter
			: this(_ => execute(), canExecute == null ? (Func<object, bool>)null : _ => canExecute()) // Omdanner de parameterløse metoder til de parameter-metoder ovenfor
		{
		}

		public event EventHandler CanExecuteChanged   // Event der fortæller UI'et, at CanExecute skal tjekkes igen (fx knappen skal gråtones)
		{
			add { CommandManager.RequerySuggested += value; }    // Hægter sig på WPF's indbyggede mekanisme til automatisk gentjek
			remove { CommandManager.RequerySuggested -= value; } // Fjerner sig fra samme mekanisme igen
		}

		public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true; // Kalder canExecute hvis den findes, ellers antages "true" (altid tilladt)

		public void Execute(object parameter) => _execute(parameter); // Kører selve handlingen (execute-metoden) med den givne parameter

		// Kald denne manuelt, hvis I ikke bruger CommandManager (fx i .NET MAUI)
		public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested(); // Tvinger UI'et til at tjekke CanExecute igen med det samme
	}
}
