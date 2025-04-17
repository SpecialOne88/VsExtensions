using PowerMacros.Entities;
using PowerMacros.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
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