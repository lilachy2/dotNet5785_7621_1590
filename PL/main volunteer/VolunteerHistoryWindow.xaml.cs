using System.Collections.Generic;
using System.Windows;

namespace PL.main_volunteer
{
    public partial class VolunteerHistoryWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerHistoryWindow(int id)
        {
            InitializeComponent();

            // טוען את רשימת הקריאות של המתנדב
            var calls = s_bl.Call.GetCloseCall(id, null,null);

            // הגדרת DataContext באופן ישיר
            this.DataContext = calls;
        }
    }
}
