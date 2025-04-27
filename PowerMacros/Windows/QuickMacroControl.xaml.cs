using Microsoft.VisualStudio.Shell;
using PowerMacros.ViewModels;
using System.Windows.Controls;

namespace PowerMacros.Windows
{
    public partial class QuickMacroControl : UserControl
    {
        private readonly QuickMacroViewModel _viewModel;

        public QuickMacroControl()
        {
            this.InitializeComponent();
            _viewModel = new QuickMacroViewModel();
            DataContext = _viewModel;
        }

        public void SetPackage(AsyncPackage package)
        {
            _viewModel.SetPackage(package);
        }
    }
}