using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BO;
using BlApi;
using System.ComponentModel;

namespace PL.main_volunteer
{
    public partial class ChooseCallWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int _volunteerId;
        private BO.Volunteer _volunteer;
        private List<BO.OpenCallInList> _callsList;

        public event PropertyChangedEventHandler PropertyChanged;

        public ChooseCallWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            LoadVolunteerData();
            this.DataContext = this;  // קביעת DataContext לעבודה עם Binding

        }

        // תכונת Volunteer עם INotifyPropertyChanged
        public BO.Volunteer Volunteer
        {
            get => _volunteer;
            set
            {
                if (_volunteer != value)
                {
                    _volunteer = value;
                    OnPropertyChanged(nameof(Volunteer));  // להודיע על שינוי בתכונה
                    LoadOpenCalls(); // טוען את הקריאות הפתוחות מחדש
                }
            }
        }

        // תכונת CallsList
        public List<BO.OpenCallInList> CallsList
        {
            get => _callsList;
            set
            {
                if (_callsList != value)
                {
                    _callsList = value;
                    OnPropertyChanged(nameof(CallsList));  // להודיע על שינוי בתכונה
                }
            }
        }

        // Method to load volunteer data
        private void LoadVolunteerData()
        {
            try
            {
                Volunteer = s_bl.Volunteer.Read(_volunteerId);  // Get volunteer data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading volunteer data: {ex.Message}");
            }
        }

        // Method to load open calls for the volunteer
        private void LoadOpenCalls()
        {
            if (Volunteer != null)
            {
                var calls = s_bl.Call.GetOpenCall(_volunteerId, null, null);  // Get open calls
                CallsList = calls.ToList();  // Assuming CallsList is a List<OpenCallInList> that will be bound to the UI
            }
        }

        // Method to notify property change
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
