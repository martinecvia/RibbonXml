using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq; // Keep for .NET 4.6
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
using ZwSoft.Windows.ToolBars;
#else
using Autodesk.Windows;
#if NET8_0_OR_GREATER
using Autodesk.Windows.ToolBars;
#endif
#endif
#endregion

using RibbonXml.Items;
using RibbonXml.Items.CommandItems;

namespace RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem
#if !ZWCAD
    [XmlInclude(typeof(ProgressBarSourceDef))]
#endif
    [XmlInclude(typeof(RibbonButtonDef))]
    [XmlInclude(typeof(RibbonCheckBoxDef))]
    [XmlInclude(typeof(RibbonMenuItemDef))]
    [XmlInclude(typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
    [XmlInclude(typeof(RibbonToggleButtonDef))]
    [XmlInclude(typeof(RibbonListButtonDef.RibbonChecklistButtonDef))]
    [XmlInclude(typeof(RibbonListButtonDef.RibbonMenuButtonDef))]
    [XmlInclude(typeof(RibbonListButtonDef.RibbonRadioButtonGroupDef))]
    [XmlInclude(typeof(RibbonListButtonDef.RibbonSplitButtonDef))]
#if (NET8_0_OR_GREATER || ZWCAD)
    [XmlInclude(typeof(RibbonToggleButtonDef.ToolBarShareButtonDef))]
#endif
    [XmlInclude(typeof(RibbonLabelDef))]
    [XmlInclude(typeof(RibbonListDef.RibbonComboDef))]
    [XmlInclude(typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
    [XmlInclude(typeof(RibbonPanelBreakDef))]
    [XmlInclude(typeof(RibbonRowBreakDef))]
    [XmlInclude(typeof(RibbonRowPanelDef))]
    [XmlInclude(typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
    [XmlInclude(typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
    [XmlInclude(typeof(RibbonSeparatorDef))]
    [XmlInclude(typeof(RibbonSliderDef))]
    [XmlInclude(typeof(RibbonSpinnerDef))]
    [XmlInclude(typeof(RibbonTextBoxDef))]
    public class RibbonItemDef : BaseRibbonXml
    {
        private string _cookie;
        public override string Cookie
        {
            get => _cookie ?? $"%Parent:Item={Id}_{Text}_{Name}";
            set => _cookie = value;
        }

        [XmlOut]
        [XmlAttribute("Text")]
        [DefaultValue(null)]
        [Description("Gets or sets the text of a ribbon item. There are two properties that support text content in ribbon items: Text and Name. " +
            "The Text property is meant for short text and is used wherever space is premium (for example, text that appears directly in ribbon face). " +
            "The Name property is meant for longer text and is used wherever space is not an issue (for example, text that appears in a drop-down list, tooltip, etc.). " +
            "Both properties can be set for a ribbon item. " +
            "The visibility of text in the ribbon is controlled by the ShowText property, which must be true to make the text visible. " +
            "Text may also be suppressed in one or more panels in a horizontal ribbon when there is not enough space to display all the panels. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Text
        public string Text { get; set; } = null;

        [XmlElement("Text")]
        public XmlCDataSection TextCData
        {
            get
            {
                if (string.IsNullOrEmpty(Text))
                    return null;
                return new XmlDocument().CreateCDataSection(Text);
            }
            set { Text = value?.Value; }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the image to be used when the item is displayed in large size." +
            "Many ribbon items can appear in two sizes: Standard and Large. " +
            "Some ribbon items support only standard size (example: RibbonCombo) and some support both (example: RibbonButton). " +
            "Ribbon items also support ico files containing multiple images in different sizes (only the first two images will be used). " +
            "If the ico file contains multiple size images, use it to set only the Image or LargeImage property. " +
            "The visibility of the image in a ribbon is controlled by the ShowImage property, which must be set to true to make the image visible.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Image
        /// <summary>
        /// Gets or sets the image resource name (without extension) used when the ribbon item is displayed in large size.
        /// </summary>
        /// <remarks>
        /// This replaces the original <see cref="System.Windows.Media.ImageSource"/> with an internal reference to an embedded image resource.
        /// The resource should typically be a PNG or ICO embedded in the assembly, and only the base name (no extension) is specified.
        /// 
        /// For ICO files with multiple sizes, only the first two images will be used. The visibility of this image is controlled
        /// by the <c>ShowImage</c> property, which must be set to <c>true</c> for the image to appear in the ribbon UI.
        /// </remarks>
        /// <example>
        /// Example: <c>Image="icon_button_save"</c> (resolved as icon_button_save.png or .ico from resources)
        /// </example>
        public ImageSource LargeImage { get; set; } = RibbonDef.GetImageSource("{fe5f6970-c86a-4119-ad96-a01ba4b2542b}");

        [XmlAttribute("LargeImage")]
        public string LargeImageDef
        {
            get => LargeImage?.ToString() ?? "";
            set => LargeImage = RibbonDef.GetImageSource(value ?? "{fe5f6970-c86a-4119-ad96-a01ba4b2542b}");
        }

        [XmlOut]
        [XmlAttribute("KeyTip")]
        [DefaultValue(null)]
        [Description("Gets or sets the keytip for the item. " +
            "Keytips are displayed in the ribbon and are used to navigate the ribbon through the keyboard. " +
            "This property must be set for a ribbon item to support a keytip. " +
            "If this property is not set, a keytip will not appear for this item, and the item will not support activation through the keyboard. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_KeyTip
        public string KeyTip { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the image to be used when the item is displayed in standard size. " +
            "Many ribbon items can appear in two sizes: Standard and Large. " +
            "Some ribbon items support only standard size (example: RibbonCombo) and some support both (example: RibbonButton). " +
            "Ribbon items also support ico files containing multiple images in different sizes (only the first two images will be used). " +
            "If the ico file contains multiple size images, use it to set only the Image or LargeImage property. " +
            "The visibility of the image in a ribbon is controlled by the ShowImage property, which must be set to true to make the image visible.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Image
        /// <summary>
        /// Gets or sets the image resource name (without extension) used when the ribbon item is displayed in standard size.
        /// </summary>
        /// <remarks>
        /// This replaces the original <see cref="System.Windows.Media.ImageSource"/> with an internal reference to an embedded image resource.
        /// The resource should typically be a PNG or ICO embedded in the assembly, and only the base name (no extension) is specified.
        /// 
        /// For ICO files with multiple sizes, only the first two images will be used. The visibility of this image is controlled
        /// by the <c>ShowImage</c> property, which must be set to <c>true</c> for the image to appear in the ribbon UI.
        /// </remarks>
        /// <example>
        /// Example: <c>Image="icon_button_save"</c> (resolved as icon_button_save.png or .ico from resources)
        /// </example>
        public ImageSource Image { get; set; } = RibbonDef.GetImageSource("{0c615a0b-631a-45ac-9e41-d37ec56c5f8c}");

        [XmlAttribute("Image")]
        public string ImageDef
        {
            get => Image?.ToString() ?? "";
            set => Image = RibbonDef.GetImageSource(value ?? "{0c615a0b-631a-45ac-9e41-d37ec56c5f8c}");
        }

        [XmlOut]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        [Description("Gets or sets custom data object in the tab. " +
            "This property can be used to store any object as a custom data object in a tab. " +
            "This data is not used by the framework. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Tag
        public string Tag { get; set; } = null;

        [XmlOut]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of a ribbon item. " +
            "There are two properties that support text content in ribbon items: Text and Name. " +
            "The Text property is meant for short text and is used wherever space is premium (for example, text that appears directly in ribbon face). " +
            "The Name property is meant for longer text and is used wherever space is not an issue (for example, text that appears in a drop-down list, tooltip, etc.). " +
            "Both properties can be set for a ribbon item. " +
            "The visibility of text in the ribbon is controlled by the ShowText property, which must be true to make the text visible. " +
            "Text may also be suppressed in one or more panels in a horizontal ribbon when there is not enough space to display all the panels. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Name
        public string Name { get; set; } = null;

        [XmlOut]
        [XmlAttribute("GroupName")]
        [DefaultValue(null)]
        [Description("Gets or sets the description text for the ribbon item. " +
            "The description text is used in the application menu, tooltips, and drop-down lists in a RibbonListButton when the list style is set to Descriptive.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_GroupName
        public string GroupName { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value indicating whether F1 help is enabled in the item's tooltip. " +
            "If this value is true, F1 help is enabled in the item's tooltip; otherwise, F1 help is disabled in the item' s tooltip. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_IsToolTipEnabled
        public bool IsToolTipEnabled { get; set; } = true;

        [XmlAttribute("IsToolTipEnabled")]
        public string IsToolTipEnabledDef
        {
            get => IsToolTipEnabled.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsToolTipEnabled = true;
                    return;
                }
                IsToolTipEnabled
                    = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlAttribute("ToolTip")]
        [DefaultValue(null)]
        [Description("Gets or sets an object to be used as the tooltip for the item. " +
            "If the object is a string, it displays the string as a tooltip without any formatting. " +
            "If the object is of any other type, and the object has a data template, the tooltip will use the data template to display the object. " +
            "If this property is null, a default tooltip of type RibbonToolTip is created at runtime using the Name, Text, and Description properties of the item. " +
            "If this property is set to an empty string, the tooltip will be suppressed, and the item will not show any tooltip. " +
            "ToolTip can also be suppressed by setting the IsToolTipEnabled property to false. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ToolTip
        public string ToolTip { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value indicating whether the item image is visible. " +
            "If the value is true, the image is visible in the ribbon, provided that a valid image has been set with the Image property. " +
            "If the value is false, the image is not visible. " +
            "The default value is true. " +
            "Derived classes may override this default.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ShowImage
        public bool ShowImage { get; set; } = true;

        [XmlAttribute("ShowImage")]
        public string ShowImageDef
        {
            get => ShowImage.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    ShowImage = true;
                    return;
                }
                ShowImage = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(100d)]
        [Description("Gets or sets the minimum width of the item. " +
            "This property is used only by resizeable items. " +
            "Resizeable items are items whose width is resized to fit in the space available. " +
            "For example, RibbonCombo and RibbonGallery are resizeable items, and RibbonButton is not a resizeable item. " +
            "If there is enough space, resizeable items are displayed in full width, which is set with the Width property. " +
            "When there is not enough space, the item will be resized to fit in the available space. " +
            "The width of the item will not go below MinWidth. " +
            "The value for this property must be zero or positive and cannot be Infinity. " +
            "The value must be in device-independent units. " +
            "The default value is 100.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_MinWidth
        public double MinWidth { get; set; } = 100d;

        [XmlAttribute("MinWidth")]
        public string MinWidthDef
        {
            get => MinWidth.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    MinWidth = result;
                    return;
                }
                MinWidth = 100d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(200d)]
        [Description("Gets or sets the width of the item. " +
            "This property is used only by resizeable items. " +
            "Resizeable items are items whose width is resized to fit in the space available. " +
            "For example, RibbonCombo and RibbonGallery are resizeable items, and RibbonButton is not a resizeable item. " +
            "If there is enough space, resizeable items are displayed in full width. " +
            "When there is not enough space, the item will be resized to fit in the available space. " +
            "The width of the item will not go below MinWidth. " +
            "The value for this property must be zero or positive and cannot be Infinity. " +
            "The value must be in device-independent units. " +
            "The default value is 200.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Width
        public double Width { get; set; } = 200d;

        [XmlAttribute("Width")]
        public string WidthDef
        {
            get => Width.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Width = result;
                    return;
                }
                Width = 200d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(RibbonItemResizeStyles.ResizeWidth)]
        public RibbonItemResizeStyles ResizeStyle { get; set; } = RibbonItemResizeStyles.ResizeWidth;

        [XmlAttribute("ResizeStyle")]
        public string ResizeStyleDef
        {
            get => ResizeStyle.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonItemResizeStyles result))
                    result = RibbonItemResizeStyles.ResizeWidth;
                ResizeStyle = result;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(RibbonItemSize.Standard)]
        [Description("Gets or sets the size with which the item is displayed. " +
            "This property is supported only by RibbonButton and RibbonLabel as well as classes derived from them. " +
            "Other ribbon items will ignore this property. " +
            "The default value is Standard.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Size
        public RibbonItemSize Size { get; set; } = RibbonItemSize.Standard;

        [XmlAttribute("Size")]
        [XmlOut]
        public string SizeDef
        {
            get => Size.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonItemSize result))
                    result = RibbonItemSize.Standard;
                Size = result;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value indicating whether the item text is visible. " +
            "If the value is true, the text for the item is visible in the ribbon, provided that a valid text has been set for this item with the Text property. " +
            "If the value is false, the text is not visible in the ribbon. " +
            "The default value is false. " +
            "Derived classes may override this default.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ShowText
        public bool ShowText { get; set; } = false;

        [XmlAttribute("ShowText")]
        public string ShowTextDef
        {
            get => ShowText.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    ShowText = false;
                    return;
                }
                ShowText = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value to indicating whether the item is visible in the ribbon. " +
            "If this property is true, the item is visible in the ribbon; otherwise, the item is hidden in ribbon and does not occupy any space. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_IsVisible
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
        [Description("Gets or sets the value indicating whether the item is enabled in the ribbon." +
            "If this property is true, the item is enabled in the ribbon; otherwise, it is disabled. " +
            "Disabled items do not respond any interaction. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_IsEnabled
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
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets the description text for the ribbon item. " +
            "The description text is used in the application menu, tooltips, and drop-down lists in a RibbonListButton when the list style is set to Descriptive.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Description
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
            set { Description = value?.Value ?? string.Empty; }
        }

        [XmlOut]
        [XmlAttribute("HelpTopic")]
        [DefaultValue(null)]
        public string HelpTopic { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool ShowToolTipOnDisabled { get; set; } = true;

        [XmlAttribute("ShowToolTipOnDisabled")]
        public string ShowToolTipOnDisabledDef
        {
            get => ShowToolTipOnDisabled.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    ShowToolTipOnDisabled = true;
                    return;
                }
                ShowToolTipOnDisabled = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool AllowInToolBar { get; set; } = true;

        [XmlAttribute("AllowInToolBar")]
        public string AllowInToolBarDef
        {
            get => AllowInToolBar.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    AllowInToolBar = true;
                    return;
                }
                AllowInToolBar = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlIgnore]
        public static readonly Dictionary<Type, Func<RibbonItem>> ItemsFactory = new Dictionary<Type, Func<RibbonItem>>()
        {
            // RibbonItem
            { typeof(RibbonListDef.RibbonComboDef), () => new RibbonCombo() },
            { typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef), () => new RibbonGallery() },
            { typeof(RibbonLabelDef), () => new RibbonLabel() },
            { typeof(RibbonPanelBreakDef), () => new RibbonPanelBreak() },
            { typeof(RibbonRowBreakDef), () => new RibbonRowBreak() },
            { typeof(RibbonRowPanelDef), () => new RibbonRowPanel() },
            { typeof(RibbonRowPanelDef.RibbonFlowPanelDef), () => new RibbonFlowPanel() },
            { typeof(RibbonRowPanelDef.RibbonFoldPanelDef), () => new RibbonFoldPanel() },
            { typeof(RibbonSeparatorDef), () => new RibbonSeparator() },
            { typeof(RibbonSliderDef), () => new RibbonSlider() },
            { typeof(RibbonSpinnerDef), () => new RibbonSpinner() },
            { typeof(RibbonTextBoxDef), () => new RibbonTextBox() },
            // RibbonCommandItem
#if !ZWCAD
            { typeof(ProgressBarSourceDef), () => new ProgressBarSource() },
#endif
            { typeof(RibbonCheckBoxDef), () => new RibbonCheckBox() },
            { typeof(RibbonListButtonDef.RibbonChecklistButtonDef), () => new RibbonChecklistButton() },
            { typeof(RibbonListButtonDef.RibbonMenuButtonDef), () => new RibbonMenuButton() },
            { typeof(RibbonListButtonDef.RibbonRadioButtonGroupDef), () => new RibbonRadioButtonGroup() },
            { typeof(RibbonListButtonDef.RibbonSplitButtonDef), () => new RibbonSplitButton() },
            { typeof(RibbonMenuItemDef), () => new RibbonMenuItem() },
            { typeof(RibbonMenuItemDef.ApplicationMenuItemDef), () => new ApplicationMenuItem() },
            // RibbonButton
            { typeof(RibbonButtonDef), () => new RibbonButton() },
            { typeof(RibbonToggleButtonDef), () => new RibbonToggleButton() },
#if (NET8_0_OR_GREATER || ZWCAD)
            { typeof(RibbonToggleButtonDef.ToolBarShareButtonDef), () => new ToolBarShareButton() },
#endif
        };
    }
}