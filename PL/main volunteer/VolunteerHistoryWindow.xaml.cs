using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PL.main_volunteer
{
    public partial class VolunteerHistoryWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
         private int _volunteerId;

        //  public static readonly DependencyProperty SelectedStatusProperty =
        //DependencyProperty.Register("SelectedStatus", typeof(BO.Calltype?), typeof(VolunteerHistoryWindow), new PropertyMetadata(null, OnSelectedStatusChanged));

        //  // הגדרת המאפיין
        //  public BO.Calltype? SelectedStatus
        //  {
        //      get => (BO.Calltype?)GetValue(SelectedStatusProperty);
        //      set => SetValue(SelectedStatusProperty, value);
        //  }

        //  private static void OnSelectedStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //  {
        //      var window = d as VolunteerHistoryWindow;
        //      if (window != null)
        //      {
        //          // כאן תוכל לטפל בשינוי שנעשה ולבצע פעולות אחרות אם צריך
        //          var newValue = e.NewValue as BO.Calltype?;
        //          // לדוגמה, לבצע עדכון של ה-UI או להפעיל פונקציה על בסיס הערך החדש
        //      }
        //  }

        private BO.Calltype? _selectedStatus;
        public event PropertyChangedEventHandler PropertyChanged;

        public BO.Calltype? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged(nameof(SelectedStatus));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public VolunteerHistoryWindow(int id)
        {

            InitializeComponent();
            _volunteerId = id;

            SelectedStatus = BO.Calltype.None;

            // טוען את רשימת הקריאות של המתנדב
            var calls = s_bl.Call.GetCloseCall(id, null, null);
            this.DataContext = calls;
        }

        // פונקציה שמופעלת כאשר המשתמש משנה את הבחירה ב-Filter ComboBox
     
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           

            var comboBox = sender as ComboBox;
            var selectedCallType = comboBox.SelectedItem as BO.Calltype?;

            if (selectedCallType.HasValue)
            {
                // שליחה לפונקציה כדי לעדכן את הקריאות לפי הסוג שנבחר
                var calls = s_bl.Call.GetCloseCall(_volunteerId, selectedCallType.Value, null);
                this.DataContext = calls;  // עדכון ה-DataContext עם הקריאות המעודכנות
            }
        }
        private void FilterComboBox_SelectionChanged_Sort(object sender, SelectionChangedEventArgs e)
        {
           

            var comboBox = sender as ComboBox;
            var selectedClosedCallInListEnum = comboBox.SelectedItem as BO.ClosedCallInListEnum?;

            if (selectedClosedCallInListEnum.HasValue)
            {
                // שליחה לפונקציה כדי לעדכן את הקריאות לפי הסוג שנבחר
                var calls = s_bl.Call.GetCloseCall(_volunteerId, null, selectedClosedCallInListEnum);
                this.DataContext = calls;  // עדכון ה-DataContext עם הקריאות המעודכנות
            }
        }

    }
}
