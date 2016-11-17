using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OpenBusDrivingSimulator.GUI
{
    public class ApplicationCommand : ICommand
    {
        private Action action;
        private bool canExecute;

        private event EventHandler canExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add 
            { 
                lock(canExecuteChanged)
                    canExecuteChanged += value; 
            }

            remove 
            { 
                lock (canExecuteChanged)
                    canExecuteChanged -= value; 
            }
        }

        public ApplicationCommand(Action action, bool canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
            this.canExecuteChanged += (s, e) => { };
        }

        public void Execute(object parameter)
        {
            action();
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }
    }
}
