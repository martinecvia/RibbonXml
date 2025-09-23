using System; // Keep for .NET 4.6
using System.Xml;
using System.ComponentModel;
using System.Collections.Generic; // Keep for .NET 4.6
using System.Windows.Markup;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

using RibbonXml.Items;
using RibbonXml.Items.CommandItems;

namespace RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource
    [XmlInclude(typeof(RibbonPanelSourceDef))]
    [XmlInclude(typeof(RibbonPanelSpacerDef))]
    [Description("The RibbonPanelSource class is used to store and manage the content of a panel in a ribbon. " +
        "RibbonPanel references an object of this class in its Source property and displays the content from this class. " +
        "The content is a collection of RibbonItem objects stored in an Items collection. " +
        "The items can be organized into multiple rows by adding a RibbonRowBreak item at the index at where a new row is to start. " +
        "The items can also be organized into two panels - main panel and slide-out panel - by adding a RibbonPanelBreak item at the index where the slide-out panel is to start.")]
    public class RibbonPanelSourceDef : BaseRibbonXml
    {
        private string _cookie;
        public override string Cookie
        {
            get => _cookie ?? $"%Parent:PanelSource={Id}_{Title}_{Name}";
            set => _cookie = value;
        }

        #region CONTENT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // RibbonItem
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
        [XmlElement("RibbonSeparator", typeof(RibbonSeparatorDef))]
        [XmlElement("RibbonSlider", typeof(RibbonSliderDef))]
        [XmlElement("RibbonSpinner", typeof(RibbonSpinnerDef))]
        [XmlElement("RibbonTextBox", typeof(RibbonTextBoxDef))]
        // RibbonCommandItem
#if !ZWCAD
        [XmlElement("ProgressBarSource", typeof(ProgressBarSourceDef))]
#endif
        [XmlElement("RibbonCheckBox", typeof(RibbonCheckBoxDef))]
        [XmlElement("RibbonMenuItem", typeof(RibbonMenuItemDef))]
        [XmlElement("ApplicationMenuItem", typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
        // RibbonButton
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonToggleButton", typeof(RibbonToggleButtonDef))]
#if (NET8_0_OR_GREATER || ZWCAD)
        [XmlElement("ToolBarShareButton", typeof(RibbonToggleButtonDef.ToolBarShareButtonDef))]
#endif
        [XmlElement("RibbonChecklistButton", typeof(RibbonListButtonDef.RibbonChecklistButtonDef))]
        [XmlElement("RibbonMenuButton", typeof(RibbonListButtonDef.RibbonMenuButtonDef))]
        [XmlElement("RibbonRadioButtonGroup", typeof(RibbonListButtonDef.RibbonRadioButtonGroupDef))]
        [XmlElement("RibbonSplitButton", typeof(RibbonListButtonDef.RibbonSplitButtonDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();
        #endregion

        [XmlOut]
        [XmlAttribute("Title")]
        [DefaultValue(null)]
        [Description("The panel title set with this property is displayed in the panel's title bar in the ribbon. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Title
        public string Title { get; set; } = null;

        [XmlElement("Title")]
        public XmlCDataSection TitleCData
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return null;
                return new XmlDocument().CreateCDataSection(Title);
            }
            set { Title = value?.Value; }
        }

        [XmlOut]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of the ribbon panel. " +
            "The framework uses the Title property of the panel to display the panel title in the ribbon. " +
            "The name property is not currently used by the framework. " +
            "Applications can use this property to store a longer name for a panel if this is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Name
        public string Name { get; set; } = null;

        [XmlElement("Name")]
        public XmlCDataSection NameCData
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;
                return new XmlDocument().CreateCDataSection(Name);
            }
            set { Name = value?.Value; }
        }

        [XmlOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets the panel description text. " +
            "The description text is not currently used by the framework. " +
            "Applications can use this to store a description if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Description
        public string Description { get; set; } = null;

        [XmlElement("Description")]
        public XmlCDataSection DescriptionCData
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return null;
                return new XmlDocument().CreateCDataSection(Description);
            }
            set { Description = value?.Value; }
        }

        [XmlOut]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        [Description("Gets or sets the custom data object in the panel source. " +
            "This property can be used to store any object a as custom data object in a panel source. " +
            "This data is not used by the framework. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Tag
        public string Tag { get; set; } = null;

        #region CONTENT
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the command item to be used as the panel's dialog launcher. " +
            "The dialog launcher is displayed as a small button in the panel title bar. " +
            "Clicking the button raises a command that follows the standard ribbon command routing. " +
            "If this property is null the panel does not have a dialog launcher button. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_DialogLauncher
        public RibbonCommandItem DialogLauncher
        {
            get
            {
                // It must be RibbonButton if you want it to work
                RibbonButton button = DialogLauncherDef?.Transform(new RibbonButton());
                if (button != null)
                {
                    // DialogLauncher depends on this, thus must be changed
                    button.MinWidth = 0;
                    button.Width = double.NaN;
                }
                return button;
            }
        }

        [XmlElement("DialogLauncher")]
        public RibbonButtonDef DialogLauncherDef { get; set; } = null;
        #endregion

        [XmlOut]
        [XmlAttribute("KeyTip")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of the ribbon panel. " +
            "The framework uses the Title property of the panel to display the panel title in the ribbon. " +
            "The name property is not currently used by the framework. " +
            "Applications can use this property to store a longer name for a panel if this is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_KeyTip
        public string KeyTip { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool IsSlideOutPanelVisible { get; set; } = true;

        [XmlAttribute("IsSlideOutPanelVisible")]
        public string IsSlideOutPanelVisibleDef
        {
            get => IsSlideOutPanelVisible.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsSlideOutPanelVisible = true;
                    return;
                }
                IsSlideOutPanelVisible
                    = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer
        public class RibbonPanelSpacerDef : RibbonPanelSourceDef
        {
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
            [DefaultValue("Transparent")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer_LeftBorderBrush
            public System.Windows.Media.Brush LeftBorderBrush { get; set; } = System.Windows.Media.Brushes.Transparent;

            [XmlElement("LeftBorderBrush")]
            public XmlElement LeftBorderBrushDef
            {
                get
                {
                    if (LeftBorderBrush == null)
                        return null;
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(XamlWriter.Save(LeftBorderBrush));
                    return document.DocumentElement;
                }
                set
                {
                    if (value != null)
                    {
                        string xaml = value.OuterXml;
                        LeftBorderBrush = (System.Windows.Media.Brush)XamlReader.Parse(xaml);
                    }
                    else
                    {
                        LeftBorderBrush = System.Windows.Media.Brushes.Transparent;
                    }
                }
            }

            [XmlOut]
            [XmlIgnore]
            [DefaultValue("Transparent")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer_RightBorderBrush
            public System.Windows.Media.Brush RightBorderBrush { get; set; } = System.Windows.Media.Brushes.Transparent;

            [XmlElement("RightBorderBrush")]
            public XmlElement RightBorderBrushDef
            {
                get
                {
                    if (RightBorderBrush == null)
                        return null;
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(XamlWriter.Save(RightBorderBrush));
                    return document.DocumentElement;
                }
                set
                {
                    if (value != null)
                    {
                        string xaml = value.OuterXml;
                        RightBorderBrush = (System.Windows.Media.Brush)XamlReader.Parse(xaml);
                    }
                    else
                    {
                        RightBorderBrush = System.Windows.Media.Brushes.Transparent;
                    }
                }
            }
        }

        [XmlIgnore]
        public static readonly Dictionary<Type, Func<RibbonPanelSource>> SourceFactory = new Dictionary<Type, Func<RibbonPanelSource>>()
        {
            { typeof(RibbonPanelSourceDef), () => new RibbonPanelSource() },
            { typeof(RibbonPanelSpacerDef), () => new RibbonPanelSpacer() },
        };
    }
}