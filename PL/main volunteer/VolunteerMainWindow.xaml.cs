using BO;
using PL.Volunteer;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PL.main_volunteer
{
    public partial class VolunteerMainWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        //  DependencyProperty
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(
                "Volunteer",
                typeof(BO.Volunteer),
                typeof(VolunteerMainWindow),
                new PropertyMetadata(null));

        //  Volunteer
        public BO.Volunteer Volunteer
        {
            get
            {
                //  UI thread
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (BO.Volunteer)GetValue(CurrentVolunteerProperty);
                }
                else
                {
                    // Dispatcher
                    return (BO.Volunteer)Application.Current.Dispatcher.Invoke(() => GetValue(CurrentVolunteerProperty));
                }
            }
            set
            {
                SetValue(CurrentVolunteerProperty, value);
                OnPropertyChanged(nameof(Volunteer));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Visibility currentCallVisibility;
        private Visibility currentCallVisibilityEnd;

        public Visibility CurrentCallVisibility
        {
            get { return currentCallVisibility; }
            set
            {
                currentCallVisibility = value;
                OnPropertyChanged(nameof(CurrentCallVisibility));
            }
        }

        public Visibility CurrentCallVisibilityEnd
        {
            get { return currentCallVisibilityEnd; }
            set
            {
                currentCallVisibilityEnd = value;
                OnPropertyChanged(nameof(CurrentCallVisibilityEnd));
            }
        }

        // בנאי של החלון
        public VolunteerMainWindow(int Id)
        {
            try
            {
                Volunteer = s_bl.Volunteer.Read(Id);

                // בדוק אם Volunteer לא שווה ל-null
                if (Volunteer == null)
                {
                    MessageBox.Show("Volunteer not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

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

       
        // פונקציה לסיום קריאה
        private void EndCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // עדכון סיום קריאה
                s_bl.Call.UpdateEndTreatment(Volunteer.Id, Volunteer.CurrentCall.CallId);
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
         private void CancelCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // עדכון סיום קריאה
                s_bl.Call.UpdateCancelTreatment(Volunteer.Id, Volunteer.CurrentCall.Id);
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

       

    }
}
