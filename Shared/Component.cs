#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Runtime;
#else
using Autodesk.AutoCAD.Runtime;
#endif
#endregion

using System.Diagnostics;

[assembly: CommandClass(typeof(RibbonXml.Component))]
namespace RibbonXml
{
    public class Component : IExtensionApplication
    {
        public void Initialize()
        {
            Debug.WriteLine($"[&] Loaded ! {GetType().FullName}");
        }

        public void Terminate()
        {
            Debug.WriteLine($"[&] Bye !");
        }
    }
}