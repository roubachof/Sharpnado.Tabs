// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INavigationService.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MauiSample.Presentation.ViewModels;

namespace MauiSample.Presentation.Navigables
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the bindable page matching the given navigable view model type.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <param name="parameter">
        /// The parameter passed to the view model Load method.
        /// </param>
        /// <param name="modalNavigation">
        /// True if we want a modal navigation.
        /// </param>
        /// <param name="clearStack">
        /// Navigate and clears the stack history (the new view become the new navigation root)
        /// </param>
        /// <param name="animated">
        /// If true animate the navigation.
        /// </param>
        /// <returns>
        /// </returns>
        Task NavigateToAsync<TViewModel>(object parameter = null, bool modalNavigation = false, bool clearStack = false, bool animated = true)
            where TViewModel : ANavigableViewModel;

        /// <summary>
        /// Navigates to the bindable page matching the given navigable view model type + the given transition.
        /// </summary>
        /// <example>
        /// FullAutoPatientAssayScreenVm + NavigationTransition.ToResult =&gt; navigates to a PatientAssayResultView
        /// </example>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <param name="viewModel">
        /// The view model instance.
        /// </param>
        /// <param name="transition">
        /// The transition to be made.
        /// </param>
        /// <param name="rootChild">
        /// Navigates and makes the new view a child from the root page.
        /// </param>
        /// <returns>
        /// </returns>
        Task NavigateToAsync<TViewModel>(TViewModel viewModel, NavigationTransition transition, bool rootChild = false)
            where TViewModel : ANavigableViewModel;

        /// <summary>
        /// Navigation from menu means: reset the stack, and then add the new page.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model to navigate to.
        /// </typeparam>
        /// <returns>
        /// </returns>
        Task NavigateFromMenuToAsync<TViewModel>()
            where TViewModel : ANavigableViewModel;

        /// <summary>
        /// Closes the current bindable page.
        /// </summary>
        /// <returns>
        /// </returns>
        Task<IBindablePage> NavigateBackAsync(object parameter = null);
    }

    /// <summary>
    /// Bindable page.
    /// </summary>
    public interface IBindablePage
    {
        /// <summary>
        /// Gets or sets the binding context.
        /// </summary>
        object BindingContext { get; set; }
    }
}