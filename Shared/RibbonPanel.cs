using System; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml;
using System.Windows.Markup;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

[assembly: InternalsVisibleTo("System.Xml.Serialization")]
namespace RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel
    [Description("The class RibbonPanel is used to store and manage the panel in a ribbon. " +
        "RibbonPanel displays the content of the RibbonPanelSource set in the Source property.")]
    public class RibbonPanelDef : BaseRibbonXml
    {
        private string _cookie;
        public override string Cookie
        {
            get => _cookie ?? $"%Parent:Panel={m_Source?.Id}_{m_Source?.Title}_{m_Source?.Name}";
            set => _cookie = value;
        }

        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        public string Tag { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomPanelBackground { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomSlideOutPanelBackground { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomPanelTitleBarBackground { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightPanelTitleBar
        public bool HighlightPanelTitleBar { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightWhenCollapsed
        public bool HighlightWhenCollapsed { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("If the value is true, the panel is enabled. " +
            "If the value is false, the panel is disabled. " +
            "When a panel is disabled all the items in the panel are disabled. " +
            "The default value is true. " +
            "To disable all panels in a tab use RibbonTab.IsPanelEnabled. " +
            "" +
            "If the value is true, the panel is visible in the ribbon. " +
            "If the value is false, it is hidden in the ribbon. " +
            "Both visible and hidden panels of a tab are available in the ribbon's right-click menu under the Panels menu option, which allows the user to show or hide the panels. " +
            "If the panel's IsAnonymous property is set to false, it is not included in the right-click menu and the user cannot control its visibility. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_IsVisible
        public bool IsVisible { get; set; } = true;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value that indicates whether this panel is enabled. " +
            "If the value is true, the panel is enabled. " +
            "If the value is false, the panel is disabled. " +
            "When a panel is disabled all the items in the panel are disabled. " +
            "The default value is true. " +
            "To disable all panels in a tab use RibbonTab.IsPanelEnabled.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_IsEnabled
        public bool IsEnabled { get; set; } = true;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool CanToggleOrientation { get; set; } = true;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(RibbonResizeStyles.None)]
        public RibbonResizeStyles ResizeStyle { get; set; } = RibbonResizeStyles.None;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Vertical)]
        [Description("Gets or sets the orientation to be used when the panel is floating. " +
            "This property is applicable only when the panel is floating. " +
            "The orientation of a floating panel can be horizontal or vertical. " +
            "The orientation can be switched by the user with the Orientation button in the panel frame. " +
            "Set the CanToggleOrientation property to false to hide the Orientation button and hinder the user from changing the orientation.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_FloatingOrientation
        public System.Windows.Controls.Orientation FloatingOrientation { get; set; } = System.Windows.Controls.Orientation.Vertical;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsContextualTabThemeIgnored { get; set; } = false;

        #region INTERNALS
        #region CONTENT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlElement("RibbonPanelSource", typeof(RibbonPanelSourceDef))]
        [XmlElement("RibbonPanelSpacer", typeof(RibbonPanelSourceDef.RibbonPanelSpacerDef))]
        internal RibbonPanelSourceDef m_Source { get; set; } = null;
        #endregion

        [XmlElement("CustomPanelBackground")]
        internal XmlElement m_CustomPanelBackgroundSerializable
        {
            get
            {
                if (CustomPanelBackground == null)
                    return null;
                XmlDocument document = new XmlDocument();
                document.LoadXml(XamlWriter.Save(CustomPanelBackground));
                return document.DocumentElement;
            }
            set
            {
                if (value != null)
                {
                    string xaml = value.OuterXml;
                    CustomPanelBackground = (System.Windows.Media.Brush)XamlReader.Parse(xaml);
                }
                else
                {
                    CustomPanelBackground = null;
                }
            }
        }

        [XmlElement("CustomSlideOutPanelBackground")]
        internal XmlElement m_CustomSlideOutPanelBackgroundSerializable
        {
            get
            {
                if (CustomSlideOutPanelBackground == null)
                    return null;
                XmlDocument document = new XmlDocument();
                document.LoadXml(XamlWriter.Save(CustomSlideOutPanelBackground));
                return document.DocumentElement;
            }
            set
            {
                if (value != null)
                {
                    string xaml = value.OuterXml;
                    CustomSlideOutPanelBackground = (System.Windows.Media.Brush)XamlReader.Parse(xaml);
                }
                else
                {
                    CustomSlideOutPanelBackground = null;
                }
            }
        }

        [XmlElement("CustomPanelTitleBarBackground")]
        internal XmlElement m_CustomPanelTitleBarBackgroundSerializable
        {
            get
            {
                if (CustomPanelTitleBarBackground == null)
                    return null;
                XmlDocument document = new XmlDocument();
                document.LoadXml(XamlWriter.Save(CustomPanelTitleBarBackground));
                return document.DocumentElement;
            }
            set
            {
                if (value != null)
                {
                    string xaml = value.OuterXml;
                    CustomPanelTitleBarBackground = (System.Windows.Media.Brush)XamlReader.Parse(xaml);
                }
                else
                {
                    CustomPanelTitleBarBackground = null;
                }
            }
        }

        [XmlAttribute("HighlightPanelTitleBar")]
        internal string m_HighlightPanelTitleBarSerializable
        {
            get => HighlightPanelTitleBar.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    HighlightPanelTitleBar = false;
                    return;
                }
                HighlightPanelTitleBar
                    = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("HighlightWhenCollapsed")]
        internal string m_HighlightWhenCollapsedSerializable
        {
            get => HighlightWhenCollapsed.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    HighlightWhenCollapsed = false;
                    return;
                }
                HighlightWhenCollapsed
                    = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("IsVisible")]
        internal string m_IsVisibleSerializable
        {
            get => IsVisible.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsVisible = true;
                    return;
                }
                IsVisible = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
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

        [XmlAttribute("ResizeStyle")]
        internal string m_ResizeStyleSerializable
        {
            get => ResizeStyle.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonResizeStyles result))
                    result = RibbonResizeStyles.None;
                ResizeStyle = result;
            }
        }

        [XmlAttribute("FloatingOrientation")]
        internal string m_FloatingOrientationSerializable
        {
            get => FloatingOrientation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                    result = System.Windows.Controls.Orientation.Horizontal;
                FloatingOrientation = result;
            }
        }

        [XmlAttribute("CanToggleOrientation")]
        internal string m_CanToggleOrientationSerializable
        {
            get => CanToggleOrientation.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    CanToggleOrientation = true;
                    return;
                }
                CanToggleOrientation
                    = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("IsContextualTabThemeIgnored")]
        internal string m_IsContextualTabThemeIgnoredSerializable
        {
            get => IsContextualTabThemeIgnored.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsContextualTabThemeIgnored = false;
                    return;
                }
                IsContextualTabThemeIgnored
                    = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }
        #endregion
    }
}