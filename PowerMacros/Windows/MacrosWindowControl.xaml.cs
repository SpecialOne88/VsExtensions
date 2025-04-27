using PowerMacros.ViewModels;
using System.Windows.Controls;

namespace PowerMacros.Windows
{
    public partial class MacrosWindowControl : UserControl
    {
        public MacrosWindowControl()
        {
            this.InitializeComponent();
            DataContext = new MacrosWindowViewModel();
        }
    }
}