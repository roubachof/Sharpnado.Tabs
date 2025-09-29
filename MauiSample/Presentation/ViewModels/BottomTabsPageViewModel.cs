using MauiSample.Domain.Silly;
using MauiSample.Infrastructure;
using MauiSample.Presentation.Navigables;

namespace MauiSample.Presentation.ViewModels
{
    public class BottomTabsPageViewModel : ANavigableViewModel
    {
        private enum Tab
        {
            SillyDude = 3
        }
        
        private int _selectedViewModelIndex;

        public BottomTabsPageViewModel(
            INavigationService navigationService,
            ISillyDudeService sillyDudeService,
            ErrorEmulator errorEmulator)
            : base(navigationService)
        {
            SillyDudeViewModel = new SillyDudeViewModel(navigationService, sillyDudeService);

            // If you want to start at the 3rd page
            // SelectedViewModelIndex = 2;
        }

        public SillyDudeViewModel SillyDudeViewModel { get; }
        
        public int SelectedViewModelIndex
        {
            get => _selectedViewModelIndex;
            set
            {
                if (SetAndRaise(ref _selectedViewModelIndex, value))
                {
                    OnViewModelSelected((Tab)value);
                }
            }
        }

        private void OnViewModelSelected(Tab value)
        {
            if (value == Tab.SillyDude && SillyDudeViewModel.Notifier.IsNotStarted)
            {
                SillyDudeViewModel.Load(null);
            }
        }

        public override void Load(object parameter)
        {
            // Uncomment to test tab visibility
            // TaskMonitor.Create(
            //    async () =>
            //    {
            //        await Task.Delay(10000);
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            IsTabVisible = false;
            //            RaisePropertyChanged(nameof(IsTabVisible));
            //        });
            //    });
        }
    }
}
