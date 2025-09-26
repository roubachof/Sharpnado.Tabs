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

using Sharpnado.TaskLoaderView;

namespace MauiSample.Presentation.ViewModels
{
    /// <summary>
    /// Class SillyDudeVm.
    /// </summary>
    public class SillyDudeViewModel : ANavigableViewModel
    {
        /// <summary>
        /// The front service.
        /// </summary>
        private readonly ISillyDudeService _dudeService;

        private readonly Random _randomizer = new ();

        private int _addedTabCount;

        private int _selectedViewModelIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SillyDudeViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">
        /// The navigation service.
        /// </param>
        /// <param name="sillyDudeService">
        /// The silly front service.
        /// </param>
        public SillyDudeViewModel(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            Console.WriteLine("Building SillyDudeVm...");
            _dudeService = sillyDudeService;

            Notifier = new ();
        }

        /// <summary>
        /// Gets or sets the silly dude task.
        /// </summary>
        /// <value>The silly dude task.</value>
        public TaskLoaderNotifier<ObservableCollection<SillyDude>> Notifier { get; }

        public ObservableCollection<string> TabTitles { get; private set; }

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
            Console.WriteLine("SillyDudeVm|Load()");
            Notifier.Load(_ => LoadSillyDude());
        }

        private async Task<ObservableCollection<SillyDude>> LoadSillyDude()
        {
            var dudeList = await _dudeService.GetSillyPeople();
            
            TabTitles = new ObservableCollection<string>(dudeList.Select(d => d.Name));
            
            RaisePropertyChanged(nameof(TabTitles));

            Console.WriteLine($"SillyDudeVm|LoadSillyDude(): {TabTitles.Count} dudes loaded)");

            // TaskMonitor.Create(TestTabsItemsSourceNotifications);

            return new ObservableCollection<SillyDude>(dudeList);
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