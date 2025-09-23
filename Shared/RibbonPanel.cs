using System; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml;
using System.Windows.Markup;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

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
            get => _cookie ?? $"%Parent:Panel={SourceDef?.Id}_{SourceDef?.Title}_{SourceDef?.Name}";
            set => _cookie = value;
        }

        #region CONTENT
        /*
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the source that contains the ribbon items to be displayed by this panel. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_Source
        public RibbonPanelSource Source => SourceDef != null
            ? SourceDef.Transform(RibbonPanelSourceDef.SourceFactory[SourceDef.GetType()]()) : null;
        */

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlElement("RibbonPanelSource", typeof(RibbonPanelSourceDef))]
        [XmlElement("RibbonPanelSpacer", typeof(RibbonPanelSourceDef.RibbonPanelSpacerDef))]
        public RibbonPanelSourceDef SourceDef { get; set; } = null;
        #endregion

        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        public string Tag { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomPanelBackground { get; set; } = null;

        [XmlElement("CustomPanelBackground")]
        public XmlElement CustomPanelBackgroundDef
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

        /* 
         * Creation:
            <RibbonPanelSpacerDef xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                    xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                <LeftBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FFFF0000" /&gt;
                </LeftBorderBrush>
                <RightBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FF0000FF" /&gt;
                </RightBorderBrush>
            </RibbonPanelSpacerDef>
        */
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomSlideOutPanelBackground { get; set; } = null;

        [XmlElement("CustomSlideOutPanelBackground")]
        public XmlElement CustomSlideOutPanelBackgroundDef
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

        /* 
         * Creation:
            <RibbonPanelSpacerDef xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                    xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                <LeftBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FFFF0000" /&gt;
                </LeftBorderBrush>
                <RightBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FF0000FF" /&gt;
                </RightBorderBrush>
            </RibbonPanelSpacerDef>
        */
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomPanelTitleBarBackground { get; set; } = null;

        [XmlElement("CustomPanelTitleBarBackground")]
        public XmlElement CustomPanelTitleBarBackgroundDef
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

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightPanelTitleBar
        public bool HighlightPanelTitleBar { get; set; } = false;

        [XmlAttribute("HighlightPanelTitleBar")]
        public string HighlightPanelTitleBarDef
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

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightWhenCollapsed
        public bool HighlightWhenCollapsed { get; set; } = false;

        [XmlAttribute("HighlightWhenCollapsed")]
        public string HighlightWhenCollapsedDef
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

        [XmlAttribute("IsVisible")]
        public string IsVisibleDef
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

        [XmlAttribute("IsEnabled")]
        public string IsEnabledDef
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

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(RibbonResizeStyles.None)]
        public RibbonResizeStyles ResizeStyle { get; set; } = RibbonResizeStyles.None;

        [XmlAttribute("ResizeStyle")]
        public string ResizeStyleDef
        {
            get => ResizeStyle.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonResizeStyles result))
                    result = RibbonResizeStyles.None;
                ResizeStyle = result;
            }
        }

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

        [XmlAttribute("FloatingOrientation")]
        public string FloatingOrientationDef
        {
            get => FloatingOrientation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                    result = System.Windows.Controls.Orientation.Horizontal;
                FloatingOrientation = result;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool CanToggleOrientation { get; set; } = true;

        [XmlAttribute("CanToggleOrientation")]
        public string CanToggleOrientationDef
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

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsContextualTabThemeIgnored { get; set; } = false;

        [XmlAttribute("IsContextualTabThemeIgnored")]
        public string IsContextualTabThemeIgnoredDef
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
    }
}