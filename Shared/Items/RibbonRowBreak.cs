using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("System.Xml.Serialization")]
namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRowBreak
    [System.ComponentModel.Description("This class is used to organize the ribbon items in a panel into multiple rows. " +
        "Add an object of this class to any position in a RibbonPanelSource.Items or RibbonRowPanel.Items collection to move subsequent items to the next row.")]
    public class RibbonRowBreakDef : RibbonItemDef
    { }
}