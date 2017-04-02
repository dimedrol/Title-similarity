using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TitleSimilarity
{
    public class Command : ICommand
    {
        public Command( )
        {

        }

        public Command( Action<object> command )
        {
            CommandHandler = command;
        }

        public Command( Action<object> command , Predicate<object> canExecute )
            : this( command )
        {
            CanExecuteHandler = canExecute;
        }

        public Action<object> CommandHandler { get; set; }

        public Predicate<object> CanExecuteHandler { get; set; }

        [DebuggerStepThrough]
        public bool CanExecute( object parameter )
        {
            if( CanExecuteHandler == null )
                return true;

            return CanExecuteHandler( parameter );
        }


        public event EventHandler CanExecuteChanged
        {
            add
            {
                if( CanExecuteHandler != null )
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if( CanExecuteHandler != null )
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public void Execute( object parameter )
        {
            if( CommandHandler != null )
                CommandHandler( parameter );

        }

    }
}
