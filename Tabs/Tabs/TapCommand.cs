using System;

using Xamarin.Forms;

namespace Sharpnado.Tabs
{
    public class TapCommand : Command
    {
        private DateTime _lastExecution = DateTime.MinValue;

        public TapCommand(Action<object> executeMethod)
            : base(executeMethod)
        {
        }

        public TapCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
        }

        protected new void Execute(object parameter)
        {
            // Prevent multiple touch
            if (DateTime.Now - _lastExecution < TimeSpan.FromMilliseconds(1000))
            {
                return;
            }

            _lastExecution = DateTime.Now;
            base.Execute(parameter);
        }
    }
}