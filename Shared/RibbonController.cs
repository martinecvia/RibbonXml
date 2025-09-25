#define NON_VOLATILE_MEMORY

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.Linq; // Keep for .NET 4.6
using System.IO;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Reflection;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using AcApp = ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.Windows;
#else
using AcApp = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
#if !NET8_0_OR_GREATER
using Autodesk.Internal.Windows;
#endif
using Autodesk.Windows;
#endif
#endregion

using RibbonXml.Items;
using RibbonXml.Items.CommandItems;
using System.Diagnostics;

namespace RibbonXml
{
    public class RibbonDef
    {
        public const string RibbonTab__Prefix = "RP_TAB_";  // Our prefix for tabs. so we can distinguish
                                                            // other tab's from AutoCAD.
                                                            // This also prevents using the same name from different applications.
        private RibbonControl Ribbon => ComponentManager.Ribbon;

        private readonly Dictionary<string, CommandHandler> _handlers;
        private readonly Dictionary<string, object> _registeredControls = new Dictionary<string, object>();
        private readonly Dictionary<string, ContextualRibbonTab> _activeContextualTabs = new Dictionary<string, ContextualRibbonTab>();
        private readonly Dictionary<string, Func<SelectionSet, bool>> _contextualTabConditions
            = new Dictionary<string, Func<SelectionSet, bool>>();

        private List<RibbonTab> _Tabs = new List<RibbonTab>();
        private readonly Type _hWCommandHandler;

        // Used to avoid multiple instances of event registration 
        private volatile bool _hasTab = false;
        private volatile bool _hasContextual = false;

        private RibbonDef(Assembly lsAssembly, string hwNamespace, 
            Dictionary<string, CommandHandler> hwHandlers, Type hWCommandHandler)
        {
            ExecutingAssembly = lsAssembly;
            ControlsNamespace = hwNamespace;
            _handlers = hwHandlers;
            _hWCommandHandler = hWCommandHandler;
            System.Diagnostics.Debug.WriteLine($"[&] Registered RibbonController\n" +
                $"  ExecutingAssembly: {ExecutingAssembly?.GetName()}\n" +
                $"  ControlsNamespace: {ControlsNamespace ?? string.Empty}\n" +
                $"  HasDefault: {hWCommandHandler != null}\n" +
                $"  CustomHandlers: {string.Join(",", hwHandlers.Keys)}\n" +
                $"--- END OF STACK");
        }

        public RibbonTab CreateTab(string tabId, string tabName = null, string tabDescription = null)
            => CreateTab<RibbonTab>(tabId, tabName, tabDescription);

        public ContextualRibbonTab CreateContextualTab(string tabId)
        {
            string Id = tabId ?? throw new ArgumentNullException(nameof(tabId));
            ContextualRibbonTab tab = (ContextualRibbonTab)Ribbon?.Tabs?
                .FirstOrDefault(t => t is ContextualRibbonTab _contextualTab && t.Id == RibbonTab__Prefix + Id);
            if (tab != null)
                return tab;
            tab = CreateTab<ContextualRibbonTab>(tabId);
            tab.IsVisible = false;
            tab.IsContextualTab = true;
            ApplyOlderTheme(tab);
            // This protects stack agains multiple instances of Idle event registration
            if (!_hasContextual)
            {
                AcApp.Core.Application.Idle += OnApplicationIdle;
                _hasContextual = true;
            }
            return tab;
        }

        public void SetCommandHandler(string command, CommandHandler handler) =>
            _handlers[command] = handler;
        
        #region INTERNALS
        internal Assembly ExecutingAssembly { get; private set; }
        internal string ControlsNamespace { get; private set; }
        internal static System.Windows.Media.ImageSource GetImageSource(string resourceName)
        {
            if (Def == null)
                throw new InvalidOperationException("RibbonDef instance is not built yet.");
            Debug.WriteLine(resourceName);
            return null;
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
            } catch (System.Exception) { }
            return new CommandHandler.CommandHandlerDef(command); // default fallback
        }

        internal void HideContextualTab(ContextualRibbonTab _contextualTab)
            => HideContextualTab(_contextualTab.Id);
        internal void ShowContextualTab(ContextualRibbonTab _contextualTab)
            => ShowContextualTab(_contextualTab.Id);

        #endregion
        #region PRIVATE
        /// <summary>
        /// Ensures that the ribbon system has been properly initialized before use.
        /// This method is intended to prevent loading via reflection or in unsupported contexts.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the ribbon has not been initialized, which may indicate
        /// improper access such as reflection-based loading or an invalid initialization sequence.
        /// </exception>
        private void AssertInitialized()
        {
            // Prevent usage in cases where the ribbon is not properly initialized,
            // such as when loaded via reflection or outside the intended application lifecycle.
            if (Ribbon == null)
                throw new InvalidOperationException("Ribbon can't be loaded using reflection or before application initializes properly.");
        }

        private T CreateTab<T>(string tabId,
                               string tabName = null,
                               string tabDescription = null) where T : RibbonTab, new()
        {
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            if (Ribbon == null)
                return new T();
            string Id = tabId ?? throw new ArgumentNullException(nameof(tabId));
            T tab = (T)Ribbon.Tabs.FirstOrDefault(t => t.Id == RibbonTab__Prefix + Id);
            if (tab != null)
                return tab; // We really don't want to process same tab multiple times,
                            // there is no point in that
            RibbonTabDef tabDef = LoadResourceRibbon(Id);
            tab = tabDef?.Transform(new T()) ?? new T();
            Ribbon?.Tabs.Add(tab);
            tab.Id = RibbonTab__Prefix + Id;  // We want to mark these tabs as ours
                                              // For further compatibility and to prevent being overriden.
            if (tabDef != null)
            {
                foreach (RibbonPanelDef panelDef in tabDef.m_Panels)
                {
                    // Setting up cookie must happend before transforming to reference item
                    string cookie = tab.Id;
                    panelDef.Cookie = panelDef.Cookie.Replace("%Parent", cookie);
                    cookie += $";{panelDef.Id}";
                    RibbonPanel panelRef = panelDef.Transform(new RibbonPanel());
                    panelRef.UID = panelDef.Id; // For some reason panel can't have Id
                    RegisterControl(panelRef, panelDef);
                    if (panelDef.m_Source == null)
                        continue;
                    tab.Panels.Add(panelRef);
                    panelDef.m_Source.Cookie = panelDef.m_Source.Cookie.Replace("%Parent", cookie);
                    cookie += $";{panelDef.m_Source.Id}";
                    panelRef.Source = panelDef.m_Source.Transform(RibbonPanelSourceDef.SourceFactory[panelDef.m_Source.GetType()]());
                    RegisterControl(panelRef.Source, panelDef.m_Source);
                    foreach (RibbonItemDef itemDef in panelDef.m_Source.m_Items)
                    {
                        itemDef.Cookie = itemDef.Cookie.Replace("%Parent", cookie);
                        cookie += $";{itemDef.Id}";
                        RibbonItem itemRef = ProcessRibbonItem(itemDef, panelDef, cookie, currentDepth: 0); // Directly setting currentDepth to zero here,
                                                                                                            // because sometimes C# keeps refference to previous currentDepth, which is odd 
                                                                                                            // even tho function has default value defined,
                                                                                                            // so some might think it will take the default value.
                                                                                                            // This is a compiler issue with .NET 4.6 NDP46-KB3045557-x86-x64
                        if (itemRef != null) // null RibbonItem definitions will break cad instance
                            panelRef.Source.Items.Add(itemRef);
                    }
                }
            }
            tab.IsEnabled = true;
            if (!string.IsNullOrEmpty(tabDescription))
                tab.Description = tabDescription;
            tab.UID = tab.Id;
            tab.IsContextualTab = false;
            if (tabDef != null)
                RegisterControl(tab, tabDef);
            if (!_hasTab)
            {
                if (Ribbon != null)
                {
                    // We want to keep our tabs displayed at all costs
                    Ribbon.Tabs.CollectionChanged += (s, e) =>
                    {
                        // The contents of the collection changed dramatically
                        if (e.Action != NotifyCollectionChangedAction.Reset)
                            return;
                        if (_Tabs.Count == 0 || Ribbon == null)
                            return;
                        // Defer to avoid reentrancy
                        Ribbon?.Dispatcher?.BeginInvoke(new Action(() =>
                        {
                            if (Ribbon == null) return;
                            foreach (RibbonTab reAdd in _Tabs)
                            {
                                // If for some reason tab still exists, ignore it
                                if (Ribbon.Tabs.Contains(reAdd))
                                    continue;
                                bool wasActive = reAdd.IsActive;
                                Ribbon.Tabs.Add(reAdd);
                                // Adding ribbon to tab deactivates its IsActive state to default,
                                // so we check if it was active before,
                                // and make it active again
                                reAdd.IsActive = wasActive;
                                System.Diagnostics.Debug.WriteLine($"[&] Re-added: {reAdd.Id}");
                            }
                        }));
                    };
                    _hasTab = true;
                }
            }
            _Tabs.Add(tab);
            return tab;
        }

        public RibbonTabDef LoadResourceRibbon(string resourceName)
        {
            try
            {
                string manifestResource = ExecutingAssembly.GetManifestResourceNames()
                .FirstOrDefault(resource => resource.EndsWith($"{resourceName}.xml", StringComparison.OrdinalIgnoreCase));
                using (Stream stream = ExecutingAssembly.GetManifestResourceStream(manifestResource))
                {
                    if (stream == null) return default;
                    try
                    {
                        Type type = typeof(RibbonTabDef);
                        XmlSerializer serializer = new XmlSerializer(type, new XmlRootAttribute("RibbonTab"));
                        return (RibbonTabDef)serializer.Deserialize(stream);
                    }
                    catch (InvalidOperationException exception)
                    {
                        LogException(exception);
                        return default;
                    }
                    catch (Exception exception)
                    {
                        LogException(exception);
                        return default;
                    }
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                return default;
            }
        }

        private void HideContextualTab(string _contextualId)
        {
            if (_activeContextualTabs.ContainsKey(_contextualId)
                && _activeContextualTabs[_contextualId] is ContextualRibbonTab _contextualTab)
            {
                Ribbon?.HideContextualTab(_contextualTab);
                _contextualTab.IsVisible = false;
                _activeContextualTabs.Remove(_contextualId);
            }
        }

        private void ShowContextualTab(string _contextualId)
        {
            if (!_activeContextualTabs.ContainsKey(_contextualId))
            {
                ContextualRibbonTab _contextualTab = (ContextualRibbonTab)Ribbon?.Tabs?.FirstOrDefault(t => t.Id == _contextualId && t is ContextualRibbonTab);
                if (_contextualTab == null) // We dont need to draw or loop for tabs that does not exists anymore
                    return;
                _activeContextualTabs.Add(_contextualId, _contextualTab);
            }
        }

        private void OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)  // Case that happens when AutoCAD's main thread is occupied
                                    // and event was fired in the middle of cleaning up databases
                                    // [bug at: Autodesk AutoCAD 2017 #11387]
                return;
            AcApp.Document document = AcApp.Core.Application.DocumentManager.MdiActiveDocument;
            PromptSelectionResult result = document.Editor.SelectImplied();
            if (result.Status != PromptStatus.OK || result.Value == null || result.Value.Count == 0)
            {
                foreach (string Id in _contextualTabConditions.Keys)
                {
                    RibbonTab _contextualTab = Ribbon?.Tabs?.FirstOrDefault(t => t.Id == Id);
                    if (_contextualTab != null)
                    {
                        Ribbon?.HideContextualTab(_contextualTab);
                        _contextualTab.IsVisible = false;
                        _activeContextualTabs.Remove(Id);
                    }
                }
                return;
            }
            SelectionSet selection = result.Value;
            if (selection != null)
            {
                foreach (KeyValuePair<string, Func<SelectionSet, bool>> pair in _contextualTabConditions)
                {
                    if (pair.Value == null)
                        continue; // If for some reason Func<SelectionSet, bool>> will be null during tab creation
                                  // we will just skip handling this tab and treat it as normal one
                    ContextualRibbonTab _contextualTab;
                    if (_activeContextualTabs.ContainsKey(pair.Key)) _contextualTab = _activeContextualTabs[pair.Key];
                    else _contextualTab = (ContextualRibbonTab)Ribbon?.Tabs?.FirstOrDefault(t => t.Id == pair.Key && t is ContextualRibbonTab);
                    if (_contextualTab == null) // We dont need to draw or loop for tabs that does not exists anymore
                        continue;
                    if (pair.Value.Invoke(selection))
                    {
                        if (!_activeContextualTabs.ContainsKey(pair.Key))
                            _activeContextualTabs.Add(pair.Key, _contextualTab);
                    }
                    else
                    {
                        Ribbon?.HideContextualTab(_contextualTab);
                        _contextualTab.IsVisible = false;
                        _activeContextualTabs.Remove(pair.Key);
                    }
                }
            }
        }

        private void OnApplicationIdle(object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)  // Case that happens when AutoCAD's main thread is occupied
                                    // and event was fired in the middle of cleaning up databases
                                    // [bug at: Autodesk AutoCAD 2017 #11387]
                return;
            foreach (ContextualRibbonTab _contextualTab in _activeContextualTabs.Values)
            {
                if (!_contextualTab.IsContextualTab)
                {
                    _contextualTab.IsVisible = true;
                    _contextualTab.IsContextualTab = true;
                }
                if (!_contextualTab.IsVisible)
                {
                    Ribbon?.ShowContextualTab(_contextualTab, false, true);
                    _contextualTab.IsActive = true;
                }
            }
        }

        private RibbonItem ProcessRibbonItem(RibbonItemDef itemDef, RibbonPanelDef panelDef, string cookie,
            int currentDepth = 0) // this signalizes how many hops had happend during reccursion,
                                  // we don't want to be stack-overflowed, so depth is actually checked limited
        { 
            if (currentDepth < 4 || RibbonItemDef.ItemsFactory.ContainsKey(itemDef.GetType()))
            {
                itemDef.Cookie = itemDef.Cookie.Replace("%Parent", cookie);
                RibbonItem itemRef = itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]());
                switch (itemDef)
                {
                    case RibbonRowPanelDef item:
                        List<RibbonItemDef> children;
                        if (item.m_Source != null && item.m_Items.Count != 0)
                        {
                            item.m_Source.Cookie = item.m_Source.Cookie.Replace("%Parent", item.Id);
                            cookie += $";{item.m_Source.Id}";
                            item.m_Source.m_Items.AddRange(item.m_Items);
                            children = item.m_Source.m_Items;
                            ((RibbonRowPanel)itemRef).Source = item.m_Source.Transform(new RibbonSubPanelSource()); // Not sure if transformation should happen here
                        }
                        else
                        {
                            children = item.m_Items;
                        } 
                        RibbonItemCollection target = ((RibbonRowPanel)itemRef).Source?.Items ?? ((RibbonRowPanel)itemRef).Items;
                        foreach (RibbonItemDef childDef in children)
                        {
                            // The following item types are not supported in this collection: RibbonRowPanel and RibbonPanelBreak
                            if (childDef is RibbonRowPanelDef || childDef is RibbonPanelBreakDef)
                                continue;
                            RibbonItem childRef = ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                            if (childRef != null)
                                target.Add(childRef);
                        }
                        break;
                    case RibbonListDef.RibbonComboDef item:
                        // If ItemsBinding is set to a valid binding, this collection should not be modified
                        // An exception is thrown if the Items collection is modified when ItemsBinding is not null
                        if (((RibbonList)itemRef).ItemsBinding == null && item.m_Items.Count > 0)
                        {
                            // Either Items or ItemsBinding can be used to manage the collection, but not both
                            foreach (RibbonItemDef childDef in item.m_Items)
                            {
                                RibbonItem childRef = ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                                if (childRef != null)
                                    ((RibbonList)itemRef).Items.Add(childRef);
                            }
                        }
                        foreach (RibbonItemDef childDef in item.m_MenuItems)
                        {
                            RibbonCommandItem childRef = (RibbonCommandItem)ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                            if (childRef != null)
                                ((RibbonCombo)itemRef).MenuItems.Add(childRef);
                        }
                        break;
                    case RibbonListButtonDef item:

                        foreach (RibbonItemDef childDef in item.m_Items)
                        {
                            // Set of rules for each implementation of RibbonListButtonDef
                            switch (item)
                            {
                                case RibbonListButtonDef.RibbonMenuButtonDef _:
                                    if (!(childDef is RibbonMenuItemDef) && !(childDef is RibbonSeparatorDef))
                                        continue;
                                    break;
                                case RibbonListButtonDef.RibbonRadioButtonGroupDef _:
                                    if (!(childDef is RibbonToggleButtonDef))
                                        continue;
                                    break;
                                default:
                                    if (!(childDef is RibbonCommandItemDef) && !(childDef is RibbonSeparatorDef))
                                        continue;
                                    break;
                            }
                            RibbonItem childRef = ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                            if (childRef != null)
                                ((RibbonListButton)itemRef).Items.Add(childRef);
                        }
                        break;
                        { } // Little C# hack for better memory management
                }
                RegisterControl(itemRef, itemDef);
                return itemRef;
            }
            return null;
        }

        private void RegisterControl(object itemRef, BaseRibbonXml itemDef)
        {
            if (!string.IsNullOrEmpty(itemDef.UUID) && !_registeredControls.ContainsKey(itemDef.UUID))
            {
                Type wrapperType = Assembly.GetExecutingAssembly()
                    .GetType($"{ControlsNamespace}.{itemDef.Id}", false, true);
                if (wrapperType != null)
                {
                    try
                    {
                        ConstructorInfo constructor = wrapperType.GetConstructors()?
                            .FirstOrDefault(c =>
                            {
                                ParameterInfo[] parameters = c.GetParameters();
                                return parameters.Length == 2
                                    && parameters[0].ParameterType.IsAssignableFrom(itemRef.GetType())
                                    && parameters[1].ParameterType.IsAssignableFrom(itemDef.GetType());
                            });
                        if (constructor != null)
                        {
                            object invoke = constructor.Invoke(new object[] { itemRef, itemDef });
                            if (invoke != null)
                                _registeredControls.Add(itemDef.UUID, invoke);
                        }
                    }
                    catch (System.Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine($"[&] Problem while registering {itemDef.GetType()}/{itemDef.Id}\n" +
                            $"Control: {ControlsNamespace}.{itemDef.Id}\n" +
                            $"Message: {exception.Message}\n" +
                            $"--- END OF STACK");
                    }
                }
            }
        }

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
        private void LogException(System.Exception exception)
        {
            // Log all nested exceptions
            int currentNest = 1;
            System.Exception currentException = exception;
            while (currentException != null)
            {
                System.Diagnostics.Debug.WriteLine($"[&] {currentNest}:(InvalidOperationException): {currentException.Message}");
                currentException = currentException.InnerException;
                currentNest++;
            }
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
            private string _controlsNamespace;

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

            public RibbonRegistry SetControlsNamespace(string controlsNamespace)
            {
                _controlsNamespace = controlsNamespace;
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
            public RibbonDef Build(Assembly executingAssembly)
            {
                RibbonDef def = new RibbonDef(executingAssembly ?? Assembly.GetCallingAssembly(),
                                             _controlsNamespace,
                                             new Dictionary<string, CommandHandler>(_handlers),
                                             _defaultHandler);
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