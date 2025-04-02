using System.Windows.Input;

namespace SiteHealthMonitor.Commands
{
    public class RelayCommand(Action<object> execute, Func<object, bool> canExecute = null!) : ICommand
    {
        private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public bool CanExecute(object? parameter) => parameter != null && (canExecute?.Invoke(parameter) ?? true);

        public void Execute(object? parameter)
        {
            if (parameter != null) _execute(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public static void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}