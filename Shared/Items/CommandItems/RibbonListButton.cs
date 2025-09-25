using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

[assembly: InternalsVisibleTo("System.Xml.Serialization")]
namespace RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton
    [Description("This is an abstract base class for list type buttons. " +
        "List type buttons are buttons that support a drop-down list. " +
        "RibbonSplitButton, RibbonChecklistButton, RibbonMenuButton are examples of list type buttons.")]
    [XmlInclude(typeof(RibbonChecklistButtonDef))]
    [XmlInclude(typeof(RibbonMenuButtonDef))]
    [XmlInclude(typeof(RibbonRadioButtonGroupDef))]
    [XmlInclude(typeof(RibbonSplitButtonDef))]
    public abstract class RibbonListButtonDef : RibbonButtonDef
    {
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value that indicates whether the list button is to behave like a split button. " +
            "If this property is true, the list button supports executing the button without opening the drop-down list, and the drop-down list is opened by clicking the arrow. " +
            "If it is false, the list button always opens the drop-down list when clicked, and items need to be executed from the drop-down list. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton_IsSplit
        public bool IsSplit { get; set; } = true;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the drop-down list supports the grouping of items. " +
            "Grouping is accomplished by setting the property RibbonItem.GroupName for the drop-down items, so the items in the drop-down list should set the group name with that property. " +
            "If this property is true, grouping is enabled in the drop-down list. " +
            "If it is false, grouping is not enabled. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton_IsGrouping
        public bool IsGrouping { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(RibbonListButton.RibbonListButtonSynchronizeOption.All)]
        public RibbonListButton.RibbonListButtonSynchronizeOption SynchronizeOption { get; set; } = RibbonListButton.RibbonListButtonSynchronizeOption.All;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool AllowOrientation { get; set; } = false;

        #region INERNALS
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
        [XmlElement("RibbonChecklistButton", typeof(RibbonChecklistButtonDef))]
        [XmlElement("RibbonMenuButton", typeof(RibbonMenuButtonDef))]
        [XmlElement("RibbonRadioButtonGroup", typeof(RibbonRadioButtonGroupDef))]
        [XmlElement("RibbonSplitButton", typeof(RibbonSplitButtonDef))]
        internal List<RibbonItemDef> m_Items { get; set; } = new List<RibbonItemDef>();
        #endregion

        [XmlAttribute("IsSplit")]
        internal string m_IsSplitSerializable
        {
            get => IsSplit.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsSplit = true;
                    return;
                }
                IsSplit = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("IsGrouping")]
        internal string m_IsGroupingSerializable
        {
            get => IsGrouping.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsGrouping = false;
                    return;
                }
                IsGrouping = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("SynchronizeOption")]
        internal string m_SynchronizeOptionSerializable
        {
            get => SynchronizeOption.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonListButton.RibbonListButtonSynchronizeOption result))
                    result = RibbonListButton.RibbonListButtonSynchronizeOption.All;
                SynchronizeOption = result;
            }
        }

        [XmlAttribute("AllowOrientation")]
        internal string m_AllowOrientationSerializable
        {
            get => AllowOrientation.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    AllowOrientation = false;
                    return;
                }
                AllowOrientation = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }
        #endregion

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonChecklistButton
        [Description("This class is used to support the Checklist button in a ribbon. " +
            "The Checklist button displays a list of checkboxes in the drop down list. " +
            "The items in the drop down list should be of type RibbonCommandItem or RibbonSeparator. " +
            "Other items are not supported in the drop down list. An exception is thrown if an unsupported item is added to the Items collection.")]
        public class RibbonChecklistButtonDef : RibbonListButtonDef
        { }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonMenuButton
        [Description("This class is used to support the menu button in a ribbon. " +
            "The Menu button displays a standard menu in the drop-down list. " +
            "The menu can be a nested menu with sub-menus. " +
            "The items in the drop-down list should be of type RibbonMenuItem or RibbonSeparator. " +
            "Other item types are not supported in the drop-down list. " +
            "An exception is thrown if an unsupported item is added to the Items collection.")]
        public class RibbonMenuButtonDef : RibbonListButtonDef
        { }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRadioButtonGroup
        [Description("This class is used to support radio button groups in a ribbon. " +
            "This class contains a collection of RibbonToggleButton items, which act as radio buttons by supporting a mutually exclusive checkstate. " +
            "The radio buttons in the group use display properties like Size, ShowText, and ShowImage from this parent group and ignore the properties set in the radio button itself.")]
        public class RibbonRadioButtonGroupDef : RibbonListButtonDef
        {
            public RibbonRadioButtonGroupDef()
            {
                base.IsSplit = false;
                base.Width = double.NaN;
                base.AllowInToolBar = true;
            }

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(3)]
            public int MaxRow { get; set; } = 3;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(10_000)]
            public int MaxColumn { get; set; } = 10_000;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonItemSize.Standard)]
            public RibbonItemSize CollapsedSize { get; set; } = RibbonItemSize.Standard;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
            public System.Windows.Controls.Orientation ExpandOrientation { get; set; } = System.Windows.Controls.Orientation.Horizontal;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(true)]
            public bool CanCollapse { get; set; } = true;

            #region INTERNALS
            [XmlAttribute("MaxRow")]
            internal string m_MaxRowSerializable
            {
                get => MaxRow.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxRow = x;
                    else
                    {
                        MaxRow = 3;
                    }
                }
            }

            [XmlAttribute("MaxColumn")]
            internal string m_MaxColumnSerializable
            {
                get => MaxColumn.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxColumn = x;
                    else
                    {
                        MaxColumn = 10_000;
                    }
                }
            }

            [XmlAttribute("ExpandOrientation")]
            internal string m_ExpandOrientationSerializable
            {
                get => ExpandOrientation.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                        result = System.Windows.Controls.Orientation.Horizontal;
                    ExpandOrientation = result;
                }
            }

            [XmlAttribute("CollapsedSize")]
            internal string m_CollapsedSizeSerializable
            {
                get => CollapsedSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonItemSize result))
                        result = RibbonItemSize.Standard;
                    CollapsedSize = result;
                }
            }

            [XmlAttribute("CanCollapse")]
            internal string m_CanCollapseSerializable
            {
                get => CanCollapse.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        // Passing the default value
                        CanCollapse = true;
                        return;
                    }
                    CanCollapse = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
                }
            }
            #endregion
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSplitButton
        [Description("This class is used to support split buttons in a ribbon. " +
            "The items in the drop-down list should be of type RibbonCommandItem or RibbonSeparator. " +
            "Other items are not supported in the drop-down list. " +
            "An exception is thrown if an unsupported item is added to the Items collection.")]
        public class RibbonSplitButtonDef : RibbonListButtonDef
        {
            public RibbonSplitButtonDef()
            {
                base.IsSplit = true;
                base.SynchronizeOption = RibbonListButton.RibbonListButtonSynchronizeOption.Image;
            }

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonSplitButtonListStyle.List)]
            public RibbonSplitButtonListStyle ListStyle { get; set; } = RibbonSplitButtonListStyle.List;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonImageSize.Large)]
            public RibbonImageSize ListImageSize { get; set; } = RibbonImageSize.Large;

            #region INTERNALS
            [XmlAttribute("ListStyle")]
            internal string m_ListStyleSerializable
            {
                get => ListStyle.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonSplitButtonListStyle result))
                        result = RibbonSplitButtonListStyle.List;
                    ListStyle = result;
                }
            }

            [XmlAttribute("ListImageSize")]
            internal string m_ListImageSizeSerializable
            {
                get => ListImageSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonImageSize result))
                        result = RibbonImageSize.Large;
                    ListImageSize = result;
                }
            }
            #endregion
        }
    }
}