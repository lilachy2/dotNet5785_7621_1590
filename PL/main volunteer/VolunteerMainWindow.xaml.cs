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
                var fordebug = s_bl.Volunteer.Read(Id);
                Volunteer = fordebug;

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

        private bool _originalActiveStatus; // משתנה לשמירת הסטטוס המקורי

        private void UpdateVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // שמור את הסטטוס המקורי לפני ניסיון העדכון
                _originalActiveStatus = Volunteer.Active;

                // ניסיון לעדכן את המתנדב
                s_bl.Volunteer.Update(Volunteer, Volunteer.Id);

                MessageBox.Show("Your details have been successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (BlCan_chang_to_NotActivException ex)
            {
                // החזר את ה-CheckBox למצבו המקורי במקרה של חריגה
                Volunteer.Active = _originalActiveStatus;
                MessageBox.Show($"Error updating volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbDistanceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // בדיקה אם הוספו פריטים לבחירה
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.DistanceType selectedDistanceType)
            {
                // הדפסת הערך שנבחר בקונסולה
                Console.WriteLine($"Selected DistanceType: {selectedDistanceType}");

                // אם אתה רוצה, אתה יכול גם לעדכן את מודל המתנדב כאן
                // למשל, אם אתה מעדכן את ה- DistanceType של המתנדב:
                Volunteer.DistanceType = selectedDistanceType;
            }
        }

        // פונקציה לסיום קריאה
        private void EndCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // עדכון סיום קריאה
                s_bl.Call.UpdateEndTreatment(Volunteer.Id, Volunteer.CurrentCall.Id);
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

        private void ViewVolunteerHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var volunteerHistoryWindow = new VolunteerHistoryWindow(Volunteer.Id);
            volunteerHistoryWindow.ShowDialog();
        }
        private void ChooseCallButton_Click(object sender, RoutedEventArgs e)
        {
            var chooseCallWindow = new ChooseCallWindow(Volunteer.Id);
            chooseCallWindow.ShowDialog();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
