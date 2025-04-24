using PowerMacros.ViewModels;
using System.Windows.Controls;

namespace PowerMacros.Windows
{
    /// <summary>
    /// Interaction logic for MacrosWindowControl.
    /// </summary>
    public partial class MacrosWindowControl : UserControl
    {
        public MacrosWindowControl()
        {
            this.InitializeComponent();
            DataContext = new MacrosWindowViewModel();
        }
    }
}