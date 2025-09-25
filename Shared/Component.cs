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
        class TestHandler : CommandHandler
        {
            public TestHandler(string command) : base(command)
            { }

            public override void Execute(object _)
            {
                Debug.WriteLine($"I have been executed {Command}");
            }
        }
        public void Initialize()
        {
            Debug.WriteLine($"[&RibbonXml] Loaded ! {GetType().FullName}");

        RibbonDef def = new RibbonDef.RibbonRegistry()
                .SetDefaultHandler(typeof(TestHandler))
                .AddCommandHandler(new TestHandler("TEST_COMMAND"))
                .Build(System.Reflection.Assembly.GetExecutingAssembly());
        }

        public void Terminate()
        {
            Debug.WriteLine($"[&RibbonXml] Bye !");
        }
    }
}