using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

using RibbonXml.Items;
using RibbonXml.Items.CommandItems;

namespace RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource
    public class RibbonSubPanelSourceDef : BaseRibbonXml
    {
        private string _cookie;
        public override string Cookie
        {
            get => _cookie ?? $"%Parent:SubPanelSource={Id}__{Name}";
            set => _cookie = value;
        }

        [XmlOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Description
        public string Description { get; set; } = null;

        [XmlOut]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Name
        public string Name { get; set; } = null;

        [XmlOut]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Tag
        public string Tag { get; set; } = null;

        #region INTERNALS
        #region CONTENT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // RibbonItem
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
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

        [XmlElement("Description")]
        public XmlCDataSection m_DescriptionCData
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return null;
                return new XmlDocument().CreateCDataSection(Description);
            }
            set { Description = value?.Value ?? string.Empty; }
        }

        [XmlElement("Name")]
        public XmlCDataSection m_NameCData
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;
                return new XmlDocument().CreateCDataSection(Name);
            }
            set { Name = value?.Value ?? Id; }
        }

        [XmlElement("Tag")]
        public XmlCDataSection m_TagCData
        {
            get
            {
                if (string.IsNullOrEmpty(Tag))
                    return null;
                return new XmlDocument().CreateCDataSection(Tag);
            }
            set { Tag = value?.Value; }
        }
        #endregion
    }
}