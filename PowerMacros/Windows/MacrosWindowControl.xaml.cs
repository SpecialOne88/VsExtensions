using PowerMacros.Entities;
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
        public ObservableCollection<Macro> MacrosList { get; set; } = new ObservableCollection<Macro>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MacrosWindowControl"/> class.
        /// </summary>
        public MacrosWindowControl()
        {
            // Initialize the MacrosList with some sample data
            MacrosList.Add(new Macro { Name = "Macro1", Description = "Description1", MacroType = MacroType.Code, Code = "Code1" });
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "MacrosWindow");
        }
    }
}