using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilterMaster.Models
{
    internal class ActionCommand : ICommand
    {
        public Action<object> Action { get; private set; }

        public ActionCommand(Action<object> action)
        {
            Action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Action != null;
        }

        public void Execute(object parameter)
        {
            Action(parameter);
        }
    }
}
