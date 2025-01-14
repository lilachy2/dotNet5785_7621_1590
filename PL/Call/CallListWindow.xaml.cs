using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;
using BlApi;

namespace PL.Call
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private CallInListField _selectedFilter = CallInListField.None;  // Default to None (no filter)
        private CallInListField _selectedSortField = CallInListField.None; // Default to None (no sorting)

        // Declare the SelectedFilter property with PropertyChanged notifications
        public CallInListField SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));  // Notify the UI of the property change
                    UpdateCallList();  // Update the list when the filter changes
                }
            }
        }

        // Declare the SelectedSortField property with PropertyChanged notifications
        public CallInListField SelectedSortField
        {
            get { return _selectedSortField; }
            set
            {
                if (_selectedSortField != value)
                {
                    _selectedSortField = value;
                    OnPropertyChanged(nameof(SelectedSortField));  // Notify the UI of the property change
                    UpdateCallList();  // Update the list when the sort field changes
                }
            }
        }

        // Regular property for the selected call (not DependencyProperty)
        public BO.CallInList? SelectedCall { get; set; }

        // Declare the CallInList property with DependencyProperty
        public IEnumerable<BO.CallInList> CallInList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        // Register the DependencyProperty for CallInList
        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallInList", typeof(IEnumerable<BO.CallInList>),
                typeof(CallListWindow), new PropertyMetadata(null));

        // Constructor
        public CallListWindow()
        {
            InitializeComponent();
            DataContext = this;
            UpdateCallList();
        }

        // Handle ComboBox selection change event to update the filter
        private void FilterCallListByCriteria(object _, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CallInListField selectedItem)
            {
                SelectedFilter = selectedItem;
            }
        }

        // Handle ComboBox selection change event to update the sort field
        private void SortCallListByCriteria(object _, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CallInListField selectedItem)
            {
                SelectedSortField = selectedItem;
            }
        }

        // Update the call list based on the selected filter and sort field
        private void UpdateCallList()
        {
            try
            {
                // Query and retrieve the list of calls filtered and sorted by the selected criteria
                IEnumerable<BO.CallInList> callInLists = queryCallList();

                // Use the dispatcher to update the UI thread with the new call list
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CallInList = callInLists;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the call list: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlApi.Factory.Get().Call.AddObserver(callListObserver);
        }

        // Handle Window closed event to remove the observer
        private void Window_Closed(object sender, EventArgs e)
        {
            BlApi.Factory.Get().Call.RemoveObserver(callListObserver);
        }

        private void callListObserver()
        {
            UpdateCallList();  // Refresh the list after a change occurs
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create and open the call window to add a new call
                CallWindow callWindow = new CallWindow();
                callWindow.ShowDialog();
                UpdateCallList(); // After adding a call, refresh the list automatically
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the call: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Double-click event to view a single call's details
        private void CallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                try
                {
                    CallWindow callWindow = new CallWindow(SelectedCall.CallId);
                    callWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while opening call details: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void CancelCallButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var callToCancel = button?.CommandParameter as BO.CallInList;

            if (callToCancel != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel this call?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        BlApi.Factory.Get().Call.UpdateCancelTreatment(callToCancel.Id ?? 0, callToCancel.CallId);

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            MessageBox.Show("Call Cancel successfully.",
                                         "Success",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Information);
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while Cancel the call: {ex.Message}",
                                         "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void DeleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var callToDelete = button?.CommandParameter as BO.CallInList;

            if (callToDelete != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this call?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        BlApi.Factory.Get().Call.Delete(callToDelete.Id ?? 0);

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            MessageBox.Show("Call deleted successfully.",
                                         "Success",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Information);
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting the call: {ex.Message}",
                                         "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private IEnumerable<BO.CallInList> queryCallList()
        {
            IEnumerable<BO.CallInList> calls;

            switch (SelectedFilter)
            {
                case CallInListField.Id:
                    calls = s_bl.Call.GetCallsList(CallInListField.Id, null, null);
                    break;
                case CallInListField.CallType:
                    calls = s_bl.Call.GetCallsList(CallInListField.CallType, null, null);
                    break;
                case CallInListField.Status:
                    calls = s_bl.Call.GetCallsList(CallInListField.Status, null, null);
                    break;
                case CallInListField.None:  // No filter (default, show all)
                    calls = s_bl.Call.GetCallsList(null, null, null);
                    break;
                default:
                    calls = s_bl.Call.GetCallsList(null, null, null);
                    break;
            }

            // Apply sorting based on the selected sort field
            switch (SelectedSortField)
            {
                case CallInListField.Id:
                    calls = calls.OrderBy(c => c.Id);
                    break;
                case CallInListField.CallType:
                    calls = calls.OrderBy(c => c.CallType);
                    break;
                case CallInListField.Status:
                    calls = calls.OrderBy(c => c.Status);
                    break;
                case CallInListField.None:  // No sorting (default)
                    break;
            }

            return calls;
        }
    }
}
