using System; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonButton
    public class RibbonButtonDef : RibbonCommandItemDef
    {
        public RibbonButtonDef()
        {
            base.MinWidth = 0;
            base.Width = double.NaN;
            base.ResizeStyle = RibbonItemResizeStyles.HideText;
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
        [Description("Accesses the orientation of text and image. " +
            "This is a dependency property. " +
            "The default value is Horizontal.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonButton_Orientation
        public System.Windows.Controls.Orientation Orientation { get; set; } = System.Windows.Controls.Orientation.Horizontal;
        
        #region INTERNALS
        [XmlAttribute("Orientation")]
        public string m_OrientationSerializable
        {
            get => Orientation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                    result = System.Windows.Controls.Orientation.Horizontal;
                Orientation = result;
            }
        }
        #endregion
    }
}