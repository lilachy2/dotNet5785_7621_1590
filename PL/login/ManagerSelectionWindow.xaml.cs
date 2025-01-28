using System.Windows;
using System.Windows.Threading;

namespace PL.login
{
    public partial class ManagerSelectionWindow : Window
    {

        public int SelectedOption { get; private set; } = -1; // Default value indicating no selection.

        public ManagerSelectionWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = 1;
            this.DialogResult = true; // Close the window with OK status.
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            SelectedOption = 2;
            this.DialogResult = true; // Close the window with OK status.
        }
    }
}
