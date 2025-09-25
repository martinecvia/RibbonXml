using System; // Keep for .NET 4.6
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("System.Xml.Serialization")]
namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner
    [Description("This class is used to create a spinner in a ribbon. " +
        "RibbonSpinner supports int and double data types by default. " +
        "The implementation is kept generic so that other data types can be easily supported by deriving from this class and overriding the virtual methods.")]
    public class RibbonSpinnerDef : RibbonItemDef
    {
        public RibbonSpinnerDef()
        {
            base.ShowImage = false;
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the current value. " +
            "The data types int and double are supported by default. " +
            "To support other data types, derive from this class and override the virtual methods. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner_Value
        public object Value { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the value specifying the amount of change that occurs when the up or down spin button is pressed. " +
            "The data type of the value assigned to this property must be same as the data type of the Value property. " +
            "The data types int and double are supported by default. " +
            "To support other data types, derive from this class and override the virtual methods. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner_Change
        public object Change { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the minimum value of the spin range. " +
            "The data type of the value assigned to this property must be same as the data type of the Value property. " +
            "The data types int and double are supported by default. " +
            "To support other data types, derive from this class and override the virtual methods. " +
            "The default value is null." +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner_Minimum
        public object Minimum { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the maximum value of the spin range. " +
            "The data type of the value assigned to this property must be same as the data type of the Value property. " +
            "The data types int and double are supported by default. " +
            "To support other data types, derive from this class and override the virtual methods. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner_Maximum
        public object Maximum { get; set; } = null;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the value is directly editable. " +
            "If the value is true, in addition to changing the value with spin buttons, the value can be entered directly in the edit control of the spinner. " +
            "If it is false, the spinner value can only be changed using the spin buttons. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner_IsEditable
        public bool IsEditable { get; set; } = false;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(double.NaN)]
        public double ResizableBoxWidth { get; set; } = double.NaN;

        #region INTERNALS
        [XmlAttribute("Value")]
        internal string m_ValueSerializable
        {
            get => Value?.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var x)) Value = x;
                else if (int.TryParse(value, out var y)) Value = y;
                else
                {
                    // No valid value is set. Supported values are: [double, int]
                    Value = null;
                }
            }
        }

        [XmlAttribute("Change")]
        internal string m_ChangeSerializable
        {
            get => Change?.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (Value != null)
                {
                    if (Value is double && double.TryParse(value, out var a)) Change = a;
                    else if (int.TryParse(value, out var b)) Change = b;
                    else
                    {
                        // No valid value is set. Supported values are: [double, int]
                        Change = null;
                    }
                    return;
                }
                if (double.TryParse(value, out var c)) Change = c;
                else if (int.TryParse(value, out var d)) Change = d;
                else
                {
                    // No valid value is set. Supported values are: [double, int]
                    Change = null;
                }
            }
        }

        [XmlAttribute("Maximum")]
        internal string m_MaximumSerializable
        {
            get => Maximum?.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (Maximum != null)
                {
                    if (Maximum is double && double.TryParse(value, out var a)) Maximum = a;
                    else if (int.TryParse(value, out var b)) Maximum = b;
                    else
                    {
                        // No valid value is set. Supported values are: [double, int]
                        Maximum = null;
                    }
                    return;
                }
                if (double.TryParse(value, out var c)) Maximum = c;
                else if (int.TryParse(value, out var d)) Maximum = d;
                else
                {
                    // No valid value is set. Supported values are: [double, int]
                    Maximum = null;
                }
            }
        }

        [XmlAttribute("Minimum")]
        internal string m_MinimumSerializable
        {
            get => Minimum?.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (Minimum != null)
                {
                    if (Minimum is double && double.TryParse(value, out var a)) Minimum = a;
                    else if (int.TryParse(value, out var b)) Minimum = b;
                    else
                    {
                        // No valid value is set. Supported values are: [double, int]
                        Minimum = null;
                    }
                    return;
                }
                if (double.TryParse(value, out var c)) Minimum = c;
                else if (int.TryParse(value, out var d)) Minimum = d;
                else
                {
                    // No valid value is set. Supported values are: [double, int]
                    Minimum = null;
                }
            }
        }

        [XmlAttribute("IsEditable")]
        internal string m_IsEditableSerializable
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

        [XmlAttribute("ResizableBoxWidth")]
        internal string m_ResizableBoxWidthSerializable
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
    }
}