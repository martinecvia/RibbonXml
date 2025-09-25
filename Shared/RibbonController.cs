using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.Linq; // Keep for .NET 4.6

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
#if !NET8_0_OR_GREATER
using Autodesk.Internal.Windows;
#endif
using Autodesk.Windows;
#endif
#endregion

namespace RibbonXml
{
    public class RibbonDef
    {
        private readonly Dictionary<string, CommandHandler> _handlers;
        private readonly Type _hWCommandHandler;

        private RibbonControl Ribbon => ComponentManager.Ribbon;

        // Used to avoid multiple instances of event registration 
        private volatile bool _hasTab = false;
        private volatile bool _hasContextual = false;

        private RibbonDef(Dictionary<string, CommandHandler> handlers,
            Type defaultHandler)
        {
            _handlers = handlers;
            _hWCommandHandler = defaultHandler;
        }

        public void SetCommandHandler(string command, CommandHandler handler) =>
            _handlers[command] = handler;
        
        [XmlOut]
        internal System.Reflection.Assembly ExecutingAssembly { get; private set; }
        #region INTERNALS
        [XmlOut]
        internal static System.Windows.Media.ImageSource GetImageSource(string resourceName)
        {
            if (Def == null)
                throw new InvalidOperationException("RibbonDef instance is not built yet.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a handler for the given command string.
        /// Returns a default <see cref="CommandHandler.CommandHandlerDef"/> if the command is not registered.
        /// </summary>
        internal static CommandHandler GetHandler(string command)
        {
                if (string.IsNullOrEmpty(command))
                    return null;
                if (Def == null)
                    throw new InvalidOperationException("RibbonDef instance is not built yet.");
                if (Def._handlers.TryGetValue(command, out CommandHandler handler))
                    return handler;
            try
            {
                if (Def._hWCommandHandler != null)
                    return (CommandHandler)Activator.CreateInstance(Def._hWCommandHandler, command);
            } catch (Exception) { }
            return new CommandHandler.CommandHandlerDef(command); // default fallback
        }
        #endregion 
        #region PRIVATE
        private void ApplyOlderTheme(ContextualRibbonTab tab)
        {
#if !NET8_0_OR_GREATER && !ZWCAD
            T CloneBrush<T>(T brush) where T : class
            => brush == null ? null : (brush as dynamic).Clone();
            // Older AutoCAD versions have bug with template,
            // so it will just pick random template in the current list and apply it to context
            // My fix is that I'll copy Theme from a Hatch tab
            RibbonTab slave = Ribbon?.Tabs?.FirstOrDefault(t => t.IsContextualTab
                && (t.Id == "ACAD.RBN_01738148"
                    || t.Name == "Hatch Editor"          // English
                    || t.Name == "Éditeur de hachures"   // French
                    || t.Name == "Schraffur-Editor"      // German
                    || t.Name == "Editor tratteggio"     // Italian
                    || t.Name == "Edytor kreskowania"    // Polish
                    || t.Name == "Vytváření šraf"        // Czech
                ));
            if (tab.Theme == null
                && slave?.Theme is TabTheme theme)
            {
                tab.Theme = new TabTheme
                {
                    InnerBorder = CloneBrush(theme.InnerBorder),
                    OuterBorder = CloneBrush(theme.OuterBorder),
                    PanelBackground = CloneBrush(theme.PanelBackground),
                    PanelBackgroundVerticalLeft = CloneBrush(theme.PanelBackgroundVerticalLeft),
                    PanelBackgroundVerticalRight = CloneBrush(theme.PanelBackgroundVerticalRight),
                    PanelBorder = CloneBrush(theme.PanelBorder),
                    PanelDialogBoxLauncherBrush = CloneBrush(theme.PanelDialogBoxLauncherBrush),
                    PanelSeparatorBrush = CloneBrush(theme.PanelSeparatorBrush),
                    PanelTitleBackground = CloneBrush(theme.PanelTitleBackground),
                    PanelTitleBorderBrushVertical = CloneBrush(theme.PanelTitleBorderBrushVertical),
                    PanelTitleForeground = CloneBrush(theme.PanelTitleForeground),
                    RolloverTabHeaderForeground = CloneBrush(theme.RolloverTabHeaderForeground),
                    SelectedTabHeaderBackground = CloneBrush(theme.SelectedTabHeaderBackground),
                    SelectedTabHeaderForeground = CloneBrush(theme.SelectedTabHeaderForeground),
                    SlideoutPanelBorder = CloneBrush(theme.SlideoutPanelBorder),
                    TabHeaderBackground = CloneBrush(theme.TabHeaderBackground)
                };
            }
#endif
        }
        #endregion

        [XmlOut]
        public class ContextualRibbonTab : RibbonTab
        { }

        #region BUILDER
        /// <summary>
        /// Provides a registry for managing ribbon command handlers and building
        /// a <see cref="RibbonDef"/> definition that can be used in ribbon UI initialization.
        /// </summary>
        public sealed class RibbonRegistry
        {
            private readonly Dictionary<string, CommandHandler> _handlers
                = new Dictionary<string, CommandHandler>();

            private Type _defaultHandler;

            /// <summary>
            /// Sets the default command handler type.
            /// </summary>
            /// <param name="handler">
            /// The type of the handler to use as default for commands
            /// that do not have an explicit handler assigned.
            /// </param>
            /// <returns>
            /// The current <see cref="RibbonRegistry"/> instance (for fluent API).
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="handler"/> is <c>null</c>.
            /// </exception>
            public RibbonRegistry SetDefaultHandler(Type handler)
            {
                _defaultHandler = handler ?? throw new ArgumentNullException(nameof(handler));
                return this;
            }

            /// <summary>
            /// Registers a command handler for a ribbon command.
            /// </summary>
            /// <param name="handler">The handler delegate associated with the command.</param>
            /// <returns>
            /// The current <see cref="RibbonRegistry"/> instance (for fluent API).
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="handler"/> is <c>null</c>.
            /// </exception>
            public RibbonRegistry AddCommandHandler(CommandHandler handler)
            {
                _handlers[handler.Command] = handler ?? throw new ArgumentNullException(nameof(handler));
                return this;
            }

            /// <summary>
            /// Builds a <see cref="RibbonDef"/> instance with the registered command handlers.
            /// </summary>
            /// <param name="executingAssembly">
            /// The assembly to associate with the <see cref="RibbonDef"/>.
            /// If <c>null</c>, the calling assembly is used instead.
            /// </param>
            /// <returns>
            /// A fully constructed <see cref="RibbonDef"/> object containing the registered
            /// handlers and default handler type.
            /// </returns>
            public RibbonDef Build(System.Reflection.Assembly executingAssembly)
            {
                RibbonDef def = new RibbonDef(new Dictionary<string, CommandHandler>(_handlers), _defaultHandler)
                {
                    ExecutingAssembly = executingAssembly ?? System.Reflection.Assembly.GetCallingAssembly()
                };
                return DefPtr(def);
            }
        }
        #endregion
        private static RibbonDef Def { get; set; }
        private static RibbonDef DefPtr(RibbonDef m_Instance)
        {
            if (m_Instance == null)
                throw new InvalidOperationException("RibbonDef instance is not allowed to be built reflectively.");
            if (Def == null)
                Def = m_Instance;
            return Def;
        }
    }
}