using System.ComponentModel;

namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner
    [Description("This class is used to create a spinner in a ribbon. " +
        "RibbonSpinner supports int and double data types by default. " +
        "The implementation is kept generic so that other data types can be easily supported by deriving from this class and overriding the virtual methods.")]
    public class RibbonSpinnerDef : RibbonItemDef
    { }
}