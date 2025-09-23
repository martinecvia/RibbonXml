using System; // Keep for .NET 4.6
using System.ComponentModel;
using System.Linq; // Keep for .NET 4.6
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider
    public class RibbonSliderDef : RibbonItemDef
    {
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(0.0d)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Minimum
        public double Minimum { get; set; } = 0.0d;

        [XmlAttribute("Minimum")]
        public string MinimumDef
        {
            get => Minimum.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Minimum = result;
                    return;
                }
                Minimum = 0.0d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(0.0d)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Maximum
        public double Maximum { get; set; } = 0.0d;

        [XmlAttribute("Maximum")]
        public string MaximumDef
        {
            get => Maximum.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Maximum = result;
                    return;
                }
                Maximum = 0.0d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(Visibility.Collapsed)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TextBox1Visibility
        public Visibility TextBox1Visibility { get; set; } = Visibility.Collapsed;

        [XmlAttribute("TextBox1Visibility")]
        public string TextBox1VisibilityDef
        {
            get => TextBox1Visibility.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out Visibility result))
                    result = Visibility.Collapsed;
                TextBox1Visibility = result;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_TextBox1Editable
        public bool TextBox1Editable { get; set; } = false;

        [XmlAttribute("TextBox1Editable")]
        public string TextBox1EditableDef
        {
            get => TextBox1Editable.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    TextBox1Editable = false;
                    return;
                }
                TextBox1Editable = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_IsSnapToTickEnabled
        public bool IsSnapToTickEnabled { get; set; } = true; // Default value was not mentioned in official documentation,
                                                              // however in version 2017 there was true as default value

        [XmlAttribute("IsSnapToTickEnabled")]
        public string IsSnapToTickEnabledDef
        {
            get => IsSnapToTickEnabled.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsSnapToTickEnabled = true;
                    return;
                }
                IsSnapToTickEnabled = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(27.0d)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TextBox1Width
        public double TextBox1Width { get; set; } = 27.0d;

        [XmlAttribute("TextBox1Width")]
        public string TextBox1WidthDef
        {
            get => TextBox1Width.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    TextBox1Width = result;
                    return;
                }
                TextBox1Width = 27.0d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(TickPlacement.None)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TickPlacement
        public TickPlacement TickPlacement { get; set; } = TickPlacement.None;

        [XmlAttribute("TickPlacement")]
        public string TickPlacementDef
        {
            get => TickPlacement.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out TickPlacement result))
                    result = TickPlacement.None;
                TickPlacement = result;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(0.0)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Value
        public double Value { get; set; } = 0.0d;

        [XmlAttribute("Value")]
        public string ValueDef
        {
            get => Value.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Value = result;
                    return;
                }
                Value = 0.0d;
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Ticks
        public DoubleCollection Ticks { get; set; } = null;

        [XmlAttribute("Ticks")]
        public string TicksDef
        {
            get => string.Join(" ", Ticks?.Select(val => val.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                try
                {
                    char[] separators = new[] { ' ', ',', ';' };
                    Ticks = new DoubleCollection(
                        value.Split(separators, System.StringSplitOptions.RemoveEmptyEntries)
                        .Select(val => double.Parse(val, System.Globalization.CultureInfo.InvariantCulture)));
                }
                catch
                {
                    Ticks = null;
                }
            }
        }

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(null)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TextBox1Text
        public string TextBox1Text { get; set; } = null;

        [XmlElement("TextBox1Text")]
        public XmlCDataSection TextBox1TextCData
        {
            get
            {
                if (string.IsNullOrEmpty(TextBox1Text))
                    return null;
                return new XmlDocument().CreateCDataSection(TextBox1Text);
            }
            set { TextBox1Text = value?.Value; }
        }
    }
}