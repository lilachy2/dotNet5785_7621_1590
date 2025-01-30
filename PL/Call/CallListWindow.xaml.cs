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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PL.Call
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private BO.Calltype _selectedFilter = BO.Calltype.None;  // Default to None (no filter)
        private CallInListField _selectedSortField = CallInListField.None; // Default to None (no sorting)
        private BO.CallStatus _selectedFilterStatus = BO.CallStatus.NoneToFilter;  // Default to None (no filter)

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private int? volId; // to get volunterrID to oter Window
        public BO.CallStatus SelectedFilterStatus
        {
            get { return _selectedFilterStatus; }
            set
            {
                if (_selectedFilterStatus != value)
                {
                    _selectedFilterStatus = value;
                    OnPropertyChanged(nameof(SelectedFilter));  // Notify the UI of the property change
                    callListObserver();  // Update the list when the filter changes
                }
            }
        }
        public BO.Calltype SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));  // Notify the UI of the property change
                    callListObserver();  // Update the list when the filter changes
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
                    callListObserver();  // Update the list when the sort field changes
                }
            }
        }

        private bool _isBalloonVisible;
        public bool IsBalloonVisible
        {
            get => _isBalloonVisible;
            set
            {
                if (_isBalloonVisible != value)
                {
                    _isBalloonVisible = value;
                    OnPropertyChanged(nameof(IsBalloonVisible));
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

       
        public CallListWindow(int? volId1, BO.CallStatus? initialStatusFilter = null)
        {
            InitializeComponent();
            DataContext = this;
            IsBalloonVisible = false;   
            this.volId = volId1; // to get volunterrID to other Window

            if (initialStatusFilter.HasValue)
            {
                // Set the initial filter and update the call list based on it
                SelectedFilterStatus = initialStatusFilter.Value;
            }

            callListObserver();
        }


        // Handle ComboBox selection change event to update the filter
        private void FilterCallListByCriteria(object _, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Calltype selectedItem)
            {
                SelectedFilter = selectedItem;
                queryCallList();

            }
        }

        // Handle ComboBox selection change event to update the sort field
        private void SortCallListByCriteria(object _, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CallInListField selectedItem)
            {
                SelectedSortField = selectedItem;
                queryCallList();
            }
        }

        // Update the call list based on the selected filter and sort field
        private void callListObserver()
        {
            try
            {
                // Query and retrieve the list of calls filtered and sorted by the selected criteria
                if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                    _observerOperation = Dispatcher.BeginInvoke(() => {
                        queryCallList();});

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the call list: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           s_bl.Call.AddObserver(callListObserver);
        }

        // Handle Window closed event to remove the observer
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
        }

        
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

        private async void AddCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CallWindow callWindow = new CallWindow();
                callWindow.Show();
                callListObserver();

                IsBalloonVisible = true;


                await Task.Delay(5000); // המתנה של 5 שניות
                IsBalloonVisible = false;
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
                    callWindow.Show();
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
                        //BlApi.Factory.Get().Call.UpdateCancelTreatment(callToCancel.Id ?? 0, callToCancel.CallId);
                       s_bl.Call.UpdateCancelTreatment(volId ?? 0, callToCancel.Id??0);


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
                        //BlApi.Factory.Get().Call.Delete(callToDelete.Id ?? 0);
                        BlApi.Factory.Get().Call.Delete(callToDelete.CallId);

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

       
        public void queryCallList()
        {
            CallInList = ((_selectedFilter == BO.Calltype.None) ?
               s_bl?.Call.GetCallsList(null, null, SelectedSortField,null)! :
               s_bl.Call.GetCallsList(SelectedFilter, SelectedFilter, SelectedSortField, SelectedFilterStatus)!);
        }

    }
}
