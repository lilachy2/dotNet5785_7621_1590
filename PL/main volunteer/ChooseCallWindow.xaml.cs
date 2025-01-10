using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BO;
using BlApi;
using System.Windows.Controls;

namespace PL.main_volunteer
{
    public partial class ChooseCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int _volunteerId;

        public ChooseCallWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            LoadVolunteerData();
        }

        // DependencyProperty for Volunteer
        public BO.Volunteer Volunteer
        {
            get
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (BO.Volunteer)GetValue(CurrentVolunteerProperty);
                }
                else
                {
                    return (BO.Volunteer)Application.Current.Dispatcher.Invoke(() => GetValue(CurrentVolunteerProperty));
                }
            }
            set
            {
                SetValue(CurrentVolunteerProperty, value);
                OnPropertyChanged(nameof(Volunteer));
            }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(ChooseCallWindow),
                new PropertyMetadata(null, OnVolunteerChanged));

        private static void OnVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as ChooseCallWindow;
            if (window != null)
            {
                window.LoadOpenCalls(); // Reload calls when volunteer data changes
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

        // Property to bind to the ItemsControl in XAML
        public List<BO.OpenCallInList> CallsList { get; set; }

        // Click handler for selecting a call
        //private void SelectCallButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var selectedCall = (BO.OpenCallInList)((Button)sender).DataContext;
        //    s_bl.Call.AssignCallToVolunteer(selectedCall.Id, _volunteerId);  // Assign call to the volunteer
        //    LoadOpenCalls();  // Refresh the call list
        //}

        // DependencyProperty Change Notification (optional)
        private void OnPropertyChanged(string propertyName)
        {
            // You can implement property change notification here if needed
        }
    }
}
