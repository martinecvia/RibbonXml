namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelBreak
    [System.ComponentModel.Description("This class is used to organize a ribbon panel into main and slide-out panels. " +
        "Add an object of this type to RibbonPanelSource.Items to move all the items after this item into the slide-out panel. " +
        "There can be only one object of this type in RibbonPanelSource. " +
        "If there are multiple RibbonPanelBreak objects in the same panel source, only the first one will be used.")]
    public class RibbonPanelBreakDef : RibbonItemDef
    { }
}