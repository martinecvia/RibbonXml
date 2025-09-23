using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6

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
        private RibbonDef(Dictionary<string, CommandHandler> handlers,
            CommandHandler defaultHandler)
        {
            _handlers = handlers;
        }

        private RibbonControl Ribbon => ComponentManager.Ribbon;

        // Used to avoid multiple instances of event registration 
        private volatile bool _hasTab = false;
        private volatile bool _hasContextual = false;

        [XmlOut]
        internal static System.Windows.Media.ImageSource GetImageSource(string resourceName)
        {
            if (def == null)
                throw new InvalidOperationException("RibbonDef instance is not built yet.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a handler for the given command string.
        /// Returns a default CommandHandlerDef if the command is not registered.
        /// </summary>
        public static CommandHandler GetHandler(string command)
        {
            if (string.IsNullOrEmpty(command))
                return null;
            if (def == null)
                throw new InvalidOperationException("RibbonDef instance is not built yet.");
            if (def._handlers.TryGetValue(command, out CommandHandler handler))
                return handler;
            return new CommandHandler.CommandHandlerDef(command); // default fallback
        }

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
        {

        }

        #region BUILDER
        public class RibbonDefBuilder
        {
            private readonly Dictionary<string, CommandHandler> _handlers 
                = new Dictionary<string, CommandHandler>();
            private CommandHandler _defaultHandler;

            public RibbonDefBuilder SetDefaultHandler(CommandHandler handler)
            {
                if(handler == null)
                    throw new ArgumentNullException(nameof(handler));
                _defaultHandler = handler;
                return this;
            }

            public RibbonDefBuilder AddCommandHandler(string command, CommandHandler handler)
            {
                if (string.IsNullOrEmpty(command) || handler == null)
                    throw new ArgumentNullException(string.IsNullOrEmpty(command) ? nameof(command) : nameof(handler));
                _handlers[command] = handler;
                return this;
            }

            public RibbonDef Build()
            {
                RibbonDef def = new RibbonDef(
                    new Dictionary<string, CommandHandler>(_handlers), 
                    _defaultHandler);
                return DefPtr(def);
            }
        }
        #endregion
        private static RibbonDef def { get; set; }
        private static RibbonDef DefPtr(RibbonDef instance)
        {
            if (instance == null)
                throw new InvalidOperationException("RibbonDef instance is not allowed to be built reflectively.");
            if (def == null)
                def = instance;
            return def;
        }
    }
}