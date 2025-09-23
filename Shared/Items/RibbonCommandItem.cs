using System; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml.Serialization;

using RibbonXml.Items.CommandItems;

namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCommandItem
#if !ZWCAD
    [XmlInclude(typeof(ProgressBarSourceDef))]
#endif
    [XmlInclude(typeof(RibbonButtonDef))]
    [XmlInclude(typeof(RibbonCheckBoxDef))]
    [XmlInclude(typeof(RibbonListButtonDef))]
    [XmlInclude(typeof(RibbonMenuItemDef))]
    [XmlInclude(typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
    [XmlInclude(typeof(RibbonToggleButtonDef))]
#if (NET8_0_OR_GREATER || ZWCAD)
    [XmlInclude(typeof(RibbonToggleButtonDef.ToolBarShareButtonDef))]
#endif
    [XmlInclude(typeof(RibbonListButtonDef.RibbonChecklistButtonDef))]
    [XmlInclude(typeof(RibbonListButtonDef.RibbonMenuButtonDef))]
    [XmlInclude(typeof(RibbonListButtonDef.RibbonRadioButtonGroupDef))]
    [XmlInclude(typeof(RibbonListButtonDef.RibbonSplitButtonDef))]
    public class RibbonCommandItemDef : RibbonItemDef
    {
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Input.ICommand CommandHandler { get; set; } = null;

        [XmlAttribute("CommandHandler")]
        public string CommandHandlerDef
        {
            get => CommandHandler != null && CommandHandler is CommandHandler handler
                ? handler.Command : string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CommandHandler = RibbonDef.GetHandler(value);
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCommandItem_IsCheckable
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates if this is a checkable item. " +
            "This property is used only by item types that support the toggling of items. " +
            "For example, items in a drop down list of a RibbonMenuItem or RibbonChecklistButton use this property. " +
            "The default value is false.")]
        public bool IsCheckable { get; set; } = false;

        [XmlAttribute("IsCheckable")]
        public string IsCheckableDef
        {
            get => IsCheckable.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsCheckable = false;
                    return;
                }
                IsCheckable = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsActive { get; set; } = false;

        [XmlAttribute("IsActive")]
        public string IsActiveDef
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
    }
}