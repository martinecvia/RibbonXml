# RibbonXml

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/github/actions/workflow/status/martinecvia/RibbonXml/github-ci.yml)](https://github.com/martinecvia/RibbonXml/actions)
[![CodeFactor](https://www.codefactor.io/repository/github/martinecvia/ribbonxml/badge)](https://www.codefactor.io/repository/github/martinecvia/ribbonxml)
[![Last Commit](https://img.shields.io/github/last-commit/martinecvia/RibbonXml)](https://github.com/martinecvia/RibbonXml/commits/main)
[![AutoCAD Versions](https://img.shields.io/badge/AutoCAD-2017→2026-orange.svg)](https://www.autodesk.com)
[![ZWCAD Support](https://img.shields.io/badge/ZWCAD-Supported-green.svg)](https://www.zwsoft.com)
[![BricsCAD Support](https://img.shields.io/badge/BricsCAD-Unsupported-red.svg)](https://www.bricsys.com)

A comprehensive collection of Ribbon XML utilities for AutoCAD (2017–2026) and ZWCAD, designed to simplify the process of building custom tabs, panels, and commands.

Supported Platforms
- AutoCAD: 2017 through 2026
- ZWCAD: Latest builds
- Framework Support:
  - .NET Framework 4.6
  - .NET Framework 4.7 (ZWCAD)
  - .NET 8.0 (AutoCAD)
---
## Key Features

- Define ribbon tabs, panels, and controls using straightforward XML files
- Automatically load ribbon definitions from embedded resources
- Support for contextual ribbon tabs that appear based on selection conditions
- Custom command handlers for enhanced functionality
- Flexible image loading from files, URIs, or embedded resources
- Cross-platform compatibility with both AutoCAD and ZWCAD
---

# Getting Started
Clone the repository to begin:
```bash
git clone https://github.com/martinecvia/RibbonXml.git
```
```csharp
using RibbonXml;

var ribbon = new RibbonXml.Builder()
    .SetDefaultHandler(typeof(MyCommandHandler))   // Optional default handler
    .RegisterCommandHandler(new MyCommandHandler("MYCMD"))
    .RegisterImage("MyIcon", "Resources/myicon.png")
    .Build();
// Create a standard ribbon tab
var myTab = ribbon.CreateTab("MyTools", "My Tools", "Custom Toolset");
// Create a contextual tab that appears based on conditions
var contextual = ribbon.CreateContextualTab(
    "SelectionTools",
    selection => selection.Count > 0,
    "Selection Tools"
);
```
## XML Configuration
Define your ribbon structure using clean, readable XML:
```xml
<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<RibbonTab xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
           IsVisible="True" KeyTip="RP" 
           Name="RoadPAC2" Title="RoadPAC2" IsEnabled="True" 
           IsPanelEnabled="True" AllowTearOffContextualPanels="False">
    
    <RibbonPanel FloatingOrientation="Vertical" CanToggleOrientation="True" IsEnabled="True">
        <RibbonPanelSource Title="RoadPAC">
            <DialogLauncher Text="Open Project Folder" CommandHandler="RP_PROSPECTOR_NEW" />
            
            <RibbonButton Text="Launch RoadPAC" Name="Launch RoadPAC" ShowText="True" 
                         Size="Large" Orientation="Vertical" ShowImage="True"
                         LargeImage="rp_logo_blue_32"
                         CommandHandler="XX:RoadpacNET7.exe" />
            
            <RibbonButton Text="Project ToolSpace" Size="Large" Orientation="Vertical" 
                         ShowText="True" LargeImage="rp_AbstractClass_32" 
                         CommandHandler="RP_PROSPECTOR" />
            
            <RibbonSeparator Id="AcRibbonSeparator" SeparatorStyle="Spacer" />
            
            <RibbonRowPanel>
                <RibbonButton Text="Open Solution (.rpsol)" ShowText="True" 
                             Image="rp_FolderBrowserDialogControl_16" 
                             CommandHandler="RP_PROSPECTOR_SOLUTION" />
                <RibbonRowBreak />
                <RibbonButton Text="New Project" ShowText="True" 
                             Image="rp_AddEntity_16" 
                             CommandHandler="RP_PROSPECTOR_NEW" />
            </RibbonRowPanel>
        </RibbonPanelSource>
    </RibbonPanel>
</RibbonTab>
```
This project is licensed under the Apache License 2.0
