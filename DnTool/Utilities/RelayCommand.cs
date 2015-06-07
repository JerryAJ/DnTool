﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DnTool.Utilities
{
    public class RelayCommand:ICommand
    {
        //public RelayCommand(Action<object> executeMethod)
        //    :this(executeMethod,null,false)
        //{
        //}
         public RelayCommand(Action executeMethod) 
             : this(executeMethod, null, false)
        {
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)     
        {
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        public bool CanExecute()
        {
            if (_canExecuteMethod != null)
            {
                return _canExecuteMethod();
            }
            return true;
        }

        public void Execute()
        {
            if (_executeMethod != null)
            {
                _executeMethod();
            }
        }

        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return _isAutomaticRequeryDisabled;
            }
            set
            {
                if (_isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
                    }
                    _isAutomaticRequeryDisabled = value;
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }
                CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }
                CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        private readonly Action _executeMethod = null;
        private readonly Func<bool> _canExecuteMethod = null;
        private bool _isAutomaticRequeryDisabled = false;
        private List<WeakReference> _canExecuteChangedHandlers;

    }

    public class RelayCommand<T> : ICommand
    {
       public RelayCommand(Action<T> execute)
              : this(execute, null)
          {
          }
  
          public RelayCommand(Action<T> execute, Predicate<T> canExecute)
          {
             if (execute == null)
                 throw new ArgumentNullException("execute");
 
             _execute = execute;
             _canExecute = canExecute;
         }
 
         [DebuggerStepThrough]
         public bool CanExecute(object parameter)
         {
             return _canExecute == null ? true : _canExecute((T)parameter);
         }
         public event EventHandler CanExecuteChanged
         {
             add{}
             remove{} 
             //add
             //{
             //    if (_canExecute != null)
             //        CommandManager.RequerySuggested += value;
             //}
             //remove
             //{
             //    if (_canExecute != null)
             //        CommandManager.RequerySuggested -= value;
             //}
         }
 
         public void Execute(object parameter)
         {
             _execute((T)parameter);
         }
 
         readonly Action<T> _execute = null;
         readonly Predicate<T> _canExecute = null;
 
         bool ICommand.CanExecute(object parameter)
         {
             throw new NotImplementedException();
         }
 
         event EventHandler ICommand.CanExecuteChanged
         {
             add { throw new NotImplementedException(); }
             remove { throw new NotImplementedException(); }
         }
 
         void ICommand.Execute(object parameter)
         {
             throw new NotImplementedException();
         }
     
    }
}
