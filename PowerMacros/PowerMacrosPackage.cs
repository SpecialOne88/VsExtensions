using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace PowerMacros
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PowerMacrosPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(PowerMacros.Windows.MacrosWindow))]
    [ProvideToolWindow(typeof(PowerMacros.Windows.QuickMacro))]
    public sealed class PowerMacrosPackage : AsyncPackage
    {
        public const string PackageGuidString = "b64d8a74-a3e4-41c7-b6f0-fa6ac20b832a";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await PowerMacros.Commands.MacrosCommand.InitializeAsync(this);
        }
    }
}
