using System; // Keep for .NET 4.6
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
namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSeparator
    [Description("This class is used to support separators in a ribbon. " +
        "Separators can be used to add space or divider lines between ribbon items.")]
    public class RibbonSeparatorDef : RibbonItemDef
    {
        public RibbonSeparatorDef()
        {
            base.ResizeStyle = RibbonItemResizeStyles.NoResize;
            base.MinWidth = 0.0;
            base.Width = double.NaN;
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(RibbonSeparatorStyle.Line)]
        [Description("Gets or sets the value specifying the separator style. " +
            "Separator styles are used to set the appearance of the separator in a ribbon.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSeparator_SeparatorStyle
        public RibbonSeparatorStyle SeparatorStyle { get; set; } = RibbonSeparatorStyle.Line;

        #region INTERNALS
        [XmlAttribute("SeparatorStyle")]
        internal string m_SeparatorStyleSerializable
        {
            get => SeparatorStyle.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonSeparatorStyle result))
                    result = RibbonSeparatorStyle.Line;
                SeparatorStyle = result;
            }
        }
        #endregion
    }
}