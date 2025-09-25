using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("System.Xml.Serialization")]
namespace RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-__MEMBERTYPE_Properties_Autodesk_Windows_RibbonToggleButton
    [System.ComponentModel.Description("Toggle button: " +
        "This widget has two APIs for the toggle state: " +
        "1. bool IsChecked, used for true/false state. The IsCheckedBinding is set to private. " +
        "2. bool CheckState, used for null/true/false state, can be set to null after initialized only when IsThreeState is true. There is also a CheckStateBinding for CheckState. " +
        "IsChecked will be true only when CheckState is true. This is useful for those who want to use the check state as a bool value directly. " +
        "Please use CheckState as much as possible.")]
    [XmlInclude(typeof(RibbonToggleButtonDef))]
    [XmlInclude(typeof(ToolBarShareButtonDef))]
    public class RibbonToggleButtonDef
        : RibbonCommandItemDef
    {
        public RibbonToggleButtonDef()
        {
            base.IsCheckable = true;
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ToolBars_ToolBarShareButton
        [System.ComponentModel.Description("The Share Drawing button next to the Quick Access toolbar (QAT) is a button that allows you to share a link to a copy of the current drawing, and allows the drawing to be viewed or edited in the AutoCAD web app.")]
        public class ToolBarShareButtonDef : RibbonToggleButtonDef
        { }
    }
}