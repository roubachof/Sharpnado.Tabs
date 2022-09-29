using MauiSample.Infrastructure;

namespace MauiSample.Presentation.ViewModels
{
    public class ErrorEmulatorVm
    {
        private readonly ErrorEmulator _errorEmulator;

        private readonly Action _onErrorTypeChanged;

        private int _selectedIndex;

        public ErrorEmulatorVm(ErrorEmulator errorEmulator, Action onErrorTypeChanged)
        {
            _errorEmulator = errorEmulator;
            _onErrorTypeChanged = onErrorTypeChanged;

            ErrorTypes = ErrorEmulator.ErrorLabels;
        }

        public IReadOnlyList<string> ErrorTypes { get; }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _errorEmulator.ErrorType = (ErrorType)_selectedIndex;
                _onErrorTypeChanged();
            }
        }
    }
}
