using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

using RibbonXml.Items.CommandItems;

namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList
    [XmlInclude(typeof(RibbonComboDef))]
    [XmlInclude(typeof(RibbonComboDef.RibbonGalleryDef))]
    public abstract class RibbonListDef : RibbonItemDef
    {
        public RibbonListDef()
        {
            base.ShowImage = false;
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the drop-down list should support the grouping of items. " +
            "Only RibbonCombo supports grouping. " +
            "RibbonGallery does not support grouping. " +
            "Grouping is done using the property RibbonItem.GroupName in the drop-down items. " +
            "If this property is true, grouping is enabled in the drop-down list. " +
            "If it is false, grouping is not enabled. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_IsVirtualizing
        public bool IsGrouping { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(double.NaN)]
        [Description("Gets or sets the maximum height of the drop-down window that is displayed when a drop-down item is opened. " +
            "The height must be in device independent units. " +
            "The actual drop-down height depends on the number of items in the list and will not exceed the value set in this property. " +
            "The default value is a calculated value that is based on system max screen height parameters.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList_MaxDropDownHeight
        public double MaxDropDownHeight { get; set; } = double.NaN;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(double.NaN)]
        [Description("Gets or sets the width of the drop-down window that is displayed when a drop-down item is opened. " +
            "The width must be in device independent units. " +
            "The default value is NaN. " +
            "The minimum drop-down width is equal to the control width. " +
            "Thus, if the value set in this property is less than the control width, the value is ignored.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList_DropDownWidth
        public double DropDownWidth { get; set; } = double.NaN;

        #region INTERNALS
        #region CONTENT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // RibbonItem
        [XmlElement("RibbonCombo", typeof(RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonComboDef.RibbonGalleryDef))]
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
        public List<RibbonItemDef> m_Items { get; set; } = new List<RibbonItemDef>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        public List<RibbonCommandItemDef> m_MenuItems { get; set; } = new List<RibbonCommandItemDef>();
        #endregion

        [XmlAttribute("IsGrouping")]
        public string m_IsGroupingSerializable
        {
            get => IsGrouping.ToString();
            set
            {
                // RibbonGallery does not support grouping
                if (string.IsNullOrEmpty(value) || this is RibbonComboDef.RibbonGalleryDef)
                {
                    // Passing the default value
                    IsGrouping = false;
                    return;
                }
                IsGrouping = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlAttribute("DropDownWidth")]
        public string m_DropDownWidthSerializable
        {
            get => DropDownWidth.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    DropDownWidth = result;
                    return;
                }
                DropDownWidth = double.NaN;
            }
        }

        [XmlAttribute("MaxDropDownHeight")]
        public string m_MaxDropDownHeightSerializable
        {
            get => MaxDropDownHeight.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    MaxDropDownHeight = result;
                    return;
                }
                MaxDropDownHeight = double.NaN;
            }
        }
        #endregion

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo
        public class RibbonComboDef : RibbonListDef
        {
            [XmlOut]
            [XmlAttribute("Current")]
            [DefaultValue("")]
            public string Current { get; set; } = string.Empty;

            [XmlOut]
            [XmlAttribute("TextPath")]
            [DefaultValue("Text")]
            public string TextPath { get; set; } = "Text";

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(false)]
            [Description("Gets or sets the value that indicates whether the combo box text is editable. " +
                "If this property is true, the combo box allows text to be entered that is not in the list. " +
                "The entered text is not added to the list. " +
                "The entered text can be validated in the EditableTextChanging event. " +
                "The default value is false.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_IsEditable
            public bool IsEditable { get; set; } = false;

            [XmlOut]
            [XmlAttribute("EditableText")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_EditableText
            [Description("Gets or sets the editable text in the combo box. This property is applicable only if IsEditable is true. " +
                "The default value is null.")]
            public string EditableText { get; set; } = null;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(true)]
            public bool IsTextSearchEnabled { get; set; } = true;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(null)]
            [Description("Gets or sets the command handler to be called when the RibbonCombo menu items are executed. " +
                "The command is routed to the first command handler found while searching in the following order: " +
                "1. the command handler set in the item. " +
                "2. the command handler set in the RibbonCombo. " +
                "3. the command handler set in the root control that contains this item (ribbon, Quick Access Toolbar, menu, or status bar). " +
                "4. the global command handler set in ComponentManager.CommandHandler. " +
                "The default value is null.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_CommandHandler
            public System.Windows.Input.ICommand CommandHandler { get; set; } = null;

            [XmlOut]
            [XmlIgnore]
            [DefaultValue(double.NaN)]
            public double ResizableBoxWidth { get; set; } = double.NaN;

#if NET8_0_OR_GREATER
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(true)]
            [Description("Gets or sets a value that indicates if virtualization is enabled for the combo box." +
                "If the value of this property is true the combo box enables virtualization. " +
                "If it is false it disables virtualization. " +
                "The combo box performs better and uses less memory when virtualization is enabled. " +
                "When virtualization is enabled all the items in the combo box will be displayed in a uniform size. " +
                "Also hiding items in the combo box is not supported when virtualization is enabled. " +
                "Disable virtualization if the combo box needs to display items in various sizes. " +
                "The default value is true.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_IsVirtualizing
            public bool IsVirtualizing { get; set; } = true;

            #region INTERNALS
            [XmlAttribute("IsVirtualizing")]
            public string m_IsVirtualizingSerializable
            {
                get => IsVirtualizing.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        // Passing the default value
                        IsVirtualizing = true;
                        return;
                    }
                    IsVirtualizing = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
                }
            }
            #endregion
#endif

            #region INTERNALS
            [XmlAttribute("IsEditable")]
            public string m_IsEditableSerializable
            {
                get => IsEditable.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        // Passing the default value
                        IsEditable = false;
                        return;
                    }
                    IsEditable = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
                }
            }

            [XmlElement("EditableText")]
            public XmlCDataSection m_EditableTextCData
            {
                get
                {
                    if (string.IsNullOrEmpty(EditableText))
                        return null;
                    return new XmlDocument().CreateCDataSection(EditableText);
                }
                set { EditableText = value?.Value; }
            }

            [XmlAttribute("IsTextSearchEnabled")]
            public string m_IsTextSearchEnabledSerializable
            {
                get => IsTextSearchEnabled.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        // Passing the default value
                        IsTextSearchEnabled = true;
                        return;
                    }
                    IsTextSearchEnabled = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
                }
            }

            [XmlAttribute("CommandHandler")]
            public string m_CommandHandlerSerializable
            {
                get => CommandHandler != null && CommandHandler is CommandHandler handler
                    ? handler.Command : string.Empty;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        CommandHandler = RibbonXml.GetHandler(value);
                }
            }

            [XmlAttribute("ResizableBoxWidth")]
            public string m_ResizableBoxWidthSerializable
            {
                get => ResizableBoxWidth.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value)) return;
                    if (double.TryParse(value, out var result))
                    {
                        ResizableBoxWidth = result;
                        return;
                    }
                    ResizableBoxWidth = double.NaN;
                }
            }
            #endregion

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonGallery
            public class RibbonGalleryDef : RibbonComboDef
            {
                public RibbonGalleryDef()
                {
                    base.ResizeStyle = RibbonItemResizeStyles.ResizeWidth | RibbonItemResizeStyles.Collapse;
                }

                [XmlOut]
                [XmlIgnore]
                [DefaultValue(GalleryDisplayMode.Window)]
                [Description("Gets or sets the display mode of the gallery. " +
                    "The display mode is used to specify the appearance of the gallery in the ribbon. " +
                    "The default value is Window.")]
                // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonGallery_DisplayMode
                public GalleryDisplayMode DisplayMode { get; set; } = GalleryDisplayMode.Window;

                [XmlOut]
                [XmlIgnore]
                [DefaultValue(double.NaN)]
                [TypeConverter(typeof(LengthConverter))]
                public double ItemWidth { get; set; } = double.NaN;

                [XmlOut]
                [XmlIgnore]
                [DefaultValue(double.NaN)]
                [TypeConverter(typeof(LengthConverter))]
                public double ItemHeight { get; set; } = double.NaN;

                #region INTERNALS
                [XmlAttribute("DisplayMode")]
                public string m_DisplayModeSerializable
                {
                    get => DisplayMode.ToString();
                    set
                    {
                        if (!Enum.TryParse(value, true, out GalleryDisplayMode result))
                            result = GalleryDisplayMode.Window;
                        DisplayMode = result;
                    }
                }

                [XmlAttribute("ItemWidth")]
                public string m_ItemWidthSerializable
                {
                    get => ItemWidth.ToString();
                    set
                    {
                        if (string.IsNullOrEmpty(value)) return;
                        if (double.TryParse(value, out var result))
                        {
                            ItemWidth = result;
                            return;
                        }
                        ItemWidth = double.NaN;
                    }
                }

                [XmlAttribute("ItemHeight")]
                public string m_ItemHeightSerializable
                {
                    get => ItemHeight.ToString();
                    set
                    {
                        if (string.IsNullOrEmpty(value)) return;
                        if (double.TryParse(value, out var result))
                        {
                            ItemHeight = result;
                            return;
                        }
                        ItemHeight = double.NaN;
                    }
                }
                #endregion
            }
        }

        [XmlIgnore]
        internal static readonly Dictionary<Type, Func<RibbonCombo>> ListFactory = new Dictionary<Type, Func<RibbonCombo>>()
        {
            { typeof(RibbonComboDef), () => new RibbonCombo() },
            { typeof(RibbonComboDef.RibbonGalleryDef), () => new RibbonGallery() },
        };
    }
}