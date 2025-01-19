using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PL.main_volunteer
{
    public partial class VolunteerHistoryWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int _volunteerId;
        private BO.Calltype? _selectedStatus = BO.Calltype.None;  // Default to None (no filter)
        private BO.ClosedCallInListEnum? _selectedSort = BO.ClosedCallInListEnum.None; // Default to None (no sorting)


        // Event for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
      
        public static readonly DependencyProperty ClosedCallInListProperty =
           DependencyProperty.Register("ClosedCallInList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(VolunteerHistoryWindow), new PropertyMetadata(null));

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public IEnumerable<BO.ClosedCallInList?> ClosedCallInList
        {
            get => (IEnumerable<BO.ClosedCallInList?>)GetValue(ClosedCallInListProperty);
            set => SetValue(ClosedCallInListProperty, value);
        }
        // Property for SelectedStatus with change notification
        public BO.Calltype? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    LoadVolunteerHistory();
                }
            }
        }
      
        public BO.ClosedCallInListEnum? SelectedSort
        {
            get { return _selectedSort; }
            set
            {
                if (_selectedSort != value)
                {
                    _selectedSort = value;
                    OnPropertyChanged(nameof(_selectedSort));  // Notify the UI of the property change
                    LoadVolunteerHistory();  // Update the list when the filter changes
                }
            }
        }

      
        // Constructor for initializing the window with volunteer ID
        public VolunteerHistoryWindow(int id)
        {
            InitializeComponent();
            _volunteerId = id;
            // Load the volunteer's call history asynchronously
            LoadVolunteerHistory();
            this.DataContext = this;  // Set the DataContext to this window for binding

        }

        // Synchronously load the volunteer's call history
        private void LoadVolunteerHistory()
        {
            try
            {
                // Fetch the calls synchronously, but call this in a separate thread or task
                var calls = s_bl.Call.GetCloseCall(_volunteerId, SelectedStatus, SelectedSort);

                // Set the DataContext after the calls are retrieved
                ClosedCallInList = calls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading volunteer history: {ex.Message}");
            }
        }

        // Function to handle the Filter ComboBox selection change
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Calltype selectedItem)
            {
                SelectedStatus = selectedItem;
                LoadVolunteerHistory();

            }
        }

        // Function to handle the Sort ComboBox selection change
        private void FilterComboBox_SelectionChanged_Sort(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.ClosedCallInListEnum selectedItem)
            {
                SelectedSort = selectedItem;
                LoadVolunteerHistory();

            }
        }
    }
}
