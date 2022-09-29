// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ANavigableViewModel.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The a navigable view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MauiSample.Presentation.Navigables;

namespace MauiSample.Presentation.ViewModels
{
    /// <summary>
    /// The a navigable view model.
    /// </summary>
    public abstract class ANavigableViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ANavigableViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">
        /// The navigation service.
        /// </param>
        protected ANavigableViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The navigation service.
        /// </summary>
        protected INavigationService NavigationService { get; }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public virtual void Load(object parameter)
        {
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("Getting property name form expression is not supported for this type.");
            }

            if (!(expression is LambdaExpression lamda))
            {
                throw new NotSupportedException(
                    "Getting property name form expression is not supported for this type.");
            }

            if (lamda.Body is MemberExpression memberExpression)
            {
                RaisePropertyChanged(memberExpression.Member.Name);
                return;
            }

            var unary = lamda.Body as UnaryExpression;
            if (unary?.Operand is MemberExpression member)
            {
                RaisePropertyChanged(member.Member.Name);
                return;
            }

            throw new NotSupportedException("Getting property name form expression is not supported for this type.");
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The set and raise.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool SetAndRaise<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}