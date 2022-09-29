using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiSample.Domain.Silly;
using MauiSample.Presentation.Navigables;

using Sharpnado.Tasks;

namespace MauiSample.Presentation.ViewModels
{
    public class SortSillyPeopleVm : ANavigableViewModel
    {
        private readonly ISillyDudeService _sillyDudeService;

        public SortSillyPeopleVm(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            _sillyDudeService = sillyDudeService;
            InitCommands();
        }

        public ICommand OnDragAndDropEndCommand { get; private set; }

        public ICommand OnSillyDudeAddedCommand { get; private set; }

        public ObservableCollection<SillyDudeVmo> SillyPeople { get; set; }

        public override void Load(object parameter)
        {
            if (parameter is ObservableCollection<SillyDudeVmo> observableDudes)
            {
                SillyPeople =
                    new ObservableCollection<SillyDudeVmo>(observableDudes)
                    {
                        new AddSillyDudeVmo(OnSillyDudeAddedCommand),
                    };

                RaisePropertyChanged(nameof(SillyPeople));
                return;
            }

            TaskMonitor.Create(NavigationService.NavigateBackAsync(typeof(SortSillyPeopleVm)));
        }

        private void InitCommands()
        {
            OnDragAndDropEndCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SortSillyPeopleVm: OnDragAndDropEndCommand"));
            OnSillyDudeAddedCommand = new Command(() => TaskMonitor.Create(AddSillyDudeAsync));
        }

        private async Task AddSillyDudeAsync()
        {
            var newDude = await _sillyDudeService.GetRandomSilly(0);
            SillyPeople.Insert(SillyPeople.Count - 2, new SillyDudeVmo(newDude, null));
        }
    }
}
