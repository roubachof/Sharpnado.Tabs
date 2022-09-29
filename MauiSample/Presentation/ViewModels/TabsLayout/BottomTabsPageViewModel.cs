using MauiSample.Domain.Silly;
using MauiSample.Infrastructure;
using MauiSample.Presentation.Navigables;

namespace MauiSample.Presentation.ViewModels
{
    public class BottomTabsPageViewModel : ANavigableViewModel
    {
        private int _selectedViewModelIndex = 0;

        public BottomTabsPageViewModel(
            INavigationService navigationService,
            ISillyDudeService sillyDudeService,
            ErrorEmulator errorEmulator)
            : base(navigationService)
        {
            HomePageViewModel = new HomePageViewModel(navigationService, sillyDudeService);
            GridPageViewModel = new GridPageViewModel(navigationService, sillyDudeService);
        }

        public int SelectedViewModelIndex
        {
            get => _selectedViewModelIndex;
            set => SetAndRaise(ref _selectedViewModelIndex, value);
        }

        public HomePageViewModel HomePageViewModel { get; }

        public GridPageViewModel GridPageViewModel { get; }

        public bool IsTabVisible { get; set; } = true;

        public override void Load(object parameter)
        {
            HomePageViewModel.Load(parameter);
            GridPageViewModel.Load(parameter);

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
