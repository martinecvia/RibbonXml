using System.ComponentModel; // Keep for .NET 4.6
using System.Xml.Serialization; // Keep for .NET 4.6

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonLabel
    public class RibbonLabelDef : RibbonItemDef
    {
        public RibbonLabelDef()
        {
            base.ResizeStyle = RibbonItemResizeStyles.NoResize;
            base.ShowText = true;
            base.IsToolTipEnabled = false;
            base.Width = double.NaN;
            base.MinWidth = 0.0;
        }

#if NET8_0_OR_GREATER
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
        [Description("This is Orientation, a member of class RibbonLabel.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonLabel_Orientation
        public System.Windows.Controls.Orientation Orientation => System.Windows.Controls.Orientation.Horizontal;
#endif
    }
}