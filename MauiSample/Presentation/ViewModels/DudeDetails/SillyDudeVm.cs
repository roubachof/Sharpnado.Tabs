// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SillyDudeVm.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   Class SillyDudeVm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using MauiSample.Domain.Silly;
using MauiSample.Presentation.Navigables;

using Sharpnado.Tabs;
using Sharpnado.TaskLoaderView;

namespace MauiSample.Presentation.ViewModels
{
    /// <summary>
    /// Class SillyDudeVm.
    /// </summary>
    public class SillyDudeVm : ANavigableViewModel
    {
        /// <summary>
        /// The front service.
        /// </summary>
        private readonly ISillyDudeService _dudeService;

        private readonly Random _randomizer = new Random();

        private int _addedTabCount = 0;

        private int _selectedViewModelIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SillyDudeVm"/> class.
        /// </summary>
        /// <param name="navigationService">
        /// The navigation service.
        /// </param>
        /// <param name="sillyDudeService">
        /// The silly front service.
        /// </param>
        public SillyDudeVm(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            Console.WriteLine("Building SillyDudeVm...");
            _dudeService = sillyDudeService;

            SillyDudeLoaderNotifier = new TaskLoaderNotifier<SillyDudeVmo>();
        }

        /// <summary>
        /// Gets or sets the silly dude task.
        /// </summary>
        /// <value>The silly dude task.</value>
        public TaskLoaderNotifier<SillyDudeVmo> SillyDudeLoaderNotifier { get; }

        public ObservableCollection<string> TabTitles { get; set;  }

        public QuoteVmo Quote { get; private set; }

        public FilmoVmo Filmo { get; private set; }

        public MemeVmo Meme { get; private set; }

        public int SelectedViewModelIndex
        {
            get => _selectedViewModelIndex;
            set => SetAndRaise(ref _selectedViewModelIndex, value);
        }

        /// <summary>
        /// Loads the specified parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public override void Load(object parameter)
        {
            Console.WriteLine($"SillyDudeVm|Load( id: {parameter} )");
            SillyDudeLoaderNotifier.Load(_ => LoadSillyDude((int)parameter));
        }

        private async Task<SillyDudeVmo> LoadSillyDude(int id)
        {
            var dude = await _dudeService.GetSilly(id);

            Quote = new QuoteVmo(
                dude.SourceUrl,
                dude.Description,
                new TapCommand(url => {}));
            Filmo = new FilmoVmo(dude.FilmoMarkdown);
            Meme = new MemeVmo(dude.MemeUrl);
            RaisePropertyChanged(nameof(Quote));
            RaisePropertyChanged(nameof(Filmo));
            RaisePropertyChanged(nameof(Meme));

            TabTitles = new ObservableCollection<string>
            {
                "Quote",
                "Movies",
                "Fun",
                "Well",
                "Yo!",
            };

            RaisePropertyChanged(nameof(TabTitles));

            Console.WriteLine($"SillyDudeVm|LoadSillyDude(): {dude.FullName} loaded)");

            // TaskMonitor.Create(TestTabsItemsSourceNotifications);

            return new SillyDudeVmo(dude, null);
        }

        private async Task TestTabsItemsSourceNotifications()
        {
            while (true)
            {
                await Task.Delay(5000);
                bool remove = _randomizer.Next(0, 2) == 1;
                int index;

                if (TabTitles.Count == 0)
                {
                    remove = false;
                    index = 0;
                }
                else
                {
                    index = _randomizer.Next(0, TabTitles.Count - 1);
                }

                if (remove)
                {
                    string name = TabTitles[index];
                    System.Diagnostics.Debug.WriteLine($"Removing tab at index {index}: {name}");
                    TabTitles.RemoveAt(index);
                }
                else
                {
                    string name = $"Pipo n°{++_addedTabCount}";
                    System.Diagnostics.Debug.WriteLine($"Adding tab at index {index}: {name}");
                    TabTitles.Insert(index, name);
                }
            }
        }
    }
}