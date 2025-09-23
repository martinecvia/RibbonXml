using System; // Keep for .NET 4.6
using System.Collections.Generic;// Keep for .NET 4.6
using System.ComponentModel;
using System.Xml.Serialization;

namespace RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonMenuItem
    [Description("This class is used to support hierarchical items. " +
        "This class is used in places where a hierarchy is supported. " +
        "Examples are application menu and RibbonMenuItem.")]
    [XmlInclude(typeof(RibbonMenuItemDef))]
    [XmlInclude(typeof(ApplicationMenuItemDef))]
    public class RibbonMenuItemDef : RibbonCommandItemDef
    {
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
        [XmlElement("ApplicationMenuItem", typeof(ApplicationMenuItemDef))]
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

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem
        [Description("The ApplicationMenuItem class is used to manage a single menu item and its sub-menu items in a hierarchical structure in the application menu. " +
            "This class supports the following menu item types: " +
            "- regular menu item, which executes a command. " +
            "- split button type menu item. " +
            "- popup menu item, which opens a sub-menu. " +
            "The split button type menu item is supported only in the first-level menu.")]
        public class ApplicationMenuItemDef : RibbonMenuItemDef
        {
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(false)]
            [Description("Gets or sets the value indicating whether or not the menu item is to be displayed as a split button type menu item. " +
                "If the value is true the menu item is displayed as a split button. " +
                "Clicking the left portion of the split button executes the item and clicking the right portion of the button opens the next sub-level menu. " +
                "This property is applicable only for main menu items (first-level menu items) and is ignored when set in menu items in a second or subsequent level. " +
                "Setting this value to true does not make sense if the menu item does not have a sub-menu. " +
                "The default value is false.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_IsSplit
            public bool IsSplit { get; set; } = false;

            [XmlAttribute("IsSplit")]
            public string IsSplitDef
            {
                get => IsSplit.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        // Passing the default value
                        IsSplit = false;
                        return;
                    }
                    IsSplit = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
                }
            }

            [XmlOut]
            [XmlAttribute("SplitKeyTip")]
            [DefaultValue(null)]
            [Description("Gets or sets the keytip for the right portion of the split button menu item which is used to open the next level menu. " +
                "This property is applicable only for split buttons (i.e. IsSplit=true). " +
                "The default value is null.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_SplitKeyTip
            public string SplitKeyTip { get; set; } = null;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(false)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_IsPinable
            public bool IsPinable { get; set; } = false;

            [XmlAttribute("IsPinable")]
            public string IsPinableDef
            {
                get => IsPinable.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        // Passing the default value
                        IsPinable = false;
                        return;
                    }
                    IsPinable = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
                }
            }

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(0)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_MaxDescriptionLines
            public int MaxDescriptionLines { get; set; } = 0;

            [XmlAttribute("MaxDescriptionLines")]
            public string MaxDescriptionLinesDef
            {
                get => MaxDescriptionLines.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxDescriptionLines = x;
                    else
                    {
                        MaxDescriptionLines = 0;
                    }
                }
            }
        }
    }
}