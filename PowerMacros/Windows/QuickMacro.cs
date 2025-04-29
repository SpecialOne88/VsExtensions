using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace PowerMacros.Windows
{
    [Guid("76875427-d752-4fa3-89ff-93bee9030444")]
    public class QuickMacro : ToolWindowPane
    {
        private readonly QuickMacroControl _control;
        public QuickMacro() : base(null)
        {
            this.Caption = "QuickMacro";
            _control = new QuickMacroControl();
            this.Content = _control;
        }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();
            _control.SetPackage(this.Package as AsyncPackage);
        }
    }
}
