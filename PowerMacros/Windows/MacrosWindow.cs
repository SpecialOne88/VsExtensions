using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace PowerMacros.Windows
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("529422d2-e5d7-48ec-88bf-a7621f7d4b53")]
    public class MacrosWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MacrosWindow"/> class.
        /// </summary>
        public MacrosWindow() : base(null)
        {
            this.Caption = "Power Macros";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new MacrosWindowControl();
        }
    }
}
