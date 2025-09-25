using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

using RibbonXml.Items.CommandItems;

namespace RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRowPanel
    [XmlInclude(typeof(RibbonRowPanelDef))]
    [XmlInclude(typeof(RibbonFlowPanelDef))]
    [XmlInclude(typeof(RibbonFoldPanelDef))]
    [Description("The RibbonRowPanel class is used to create a sub-panel within a panel. " +
        "RibbonRowPanel is a ribbon item that can be added to a RibbonPanelSource. " +
        "Items collection to create a sub-panel containing multiple sub-rows in a main row of items. " +
        "For example, a sub-panel could be used to create a row containing two large buttons and two rows of small buttons. " +
        "The items in the sub-panel are stored and managed in the Items collection. " +
        "The items can be organized into multiple rows by adding a RibbonRowBreak item at the index where the new row is to start.")]
    public class RibbonRowPanelDef : RibbonItemDef
    {
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(100)]
        public int ResizePriority { get; set; } = 100;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(RibbonRowPanelResizeStyle.None)]
        public RibbonRowPanelResizeStyle SubPanelResizeStyle { get; set; } = RibbonRowPanelResizeStyle.None;

        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsTopJustified { get; set; } = false;

        #region INTERNALS
#if !NET8_0_OR_GREATER // This was removed in AutoCAD 
        // however keeping this does not affect usability, and its still used in .NET 4.6 and ZWCAD
        [XmlOut]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool AreItemsArrangedFromRightToLeft { get; set; } = false;

        [XmlAttribute("AreItemsArrangedFromRightToLeft")]
        public string m_AreItemsArrangedFromRightToLeftSerializable
        {
            get => AreItemsArrangedFromRightToLeft.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    AreItemsArrangedFromRightToLeft = false;
                    return;
                }
                AreItemsArrangedFromRightToLeft = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }
#endif
        #region CONTENT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlElement("RibbonSubPanelSource", typeof(RibbonSubPanelSourceDef))]
        public RibbonSubPanelSourceDef m_Source { get; set; } = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // RibbonItem
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonFoldPanelDef))]
        [XmlElement("RibbonSeparator", typeof(RibbonSeparatorDef))]
        [XmlElement("RibbonSlider", typeof(RibbonSliderDef))]
        [XmlElement("RibbonSpinner", typeof(RibbonSpinnerDef))]
        [XmlElement("RibbonTextBox", typeof(RibbonTextBoxDef))]
        // RibbonCommandItem
#if !ZWCAD
        [XmlElement("ProgressBarSource", typeof(ProgressBarSourceDef))]
#endif
        [XmlElement("RibbonCheckBox", typeof(RibbonCheckBoxDef))]
        [XmlElement("RibbonMenuItem", typeof(RibbonMenuItemDef))]
        [XmlElement("ApplicationMenuItem", typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
        // RibbonButton
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonToggleButton", typeof(RibbonToggleButtonDef))]
#if (NET8_0_OR_GREATER || ZWCAD)
        [XmlElement("ToolBarShareButton", typeof(RibbonToggleButtonDef.ToolBarShareButtonDef))]
#endif
        [XmlElement("RibbonChecklistButton", typeof(RibbonListButtonDef.RibbonChecklistButtonDef))]
        [XmlElement("RibbonMenuButton", typeof(RibbonListButtonDef.RibbonMenuButtonDef))]
        [XmlElement("RibbonRadioButtonGroup", typeof(RibbonListButtonDef.RibbonRadioButtonGroupDef))]
        [XmlElement("RibbonSplitButton", typeof(RibbonListButtonDef.RibbonSplitButtonDef))]
        public List<RibbonItemDef> m_Items { get; set; } = new List<RibbonItemDef>();
        #endregion

        [XmlAttribute("ResizePriority")]
        public string m_ResizePrioritySerializable
        {
            get => ResizePriority.ToString();
            set
            {
                if (int.TryParse(value, out var x)) ResizePriority = x;
                else
                {
                    ResizePriority = 100;
                }
            }
        }

        [XmlAttribute("SubPanelResizeStyle")]
        public string m_SubPanelResizeStyleSerializable
        {
            get => SubPanelResizeStyle.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonRowPanelResizeStyle result))
                    result = RibbonRowPanelResizeStyle.None;
                SubPanelResizeStyle = result;
            }
        }

        [XmlAttribute("IsTopJustified")]
        public string m_IsTopJustifiedSerializable
        {
            get => IsTopJustified.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Passing the default value
                    IsTopJustified = false;
                    return;
                }
                IsTopJustified = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
            }
        }
        #endregion

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel
        public class RibbonFlowPanelDef : RibbonRowPanelDef
        {
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(3)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel_MaxRowNumber
            public int MaxRowNumber { get; set; } = 3;

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel_AreColumnsStatic
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(false)]
            public bool AreColumnsStatic { get; set; } = false;

            /*
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonSupportedSubPanelStyle.None)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel_SupportedSubPanel
            public RibbonSupportedSubPanelStyle SupportedSubPanel { get; set; } = RibbonSupportedSubPanelStyle.None;

            [XmlAttribute("SupportedSubPanel")]
            public string m_SupportedSubPanelSerializable
            {
                get => SupportedSubPanel.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonSupportedSubPanelStyle result))
                        result = RibbonSupportedSubPanelStyle.None;
                    SupportedSubPanel = result;
                }
            }
            */

            #region INTERNALS
            [XmlAttribute("MaxRowNumber")]
            public string m_MaxRowNumberSerializable
            {
                get => MaxRowNumber.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxRowNumber = x;
                    else
                    {
                        MaxRowNumber = 3;
                    }
                }
            }

            [XmlAttribute("AreColumnsStatic")]
            public string m_AreColumnsStaticSerializable
            {
                get => AreColumnsStatic.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        // Passing the default value
                        AreColumnsStatic = false;
                        return;
                    }
                    AreColumnsStatic = value.Trim().Equals("TRUE", StringComparison.CurrentCultureIgnoreCase);
                }
            }
            #endregion
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel
        public class RibbonFoldPanelDef : RibbonRowPanelDef
        {
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelResizeStyle.None)]
            public new RibbonFoldPanelResizeStyle SubPanelResizeStyle { get; set; } = RibbonFoldPanelResizeStyle.None;

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_DefaultSize
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelSize.Medium)]
            public RibbonFoldPanelSize DefaultSize { get; set; } = RibbonFoldPanelSize.Medium;

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_MaxSize
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelSize.Large)]
            public RibbonFoldPanelSize MaxSize { get; set; } = RibbonFoldPanelSize.Large;

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_MinSize
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelSize.Small)]
            public RibbonFoldPanelSize MinSize { get; set; } = RibbonFoldPanelSize.Small;

            /*
            [XmlOut]
            [XmlIgnore]
            [DefaultValue(RibbonSupportedSubPanelStyle.None)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_SupportedSubPanel
            public RibbonSupportedSubPanelStyle SupportedSubPanel { get; set; } = RibbonSupportedSubPanelStyle.None;

            [XmlAttribute("SupportedSubPanel")]
            public string m_SupportedSubPanelSerializable
            {
                get => SupportedSubPanel.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonSupportedSubPanelStyle result))
                        result = RibbonSupportedSubPanelStyle.None;
                    SupportedSubPanel = result;
                }
            }
            */

            #region INTERNALS
            [XmlAttribute("SubPanelResizeStyle")]
            public new string m_SubPanelResizeStyleSerializable
            {
                get => SubPanelResizeStyle.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelResizeStyle result))
                        result = RibbonFoldPanelResizeStyle.None;
                    SubPanelResizeStyle = result;
                }
            }

            [XmlAttribute("DefaultSize")]
            public string m_DefaultSizeSerializable
            {
                get => DefaultSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelSize result))
                        result = RibbonFoldPanelSize.Medium;
                    DefaultSize = result;
                }
            }

            [XmlAttribute("MaxSize")]
            public string m_MaxSizeSerializable
            {
                get => MaxSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelSize result))
                        result = RibbonFoldPanelSize.Large;
                    MaxSize = result;
                }
            }

            [XmlAttribute("MinSize")]
            public string m_MinSizeSerializable
            {
                get => MinSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelSize result))
                        result = RibbonFoldPanelSize.Small;
                    MinSize = result;
                }
            }
            #endregion
        }
    }
}