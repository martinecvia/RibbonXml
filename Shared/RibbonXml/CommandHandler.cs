using System; // Keep for .NET 4.6

namespace Shared.RibbonXml
{
    /// <summary>
    /// A simple implementation of the <see cref="System.Windows.Input.ICommand"/> interface
    /// that executes a given CAD command string when invoked.
    /// </summary>
    public class CommandHandler : System.Windows.Input.ICommand
    {
        private readonly string _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class with the specified command string.
        /// </summary>
        /// <param name="command">The CAD command string to be executed.</param>
        public CommandHandler(string command) => _command = command; // Keep for .NET 4.6

        public string Command => _command;
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// Always returns <c>true</c> in this implementation.
        /// </summary>
        /// <param name="parameter">Unused parameter.</param>
        /// <returns><c>true</c> to indicate the command can always execute.</returns>
        public bool CanExecute(object _) => true;

        /// <summary>
        /// Executes the stored CAD command by sending it to the active document.
        /// </summary>
        /// <param name="_">Unused parameter.</param>
        public void Execute(object _)
        { }
    }
}