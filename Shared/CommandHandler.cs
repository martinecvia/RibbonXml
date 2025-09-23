using System; // Keep for .NET 4.6
using System.Diagnostics;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using AcApp = ZwSoft.ZwCAD.ApplicationServices;
#else
using AcApp = Autodesk.AutoCAD.ApplicationServices;
#endif
#endregion

namespace RibbonXml
{
    /// <summary>
    /// A simple implementation of the <see cref="System.Windows.Input.ICommand"/> interface
    /// that executes a given CAD command string when invoked.
    /// </summary>
    public abstract class CommandHandler : System.Windows.Input.ICommand
    {
        private readonly string _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class with the specified command string.
        /// </summary>
        /// <param name="command">The CAD command string to be executed.</param>
        public CommandHandler(string command)
        {
            _command = command;
        }

        public string Command => _command;
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// Always returns <c>true</c> in this implementation.
        /// </summary>
        /// <param name="parameter">Unused parameter.</param>
        /// <returns><c>true</c> to indicate the command can always execute.</returns>
        public virtual bool CanExecute(object _) => true;

        /// <summary>
        /// Executes the stored CAD command by sending it to the active document.
        /// </summary>
        /// <param name="_">Unused parameter.</param>
        public abstract void Execute(object _);

        internal class CommandHandlerDef : CommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandHandlerDef"/> class
            /// with the specified command string.
            /// </summary>
            /// <param name="command">The CAD command string associated with this handler.</param>
            public CommandHandlerDef(string command) : base(command)
            { }

            public override void Execute(object _)
            {
                var _document = AcApp.Application.DocumentManager.MdiActiveDocument;
                _document?.SendStringToExecute($"{_command} ", true, false, true);
                Debug.WriteLine($"[&] Attempted {_command}");
            }
        }
    }
}