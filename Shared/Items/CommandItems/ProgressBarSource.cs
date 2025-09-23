using System; // Keep for AutoCAD & .NET 4.6
using System.ComponentModel;
using System.Xml; // Keep for AutoCAD
using System.Xml.Serialization; // Keep for AutoCAD

namespace RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ProgressBarSource
    [Description("The data source of the Progress Bar widget control. " +
        "Architecturally a progress bar widget is divided into data elements, which provide the content of the progress bar, and visual elements, which provide the display and interaction between the data elements and user. " +
        "The data elements consist of the following data items: " +
        "- the current value of the progress bar " +
        "- the current operation string " +
        "The visual elements of the progress bar widget mainly consist of the following items: " +
        "- a Cancel button " +
        "- a WPF progress bar control " +
        "- a TextBlock that displays the current value by percentage " +
        "- a TextBlock that displays the current operation " +
        "The ProgressBarSource class derives from RibbonCommandItem and is responsible for maintaining the following: " +
        "1. the visibility state of the Cancel button " +
        "2. the current operation string " +
        "3. the value that represents the current magnitude " +
        "4. the minimum value of the progress bar range " +
        "5. the maximum value of the progress bar range." +
        "The data elements are linked to and drive the visual elements by the concept of data binding.")]
    public class ProgressBarSourceDef : RibbonCommandItemDef
    {
#if !ZWCAD
        public ProgressBarSourceDef()
        {
            if (MaximumValue < MinimumValue)
                throw new ArgumentException($"MaximumValue can't be smaller then MinimumValue [max:{MaximumValue} < min:{MinimumValue}]");
            CurrentValue = MinimumValue;
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool HasCancelButton { get; set; } = false;

        [XmlAttribute("HasCancelButton")]
        public string HasCancelButtonDef
        {
            get => HasCancelButton.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    HasCancelButton = false;
                    return;
                }
                HasCancelButton = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlAttribute("CurrentOperation")]
        [DefaultValue("")]
        public string CurrentOperation { get; set; } = string.Empty;

        [XmlElement("CurrentOperation")]
        public XmlCDataSection CurrentOperationCData
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentOperation))
                    return null;
                return new XmlDocument().CreateCDataSection(CurrentOperation);
            }
            set { CurrentOperation = value?.Value ?? string.Empty; }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(100d)]
        public double MaximumValue { get; set; } = 100d;

        [XmlAttribute("MaximumValue")]
        public string MaximumValueDef
        {
            get => MaximumValue.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    if (result < MinimumValue)
                    {
                        MaximumValue = 100d;
                        return;
                    }
                    MaximumValue = result;
                    return;
                }
                MaximumValue = 100d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(0.0d)]
        public double MinimumValue { get; set; } = 0.0d;

        [XmlAttribute("MinimumValue")]
        public string MinimumValueDef
        {
            get => MinimumValue.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    if (result > MaximumValue)
                    {
                        MinimumValue = 0.0d;
                        return;
                    }
                    MinimumValue = result;
                    return;
                }
                MinimumValue = 0.0d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(0.0d)]
        public double CurrentValue { get; set; } = 0.0d;

        [XmlAttribute("CurrentValue")]
        public string CurrentValueDef
        {
            get => CurrentValue.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    if (result < MinimumValue || result > MaximumValue)
                    {
                        // Value must be in range of MinimumValue and MaximumValue
                        CurrentValue = MinimumValue;
                        return;
                    }
                    CurrentValue = result;
                    return;
                }
                CurrentValue = MinimumValue;
            }
        }
#endif
    }
}