#define NON_VOLATILE_MEMORY

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.Linq; // Keep for .NET 4.6
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;

using System.Diagnostics;

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
using System.Security.Policy;

namespace RibbonXml
{
    public class RibbonXml
    {
        public const string RibbonTab__Prefix = "RP_TAB_";  // Our prefix for tabs. so we can distinguish
                                                            // other tab's from AutoCAD.
                                                            // This also prevents using the same name from different applications.
        #region CONSTRUCTOR
        private readonly Assembly _lsAssembly;
        private readonly IReadOnlyDictionary<string, Type> _hwControllers;
        private readonly IReadOnlyDictionary<string, CommandHandler> _hwHandlers;
        private readonly Dictionary<string, BitmapImage> _mlImages;
        private readonly Type _tPCommandHandler;
        internal RibbonXml(
            Assembly lsAssembly,
            Dictionary<string, Type> hwControllers,
            Dictionary<string, CommandHandler> hwHandlers,
            Dictionary<string, BitmapImage> mlImages,
            Type hWCommandHandler = null)
        {
            _lsAssembly = lsAssembly;
            _hwControllers = hwControllers;
            _hwHandlers = hwHandlers;
            _mlImages = mlImages;
            _tPCommandHandler = hWCommandHandler;
        }
        #endregion

        private readonly Dictionary<string, ContextualRibbonTab> _activeContextualTabs = new Dictionary<string, ContextualRibbonTab>();
        private readonly Dictionary<Tuple<string, string>, Func<SelectionSet, bool>> _contextualTabConditions
            = new Dictionary<Tuple<string, string>, Func<SelectionSet, bool>>();
        private readonly List<RibbonTab> _tabs = new List<RibbonTab>();

        private RibbonControl Ribbon => ComponentManager.Ribbon;

        /// <summary>
        /// Gets the list of ribbon tabs (read-only). 
        /// You can add or remove tabs using AddTab/RemoveTab methods.
        /// </summary>
        public IReadOnlyList<RibbonTab> Tabs => _tabs.AsReadOnly();

        public RibbonTab CreateTab(string tabId, string tabName = null, string tabDescription = null)
            => CreateTab<RibbonTab>(tabId, tabName, tabDescription);

        public ContextualRibbonTab CreateContextualTab(string tabId, string tabName = null, string tabDescription = null)
            => CreateContextualTab<ContextualRibbonTab>(tabId, tabName, tabDescription);

        public ContextualRibbonTab CreateContextualTab(string tabId,
            Func<SelectionSet, bool> onSelectionMatch,
            string tabName = null,
            string tabDescription = null)
        {
            ContextualRibbonTab tab = CreateContextualTab<ContextualRibbonTab>(tabId, tabName, tabDescription);
            if (onSelectionMatch != null)
            {
                var selectionId = Tuple.Create(tab.Id, $"Selection={onSelectionMatch.GetHashCode()})");
                if (_contextualTabConditions.Count == 0)
                    AcApp.Core.Application.DocumentManager.MdiActiveDocument.ImpliedSelectionChanged += OnSelectionChanged;
                if (!_contextualTabConditions.ContainsKey(selectionId))
                    _contextualTabConditions.Add(selectionId, onSelectionMatch);
            }
            return tab;
        }

        private volatile bool _hasContextual = false;
        public T CreateContextualTab<T>(string tabId,
                                        string tabName = null,
                                        string tabDescription = null) where T : ContextualRibbonTab, new()
        {
            string Id = tabId ?? throw new ArgumentNullException(nameof(tabId));
            T tab = (T)Ribbon?.Tabs?
                .FirstOrDefault(t => t is T _contextualTab && t.Id == RibbonTab__Prefix + Id);
            if (tab != null)
                return tab;
            tab = CreateTab<T>(tabId, tabName, tabDescription);
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

        #region INTERNALS
        internal static System.Windows.Media.ImageSource GetImageSource(string resourceName,
            string fallback)
        {
            if (resourceName == null && fallback == null) return null;
            if (ptr == null) throw new InvalidOperationException("RibbonDef instance is not built yet.");
            // Return cached image if it exists
            resourceName = resourceName ?? fallback;
            // Resource was not found, but fallback was registered, returns fallback
            if (ptr._mlImages.TryGetValue(resourceName, out BitmapImage var0))
                return var0;
            if (fallback != null)
            {
                if (ptr._mlImages.TryGetValue(fallback, out BitmapImage var1))
                    return var1;
                Assembly lsAssembly = Assembly.GetExecutingAssembly();
                string fallbackPath = lsAssembly?.GetManifestResourceNames()
                    .FirstOrDefault(r => r.EndsWith(fallback, StringComparison.OrdinalIgnoreCase));
                if (fallbackPath != null)
                {
                    try
                    {
                        using (Stream stream = lsAssembly?.GetManifestResourceStream(fallbackPath))
                        {
                            if (stream == null) return null;
                            BitmapImage bitMap = new BitmapImage();
                            bitMap.BeginInit();
                            bitMap.StreamSource = stream;
                            bitMap.CacheOption = BitmapCacheOption.OnLoad;
                            bitMap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bitMap.EndInit();
                            // To make it thread safe and immutable
                            if (bitMap.CanFreeze)
                                bitMap.Freeze();
                            ptr._mlImages[Path.GetFileNameWithoutExtension(fallback)] = bitMap;
                            return bitMap;
                        }
                    } catch (Exception) { return null; }
                }
            }
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
            if (ptr == null)
                throw new InvalidOperationException("RibbonDef instance is not built yet.");
            if (ptr._hwHandlers.TryGetValue(command, out CommandHandler handler))
                return handler;
            try
            {
                if (ptr._tPCommandHandler != null)
                    return (CommandHandler)Activator.CreateInstance(ptr._tPCommandHandler, command);
            } catch (Exception) { }
            return new CommandHandler.CommandHandlerDef(command); // Default, which will just execute command
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

        private volatile bool _hasTab = false;
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
            T tab = (T)Ribbon?.Tabs?.FirstOrDefault(t => t.Id == RibbonTab__Prefix + Id);
            if (tab != null)
                return tab; // We really don't want to process same tab multiple times,
                            // there is no point in that
            RibbonTabDef tabDef = LoadResourceRibbon(Id);
            tab = tabDef?.Transform(new T()) ?? new T();
            Ribbon?.Tabs?.Add(tab);
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
                                                                                                            // RibbonItem null definitions will break CAD ribbon instance
                        if (itemRef != null)
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
                        if (_tabs.Count == 0 || Ribbon == null)
                            return;
                        // Defer to avoid reentrancy
                        Ribbon?.Dispatcher?.BeginInvoke(new Action(() =>
                        {
                            if (Ribbon == null) return;
                            foreach (RibbonTab reAdd in _tabs)
                            {
                                // If for some reason tab still exists, ignore it
                                if (Ribbon.Tabs.Contains(reAdd))
                                    continue;
                                Ribbon.Tabs.Add(reAdd);
                            }
                        }));
                    };
                    _hasTab = true;
                }
            }
            _tabs.Add(tab);
            return tab;
        }

        private RibbonTabDef LoadResourceRibbon(string resourceName)
        {
            try
            {
                string manifestResource = _lsAssembly.GetManifestResourceNames()
                    .FirstOrDefault(resource => resource.EndsWith($"{resourceName}.xml", StringComparison.OrdinalIgnoreCase));
                if (manifestResource == null) return default;
                using (Stream stream = _lsAssembly.GetManifestResourceStream(manifestResource))
                {
                    if (stream == null) return default;
                    try
                    {
                        Type type = typeof(RibbonTabDef);
                        XmlSerializer serializer = new XmlSerializer(type, new XmlRootAttribute("RibbonTab"));
                        return (RibbonTabDef)serializer.Deserialize(stream);
                    }
                    catch (Exception) { return default; }
                }
            } catch (Exception) { return default; }
        }

        private void HideContextualTab(string _contextualId)
        {
            if (_activeContextualTabs.ContainsKey(_contextualId)
                && _activeContextualTabs[_contextualId] is ContextualRibbonTab _contextualTab)
            {
                _contextualTab.ShowReasons.Clear();
                _activeContextualTabs.Remove(_contextualId);
                Ribbon?.HideContextualTab(_contextualTab);
            }
        }

        private void ShowContextualTab(string _contextualId, string reason = "_manual")
        {
            ContextualRibbonTab _contextualTab = (ContextualRibbonTab)Ribbon?.Tabs?.FirstOrDefault(t => t.Id == _contextualId && t is ContextualRibbonTab);
            if (_contextualTab == null) // We dont need to draw or loop for tabs that does not exists anymore
                return;
            _contextualTab.AddShowReason($"Manual({(string.IsNullOrEmpty(reason) ? "_manual" : reason)})");
            if (!_activeContextualTabs.ContainsKey(_contextualId))
                _activeContextualTabs.Add(_contextualId, _contextualTab);
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
                foreach (Tuple<string, string> key in _contextualTabConditions.Keys)
                {
                    ContextualRibbonTab _contextualTab = (ContextualRibbonTab)Ribbon?.Tabs?.FirstOrDefault(t => t.Id == key.Item1 && t is ContextualRibbonTab);
                    if (_contextualTab != null)
                    {
                        // No selection is implied
                        _contextualTab.ShowReasons.RemoveAll(r => r.StartsWith("Selection("));
                        if (_contextualTab.CanHide)
                        {
                            _activeContextualTabs.Remove(key.Item1);
                            Ribbon?.HideContextualTab(_contextualTab);
                        }
                    }
                }
                return;
            }
            SelectionSet selection = result.Value;
            if (selection != null)
            {
                foreach (KeyValuePair<Tuple<string, string>, Func<SelectionSet, bool>> pair in _contextualTabConditions)
                {
                    if (pair.Value == null)
                        continue; // If for some reason Func<SelectionSet, bool>> will be null during tab creation
                                  // we will just skip handling this tab and treat it as normal one
                    ContextualRibbonTab _contextualTab;
                    if (_activeContextualTabs.TryGetValue(pair.Key.Item1, out ContextualRibbonTab var0)) _contextualTab = var0;
                    else _contextualTab = (ContextualRibbonTab)Ribbon?.Tabs?.FirstOrDefault(t => t.Id == pair.Key.Item1 && t is ContextualRibbonTab);
                    if (_contextualTab == null) // We dont need to draw or loop for tabs that does not exists anymore
                        continue;
                    if (pair.Value.Invoke(selection))
                    {
                        _contextualTab.AddShowReason($"Selection({pair.Key.Item2})");
                        if (!_activeContextualTabs.ContainsKey(pair.Key.Item1))
                            _activeContextualTabs.Add(pair.Key.Item1, _contextualTab);
                    }
                    else
                    {
                        _contextualTab.DelShowReason($"Selection({pair.Key.Item2})");
                        if (_contextualTab.CanHide)
                        {
                            _activeContextualTabs.Remove(pair.Key.Item1);
                            Ribbon?.HideContextualTab(_contextualTab);
                        }
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

        private void RegisterControl(object itemRef, RibbonBase itemDef)
        {
            if (!string.IsNullOrEmpty(itemDef.Id) && _hwControllers.TryGetValue(itemDef.Id, out Type wrapperType))
            {
                try
                {
                    (wrapperType?.GetConstructors()?
                        .FirstOrDefault(ctor =>
                        {
                            ParameterInfo[] parameters = ctor.GetParameters();
                            return parameters.Length == 2
                                && parameters[0].ParameterType.IsAssignableFrom(itemRef.GetType())
                                && parameters[1].ParameterType.IsAssignableFrom(itemDef.GetType());
                        }))?.Invoke(new object[] { itemRef, itemDef });
                } catch (Exception) { }
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
        #endregion
        /// <summary>
        /// Represents a contextual Ribbon tab that can be shown for multiple reasons.
        /// The tab remains visible as long as there is at least one active reason.
        /// </summary>
        [XmlOut]
        public class ContextualRibbonTab : RibbonTab
        {
            /// <summary>
            /// Gets the list of reasons why this tab is currently shown.
            /// Each reason keeps the tab visible until it is removed or Hidden manually.
            /// </summary>
            public List<string> ShowReasons { get; private set; } = new List<string>();
            public virtual bool CanHide => ShowReasons.Count == 0;

            public void AddShowReason(string reason)
            {
                if (!string.IsNullOrWhiteSpace(reason) && !ShowReasons.Contains(reason))
                    ShowReasons.Add(reason);
            }

            public void DelShowReason(string reason)
            {
                if (!ShowReasons.Contains(reason))
                    return;
                ShowReasons.Remove(reason);
            }
        }
        private static RibbonXml ptr { get; set; }
        internal static RibbonXml DefPtr(RibbonXml m_Instance)
        {
            if (m_Instance == null)
                throw new InvalidOperationException("RibbonDef instance is not allowed to be built reflectively.");
            if (ptr == null)
                ptr = m_Instance;
            return ptr;
        }
    }
    #region BUILDER
    /// <summary>
    /// Builder for configuring ribbon tabs, command handlers, and images
    /// before creating a <see cref="RibbonXml"/> instance.
    /// </summary>
    public sealed class Builder
    {
        private readonly Dictionary<string, Type> _controls = new Dictionary<string, Type>();
        private readonly Dictionary<string, Tuple<string, BitmapImage>> _bitImages 
            = new Dictionary<string, Tuple<string, BitmapImage>>();
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
        /// The current <see cref="Builder"/> instance (for fluent API).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public Builder SetDefaultHandler(Type handler)
        {
            _defaultHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            return this;
        }

        /// <summary>
        /// Registers a command handler for a ribbon command.
        /// </summary>
        /// <param name="handler">The handler delegate associated with the command.</param>
        /// <returns>
        /// The current <see cref="Builder"/> instance (for fluent API).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public Builder RegisterCommandHandler(CommandHandler handler)
        {
            _handlers[handler.Command] = handler ?? throw new ArgumentNullException(nameof(handler));
            return this;
        }

        /// <summary>
        /// Registers a custom ribbon control type with a string identifier.
        /// </summary>
        /// <param name="Id">Unique identifier for the control.</param>
        /// <param name="control">Type of the control (must inherit from RibbonControl).</param>
        /// <returns>Current builder instance for fluent API.</returns>
        public Builder RegisterControlsType<T>(string Id, Type control)
        {
            if (string.IsNullOrEmpty(Id) && control == null
                && _controls.ContainsKey(Id) 
                && !typeof(RibbonControl).IsAssignableFrom(control))
            {
                return this; // Don't do anything
            }
            _controls[Id] = control;
            return this;
        }

        /// <summary>
        /// Registers an image from a file path, URI, or embedded resource.
        /// </summary>
        /// <param name="key">Unique key for the image.</param>
        /// <param name="lsPathOrURI">Path, URI, or resource name of the image.</param>
        /// <returns>Current builder instance.</returns>
        public Builder RegisterImage(string key, string lsPathOrURI)
        {
            if (!_bitImages.ContainsKey(key))
#pragma warning disable CS8619 // Typ odkazu s možnou hodnotou null v hodnotě neodpovídá cílovému typu.
                _bitImages[key] = Tuple.Create(lsPathOrURI, (BitmapImage)null);
#pragma warning restore CS8619 // Typ odkazu s možnou hodnotou null v hodnotě neodpovídá cílovému typu.
            return this;
        }

        /// <summary>
        /// Registers an already loaded <see cref="BitmapImage"/> with a key.
        /// </summary>
        /// <param name="key">Unique key for the image.</param>
        /// <param name="bitMap">BitmapImage instance.</param>
        /// <returns>Current builder instance.</returns>
        public Builder RegisterImage(string key, BitmapImage bitMap)
        {
            if (!_bitImages.ContainsKey(key))
#pragma warning disable CS8619 // Typ odkazu s možnou hodnotou null v hodnotě neodpovídá cílovému typu.
                _bitImages[key] = Tuple.Create((string)null, bitMap);
#pragma warning restore CS8619 // Typ odkazu s možnou hodnotou null v hodnotě neodpovídá cílovému typu.
            return this;
        }

        /// <summary>
        /// Builds a fully configured <see cref="RibbonXml"/> instance with all
        /// registered controls, command handlers, and images.
        /// </summary>
        /// <returns>New <see cref="RibbonXml"/> object.</returns>
        public RibbonXml Build()
        {
            Dictionary<string, BitmapImage> bitMaps = new Dictionary<string, BitmapImage>();
            Assembly assembly = Assembly.GetCallingAssembly() ?? Assembly.GetExecutingAssembly();
            foreach (KeyValuePair<string, Tuple<string, BitmapImage>> pair in _bitImages)
            {
                // Not sure how this can happen, but let's keep it safe
                if (pair.Value.Item1 == null && pair.Value.Item2 == null) continue;
                if (pair.Value.Item2 != null)
                {
                    // User passed valid BitMap
                    bitMaps[pair.Key] = pair.Value.Item2;
                    continue;
                }
                string lsURI = pair.Value.Item1;
                try
                {
                    BitmapImage bitMap;
                    // Handles IO.FilePath between UriKind.Absolute and UriKind.Relative
                    string lsPath = Path.IsPathRooted(lsURI)
                        ? lsURI
                        : Path.Combine(Path.GetDirectoryName(assembly.Location) ?? string.Empty, lsURI);
                    Uri URI = null;
                    if (File.Exists(lsPath))
                        URI = new Uri(lsPath, UriKind.Absolute);
                    else if (Uri.TryCreate(lsURI, UriKind.Absolute, out var var0) && var0.Scheme.StartsWith("http"))
                        URI = var0;
                    else if (lsURI.StartsWith("pack://"))
                        URI = new Uri(lsURI, UriKind.Absolute);

                    if (URI != null)
                    {
                        bitMap = new BitmapImage(URI);
                        bitMap.BeginInit();
                        bitMap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        bitMap.CacheOption = BitmapCacheOption.OnLoad;
                        bitMap.EndInit();
                        bitMap.Freeze();
                    }
                    else
                    {
                        using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{lsURI}"))
                        {
                            if (stream == null || stream.Length < 4)
                                continue;
                            byte[] header = new byte[4];
                            stream.Read(header, 0, header.Length);
                            // Checks if resolved image stream is a loadable image
                            if (!((header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) || // PNG (89 50 4E 47)
                                (header[0] == 0xFF && header[1] == 0xD8) ||                                             // JPG (FF D8)
                                (header[0] == 0x42 && header[1] == 0x4D) ||                                             // BMP (42 4D = "BM")
                                (header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x01 && header[3] == 0x00) ||   // ICO (00 00 01 00)
                                (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) ||   // GIF (ASCII "GIF8")
                                (header[0] == 0x49 && header[1] == 0x49 && header[2] == 0x2A) ||
                                (header[0] == 0x4D && header[1] == 0x4D && header[2] == 0x00 && header[3] == 0x2A)))    // TIFF ("II*" or "MM*")
                                continue;
                            stream.Position = 0;          // Resets the stream back to the position it was before read,
                                                            // this way we can check other formats not caught by header-types
                            bitMap = new BitmapImage();
                            bitMap.BeginInit();
                            bitMap.StreamSource = stream;
                            bitMap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bitMap.CacheOption = BitmapCacheOption.OnLoad;
                            bitMap.EndInit();
                            // To make it thread safe and immutable
                            bitMap.Freeze();
                        }
                    }
                    if (bitMap != null)
                        bitMaps[pair.Key] = bitMap;
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"[&RibbonXml] Failed to load image:\n" +
                        $"  Key: '{pair.Key}'\n" +
                        $"  Path: '{lsURI}'\n" +
                        $"  Message: {exception.Message}\n" +
                        $"--- END OF STACK");
                }
            }
            Debug.WriteLine($"Registered: {string.Join(",", bitMaps)}");
            RibbonXml def = new RibbonXml(
                assembly,
                new Dictionary<string, Type>(_controls),
                new Dictionary<string, CommandHandler>(_handlers),
                bitMaps,
                _defaultHandler
            );
            return RibbonXml.DefPtr(def);
        }
    }
    #endregion
}