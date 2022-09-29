// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewLocator.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MauiSample.Presentation.ViewModels;

namespace MauiSample.Presentation.Navigables
{
    /// <summary>
    /// Service responsible for locating the correct view from the ViewModel infos.
    /// The service is currently also responsible for the creation of the view and the view model if needed.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Builds the view matching the given view model type.
        /// Builds the view model and bind it to the created view.
        /// Loads the view model.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <returns>
        /// </returns>
        IBindablePage GetViewFor<TViewModel>()
            where TViewModel : ANavigableViewModel;

        /// <summary>
        /// Builds the view matching the given [view model type + transition].
        /// Binds the view model instance to the created view.
        /// </summary>
        /// <example>
        /// FullAutoPatientAssayScreenVm + NavigationTransition.ToResult =&gt; creates a PatientAssayResultView
        /// </example>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <param name="viewModel">
        /// The view model to be bound to the created view.
        /// </param>
        /// <param name="transition">
        /// The transition leading to the view.
        /// </param>
        /// <remarks>
        /// The service regards the view model as already loaded.
        /// </remarks>
        /// <returns>
        /// </returns>
        IBindablePage GetViewFor<TViewModel>(TViewModel viewModel, NavigationTransition transition)
            where TViewModel : ANavigableViewModel;

        /// <summary>
        /// Gets the view type matching the given view model.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <returns>
        /// </returns>
        Type GetViewTypeFor<TViewModel>()
            where TViewModel : ANavigableViewModel;

        /// <summary>
        /// Gets the view type matching the given view model and transition.
        /// </summary>
        /// <param name="viewModel">
        /// </param>
        /// <param name="transition">
        /// </param>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <returns>
        /// </returns>
        Type GetViewTypeFor<TViewModel>(TViewModel viewModel, NavigationTransition transition)
            where TViewModel : ANavigableViewModel;
    }
}