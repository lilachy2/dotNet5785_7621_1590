using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    public partial class LoginSystem : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public int Id { get; set; }
        public string Password { get; set; }

        public LoginSystem()
        {
            InitializeComponent();
            DataContext = this; 

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var volunteer = s_bl.Volunteer.Read(Id);
                if (Id == 0 || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Error: Please enter full details.");
                    return;
                }

                if (volunteer == null)
                {
                    MessageBox.Show("Error: Volunteer not registered in the system.");
                    return;
                }

                if (volunteer.Role == BO.Role.Volunteer)
                {
                    VolunteerWindow volunteerWindow = new VolunteerWindow(Id);
                    volunteerWindow.ShowDialog();
                }
                else if (volunteer.Role == BO.Role.Manager)
                {
                    var result = MessageBox.Show("Select the screen you want to open:\n" +
                                                  "1. Volunteer Window\n" +
                                                  "2. Volunteer in List Window",
                                                  "Manager Selection", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        VolunteerWindow managerWindow = new VolunteerWindow(Id);
                        managerWindow.Show();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        VolunteerListWindow managerVolunteerListWindow = new VolunteerListWindow();
                        managerVolunteerListWindow.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Password = passwordBox.Password;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                // אפשר להוסיף כאן לוגיקה כדי להחזרת ערך ברירת מחדל או שינוי צבע
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // אפשר להוסיף כאן לוגיקה כדי להסיר ערך ברירת מחדל אם יש
        }

        private void IdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Id == 0)
            {
                Id = -1;
            }
        }

        private void IdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Id == -1)
            {
                Id = 0;
            }
        }
    }
}
