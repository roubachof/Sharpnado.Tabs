// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyContainer.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The dependency container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SimpleInjector;

namespace MauiSample
{
    /// <summary>
    /// The dependency container.
    /// </summary>
    public static class DependencyContainer
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static Container Instance { get; } = new Container();
    }
}