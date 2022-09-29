using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiSample.Domain.Silly;
using MauiSample.Infrastructure;
using MauiSample.Presentation.Navigables;

using Sharpnado.TaskLoaderView;

namespace MauiSample.Presentation.ViewModels
{
    public class GridPageViewModel : ANavigableViewModel
    {
        private const int PageSize = 20;
        private readonly ISillyDudeService _sillyDudeService;

        private ObservableCollection<SillyDudeVmo> _sillyPeople;

        private int _currentIndex = -1;

        private int? _selectedDudeId;

        public GridPageViewModel(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            _sillyDudeService = sillyDudeService;

            InitCommands();

            SillyPeople = new ObservableCollection<SillyDudeVmo>();
            SillyPeopleLoaderNotifier = new TaskLoaderNotifier<List<SillyDude>>();
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetAndRaise(ref _currentIndex, value);
        }

        public TaskLoaderCommand<SillyDudeVmo> GoToSillyDudeCommand { get; protected set; }

        public ICommand OnScrollBeginCommand { get; private set; }

        public ICommand OnScrollEndCommand { get; private set; }

        public TaskLoaderNotifier<List<SillyDude>> SillyPeopleLoaderNotifier { get; }

        public ObservableCollection<SillyDudeVmo> SillyPeople
        {
            get => _sillyPeople;
            set => SetAndRaise(ref _sillyPeople, value);
        }

        public int? SelectedDudeId
        {
            get => _selectedDudeId;
            set => SetAndRaise(ref _selectedDudeId, value);
        }

        public override void Load(object parameter)
        {
            SillyPeople = new ObservableCollection<SillyDudeVmo>();

            SillyPeopleLoaderNotifier.Load((isRefreshing) => LoadSillyPeoplePageAsync(1, 20, isRefreshing));
        }

        private void InitCommands()
        {
            GoToSillyDudeCommand = new TaskLoaderCommand<SillyDudeVmo>(
                parameter =>
                {
                    SelectedDudeId = parameter.Id;
                    if (PlatformService.IsFoldingScreen)
                    {
                        return Task.CompletedTask;
                    }

                    return NavigationService.NavigateToAsync<SillyDudeVm>(((SillyDudeVmo)parameter).Id);
                });

            OnScrollBeginCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollBeginCommand"));
            OnScrollEndCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollEndCommand"));
        }

        private async Task<List<SillyDude>> LoadSillyPeoplePageAsync(int pageNumber, int pageSize, bool isRefresh)
        {
            List<SillyDude> resultPage = await _sillyDudeService.GetSillyPeoplePage(pageNumber, pageSize);
            var viewModels = resultPage.Select(dude => new SillyDudeVmo(dude, GoToSillyDudeCommand)).ToList();

            SillyPeople = new ObservableCollection<SillyDudeVmo>(viewModels);

            // Uncomment to test CurrentIndex property
            // TaskMonitor.Create(
            //    async () =>
            //    {
            //        await Task.Delay(2000);
            //        CurrentIndex = 15;
            //    });

            // Uncomment to test Reset action
            // TaskMonitor.Create(
            //   async () =>
            //   {
            //       await Task.Delay(6000);
            //       SillyPeople.Clear();
            //       await Task.Delay(3000);
            //       SillyPeople = new ObservableRangeCollection<SillyDudeVmo>(viewModels);
            //   });

            return resultPage;
        }
    }
}