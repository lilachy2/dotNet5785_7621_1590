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
        private CallStatus _selectedFilter = CallStatus.Open;  // Default to None (no filter)
        
        // Declare the SelectedFilter property with PropertyChanged notifications
        public CallStatus SelectedFilter
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
            DependencyProperty.Register(
                "CallInList",
                typeof(IEnumerable<BO.CallInList>),
                typeof(CallListWindow),
                new PropertyMetadata(null));

        // Constructor
        public CallListWindow()
        {
            InitializeComponent();
            DataContext = this;  // Set the DataContext to the window itself for binding

            // Initialize ComboBox with Enum values for filtering
            //FilterComboBox.ItemsSource = Enum.GetValues(typeof(CallInList));

            // Load the call list without any filter initially
            UpdateCallList();
        }

        // Handle ComboBox selection change event to update the filter
        private void FilterCallListByCriteria(object _, SelectionChangedEventArgs e)
        {
            // Access the selected item directly from e
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CallStatus selectedItem)
            {
                // Update the SelectedFilter property based on the selected item
                SelectedFilter = selectedItem;
            }
        }


        // Update the call list based on the selected filter
        private void UpdateCallList()
        {
            try
            {
                //// Query and retrieve the list of calls filtered by the selected filter
                //IEnumerable<BO.CallInList> calls = queryCallList();

                //// Use the dispatcher to update the UI thread with the new call list
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    CallInList = calls;
                //});
                CallInList = null;  
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the call list update
                MessageBox.Show($"An error occurred while loading the call list: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // This method contains the filtering logic
        //private IEnumerable<BO.CallInList> queryCallList()
        //{
        //    IEnumerable<BO.CallInList> calls;

        //    switch (SelectedFilter)
        //    {
        //        case CallInList.Id:
        //            calls = BlApi.Factory.Get().Call.ReadAll(null, CallInList.Id).OrderBy(c => c.Id);
        //            break;
        //        case CallInList.CallType:
        //            calls = BlApi.Factory.Get().Call.ReadAll(null, CallInList.CallType).OrderBy(c => c.CallType);
        //            break;
        //        case CallInList.Status:
        //            calls = BlApi.Factory.Get().Call.ReadAll(CallStatus.Open, CallInList.Status).Where(c => c.Status == CallStatus.Open);
        //            break;
        //        case CallInList.None:  // No filter (default, show all)
        //            calls = BlApi.Factory.Get().Call.ReadAll(null, null);
        //            break;
        //        default:
        //            calls = BlApi.Factory.Get().Call.ReadAll(null, null);
        //            break;
        //    }

        //    return calls;
        //}

        //Handle Window loaded event to register the observer for updates
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register the observer to update the call list when changes occur in the BL
            BlApi.Factory.Get().Call.AddObserver(callListObserver);
        }

        //Handle Window closed event to remove the observer
        private void Window_Closed(object sender, EventArgs e)
        {
            // Unregister the observer when the window is closed
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

                // After adding a call, refresh the list automatically
                UpdateCallList();
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
                    // Send the ID of the selected call to the new window
                    CallWindow callWindow = new CallWindow(SelectedCall.Id);
                    callWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while opening call details: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

       
        private async void DeleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieves the Button that was clicked (sender refers to the object that triggered the event).
            var button = sender as Button;

            // Safely extracts the CommandParameter from the Button, which represents the call to be deleted.
            // The CommandParameter is expected to be of type BO.CallInList. The '?' operator ensures that if the value is null, it doesn't throw an exception.
            var callToDelete = button?.CommandParameter as BO.CallInList;

            if (callToDelete != null)
            {
                // Confirmation for deletion
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

                        // Call deletion in BL
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
    }
}
