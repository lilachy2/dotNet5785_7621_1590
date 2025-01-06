using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.main_volunteer
{
    public partial class VolunteerMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // פרטי המתנדב
        public BO.Volunteer Volunteer { get; set; }

        // שדה שמראה אם יש קריאה פעילה
        public Visibility CurrentCallVisibility { get; set; }
        public Visibility CurrentCallVisibilityEnd { get; set; }

        public VolunteerMainWindow(int Id)
        {
            try
            {
                if (Id != 0)
                    Volunteer = s_bl.Volunteer.Read(Id);

                // הגדרת מצב של Visibility לפי קריאה פעילה
                if (Volunteer.CurrentCall == null)
                {
                    CurrentCallVisibility = Visibility.Visible; // מתנדב יכול לבחור קריאה חדשה
                    CurrentCallVisibilityEnd = Visibility.Hidden; // אין צורך בכפתור סיום קריאה
                }
                else
                {
                    CurrentCallVisibility = Visibility.Hidden; // אין צורך לבחור קריאה חדשה
                    CurrentCallVisibilityEnd = Visibility.Visible; // כפתור לסיים קריאה פעילה
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeComponent();
            DataContext = this;
        }

        // פונקציה לבחירת קריאה לטיפול
        private void ChooseCallButton_Click(object sender, RoutedEventArgs e)
        {
            // מציג מסך לבחירת קריאה
            var chooseCallWindow = new ChooseCallWindow(Volunteer.Id);
            chooseCallWindow.ShowDialog();
        }

        // פונקציה לסיום קריאה
        private void EndCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // עדכון סיום קריאה
                s_bl.Call.EndCall(Volunteer.CurrentCall.Id, Volunteer.Id);
                MessageBox.Show("The call has been marked as completed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // עדכון הקריאה הפעילה
                Volunteer.CurrentCall = null;
                CurrentCallVisibility = Visibility.Visible;
                CurrentCallVisibilityEnd = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error ending call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // גישה להיסטוריית קריאות
        private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new CallHistoryWindow();
            historyWindow.ShowDialog();
        }

        // גישה להיסטוריית קריאות של המתנדב
        private void ViewVolunteerHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var volunteerHistoryWindow = new VolunteerHistoryWindow(Volunteer.Id);
            volunteerHistoryWindow.ShowDialog();
        }
    }
}
