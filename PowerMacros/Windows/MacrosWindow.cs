using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace PowerMacros.Windows
{
    [Guid("529422d2-e5d7-48ec-88bf-a7621f7d4b53")]
    public class MacrosWindow : ToolWindowPane
    {
        public MacrosWindow() : base(null)
        {
            this.Caption = "Power Macros";
            this.Content = new MacrosWindowControl();
        }
    }
}
