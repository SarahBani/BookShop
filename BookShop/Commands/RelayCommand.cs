using System;
using System.Windows.Input;

namespace BookShop.Commands
{
    public sealed class RelayCommand : ICommand
    {

        #region Properties & Fields

        private readonly Action<object> _executeAction;

        #endregion /Properties & Fields

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion /Events

        #region Constructors

        public RelayCommand(Action<object> executeAction)
        {
            this._executeAction = executeAction;
        }

        #endregion /Constructors

        #region Methods

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => this._executeAction(parameter);

        #endregion /Methods

    }
}