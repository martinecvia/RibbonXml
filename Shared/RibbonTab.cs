using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("System.Xml.Serialization")]
namespace RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab
    [XmlRoot("RibbonTab")]
    [Description("The class RibbonTab is used to store and manage the contents of a ribbon tab.")]
    public class RibbonTabDef
        : BaseRibbonXml
    {
        private string _cookie;
        public override string Cookie
        {
            get => _cookie ?? $"Tab={Id}_{Title}_{Name}";
            set => _cookie = value;
        }

        [XmlOut]
        [XmlAttribute("Title")]
        [DefaultValue(null)]
        [Description("Gets or sets the tab title. " +
            "The title set with this property is displayed in the tab button for this tab in the ribbon. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Title
        public string Title { get; set; } = null;

        [XmlOut]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of the ribbon tab. " +
            "The framework uses the Title property of the tab to display the tab title in the ribbon. " +
            "The name property is not currently used by the framework. " +
            "Applications can use this property to store a longer name for the tab if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Name
        public string Name { get; set; } = null;

        [XmlOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets a description text for the tab. " +
            "The description text is not currently used by the framework. " +
            "Applications can use this to store a description if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Description
        public string Description { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether this tab is the active tab. " +
            "Hidden tabs and merged contextual tabs cannot be the active tab. " +
            "Setting this property to true for such tabs will fail, and no exception will be thrown.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_IsActive
        public bool IsActive { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool IsPanelEnabled { get; set; } = true;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Assesses whether the tab is regular tab or contextual tab. " +
            "If it is true the tab is contextual tab, and false if it is regular tab. " +
            "This is a dependency property registered with WPF. " +
            "Please see the Microsoft API for more information. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_IsContextualTab
        public bool IsContextualTab { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsMergedContextualTab { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool AllowTearOffContextualPanels { get; set; } = false;

        [XmlOut]
        [XmlAttribute("KeyTip")]
        [DefaultValue(null)]
        [Description("Gets or sets the keytip for the tab. " +
            "Keytips are displayed in the ribbon when navigating the ribbon with the keyboard. " +
            "If this property is null or empty, the keytip will not appear for this tab, and the tab will not support activation through the keyboard. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_KeyTip
        public string KeyTip { get; set; } = null;

        [XmlOut]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        [Description("Gets or sets custom data object in the tab. " +
            "This property can be used to store any object as a custom data object in a tab. " +
            "This data is not used by the framework. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Tag
        public string Tag { get; set; } = null;

        #region INTERNALS
        #region CONTENT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlElement("RibbonPanel")]
        [Description("Gets the collection used to store the panels in the tab. " +
            "The default is an empty collection.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Panels
        internal List<RibbonPanelDef> m_Panels { get; set; } = new List<RibbonPanelDef>();
        #endregion

        [XmlElement("Title")]
        internal XmlCDataSection m_TitleCData
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return null;
                return new XmlDocument().CreateCDataSection(Title);
            }
            set { Title = value?.Value ?? Name; }
        }

        [XmlElement("Name")]
        internal XmlCDataSection m_NameCData
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;
                return new XmlDocument().CreateCDataSection(Name);
            }
            set { Name = value?.Value ?? Id; }
        }

        [XmlElement("Description")]
        internal XmlCDataSection m_DescriptionCData
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return null;
                return new XmlDocument().CreateCDataSection(Description);
            }
            set { Description = value?.Value ?? string.Empty; }
        }

        [XmlAttribute("IsActive")]
        internal string m_IsActiveSerializable
        {
            get => IsActive.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsActive = false;
                    return;
                }
                IsActive = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("IsEnabled")]
        internal string m_IsEnabledSerializable
        {
            get => IsEnabled.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsEnabled = true;
                    return;
                }
                IsEnabled = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("IsPanelEnabled")]
        internal string m_IsPanelEnabledSerializable
        {
            get => IsPanelEnabled.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsPanelEnabled = true;
                    return;
                }
                IsPanelEnabled = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("IsContextualTab")]
        internal string m_IsContextualTabSerializable
        {
            get => IsContextualTab.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsContextualTab = false;
                    return;
                }
                IsContextualTab = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("IsMergedContextualTab")]
        internal string m_IsMergedContextualTabSerializable
        {
            get => IsMergedContextualTab.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsMergedContextualTab = false;
                    return;
                }
                IsMergedContextualTab = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("AllowTearOffContextualPanels")]
        internal string m_AllowTearOffContextualPanelsSerializable
        {
            get => AllowTearOffContextualPanels.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    AllowTearOffContextualPanels = false;
                    return;
                }
                AllowTearOffContextualPanels = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlElement("Tag")]
        internal XmlCDataSection m_TagCData
        {
            get
            {
                if (string.IsNullOrEmpty(Tag))
                    return null;
                return new XmlDocument().CreateCDataSection(Tag);
            }
            set { Tag = value?.Value; }
        }
        #endregion
    }
}