using System.Windows;
using System.Windows.Threading;

namespace PL.login
{
    public partial class ManagerSelectionWindow : Window
    {

        // משתנה סטטי שבודק אם יש מנהל פעיל
        private static bool isManagerLoggedIn = false;

        public int SelectedOption { get; private set; } = -1; // Default value indicating no selection.

        public ManagerSelectionWindow()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (isManagerLoggedIn)
            {
                MessageBox.Show("Error: Another manager is already logged in. You cannot log in until they log out.",
                                "Login Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // סימון שמנהל מחובר
            isManagerLoggedIn = true;
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
