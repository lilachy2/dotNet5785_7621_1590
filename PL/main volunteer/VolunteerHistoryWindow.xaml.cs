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

        // Private field for SelectedStatus
        private BO.Calltype? _selectedStatus;

        // Event for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Property for SelectedStatus with change notification
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

        // Notify UI that the property has changed
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructor for initializing the window with volunteer ID
        public VolunteerHistoryWindow(int id)
        {
            InitializeComponent();
            _volunteerId = id;

            SelectedStatus = BO.Calltype.None;  // Set default value

            // Load the volunteer's call history
            var calls = s_bl.Call.GetCloseCall(id, null, null);
            this.DataContext = calls;
        }

        // Function to handle the Filter ComboBox selection change
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedCallType = comboBox.SelectedItem as BO.Calltype?;

            if (selectedCallType.HasValue)
            {
                // Update the DataContext with filtered calls based on the selected call type
                var calls = s_bl.Call.GetCloseCall(_volunteerId, selectedCallType.Value, null);
                this.DataContext = calls;
            }
        }

        // Function to handle the Sort ComboBox selection change
        private void FilterComboBox_SelectionChanged_Sort(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedClosedCallInListEnum = comboBox.SelectedItem as BO.ClosedCallInListEnum?;

            if (selectedClosedCallInListEnum.HasValue)
            {
                // Update the DataContext with filtered calls based on the selected closed call enum
                var calls = s_bl.Call.GetCloseCall(_volunteerId, null, selectedClosedCallInListEnum);
                this.DataContext = calls;
            }
        }
    }
}
