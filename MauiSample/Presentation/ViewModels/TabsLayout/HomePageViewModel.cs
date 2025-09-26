
using MauiSample.Domain.Silly;
using MauiSample.Infrastructure;
using MauiSample.Presentation.Navigables;

using Sharpnado.TaskLoaderView;

namespace MauiSample.Presentation.ViewModels
{
    public class HomePageViewModel : ANavigableViewModel
    {
        private readonly ISillyDudeService _sillyDudeService;

        public HomePageViewModel(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            _sillyDudeService = sillyDudeService;
            InitCommands();

            SillyDudeLoaderNotifier = new TaskLoaderNotifier<SillyDudeVmo>();
        }

        public TaskLoaderNotifier<SillyDudeVmo> SillyDudeLoaderNotifier { get; }

        /// <summary>
        /// Commands accessible directly on screen are declared in the ScreenVm.
        /// Here, it is a command to navigate to the second screen.
        /// </summary>
        public TaskLoaderCommand<SillyDudeVmo> GoToSillyDudeCommand { get; protected set; }

        public override void Load(object parameter)
        {
            SillyDudeLoaderNotifier.Load(_ => InitializationTask());
        }

        /// <summary>
        /// We usually init all commands in a "InitCommands" method which is called in constructor.
        /// </summary>
        private void InitCommands()
        {
            GoToSillyDudeCommand = new TaskLoaderCommand<SillyDudeVmo>(parameter => GoToSillyDudeAsync((SillyDudeVmo)parameter));
        }

        private async Task<SillyDudeVmo> InitializationTask()
        {
            var dude = await _sillyDudeService.GetRandomSilly(1);
            return new SillyDudeVmo(dude, GoToSillyDudeCommand);
        }

        /// <param name="sillyDude">The silly dude.</param>
        /// <returns>The <see cref="Task" />.</returns>
        /// <exception cref="System.InvalidOperationException">The knights demand...... A SACRIFICE!</exception>
        private async Task GoToSillyDudeAsync(SillyDudeVmo sillyDude)
        {
            if (sillyDude.Role == "Knights")
            {
                throw new InvalidOperationException("The knights demand...... A SACRIFICE!");
            }

            if (PlatformService.IsFoldingScreen)
            {
                return;
            }

            await NavigationService.NavigateToAsync<SillyDudeViewModel>(sillyDude.Id);
        }
    }
}