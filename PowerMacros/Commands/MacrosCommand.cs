using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PowerMacros.Utils;
using System;
using System.ComponentModel.Design;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace PowerMacros.Commands
{
    internal sealed class MacrosCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("5e78657d-af1d-49b2-9217-a40c01798159");
        private readonly AsyncPackage package;

        public const int Macro1CommandId = 0x0200;
        public const int Macro2CommandId = 0x0201;
        public const int Macro3CommandId = 0x0202;
        public const int Macro4CommandId = 0x0203;
        public const int Macro5CommandId = 0x0204;
        public const int Macro6CommandId = 0x0205;
        public const int Macro7CommandId = 0x0206;
        public const int Macro8CommandId = 0x0207;
        public const int Macro9CommandId = 0x0208;
        public const int Macro0CommandId = 0x0209;

        public const int QuickMacroCommandId = 0x020A;

        private MacrosCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);

            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(1); }, new CommandID(CommandSet, Macro1CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(2); }, new CommandID(CommandSet, Macro2CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(3); }, new CommandID(CommandSet, Macro3CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(4); }, new CommandID(CommandSet, Macro4CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(5); }, new CommandID(CommandSet, Macro5CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(6); }, new CommandID(CommandSet, Macro6CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(7); }, new CommandID(CommandSet, Macro7CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(8); }, new CommandID(CommandSet, Macro8CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(9); }, new CommandID(CommandSet, Macro9CommandId)));
            commandService.AddCommand(new MenuCommand((s, e) => { ExecuteMacro(0); }, new CommandID(CommandSet, Macro0CommandId)));

            var quickMacro = new CommandID(CommandSet, QuickMacroCommandId);
            var quickMenu = new MenuCommand(this.ExecuteQuickMacro, quickMacro);
            commandService.AddCommand(quickMenu);
        }

        public static MacrosCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in MacrosCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new MacrosCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Open MacrosWindow
            var window = this.package.FindToolWindow(typeof(PowerMacros.Windows.MacrosWindow), 0, true);
            if (window == null || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create window");
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void ExecuteMacro(int index)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var macros = MacroLoader.LoadMacrosFromSettings();

            if (macros == null || macros.Count == 0)
            {
                return;
            }

            var selectedMacro = macros.FirstOrDefault(x => x.Shortcut.Equals($"Shift+Ctrl+M, {index}"));
            if (selectedMacro == null)
            {
                return;
            }

            if (selectedMacro.MacroType == Entities.MacroType.Code)
            {
                TextEditor.InsertTextInCurrentView(selectedMacro.Code);
            }
        }

        private void ExecuteQuickMacro(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var window = this.package.FindToolWindow(typeof(PowerMacros.Windows.QuickMacro), 0, true);
            if (window == null || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create window");
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            const int width = 250;
            const int height = 100;
            windowFrame.SetFramePos(
                VSSETFRAMEPOS.SFP_fSize,
                Guid.Empty,
                0,
                0,
                width,
                height
            );
            windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_FloatOnly);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
