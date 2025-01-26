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
using System.Security.Cryptography;


namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window, INotifyPropertyChanged
    {
        private VolInList _selectedFilter = VolInList.None;  // Default to None (no filter)

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Declare the SelectedFilter property with PropertyChanged notifications
        public VolInList SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));  // Notify the UI of the property change
                    UpdateVolunteerList();  // Update the list when the filter changes
                }
            }
        }
        // Regular property for the selected volunteer (not DependencyProperty)
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        
        // Declare the VolunteerInList property with DependencyProperty
        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolInListProperty); }
            set { SetValue(VolInListProperty, value); }
        }

        // Register the DependencyProperty for VolunteerInList
        public static readonly DependencyProperty VolInListProperty =
            DependencyProperty.Register(
                "VolunteerInList",
                typeof(IEnumerable<BO.VolunteerInList>),
                typeof(VolunteerListWindow),
                new PropertyMetadata(null));

        // Constructor
        public VolunteerListWindow(Window previousWindow = null)
        {
            _previousWindow = previousWindow;
            InitializeComponent();
            DataContext = this;  // Set the DataContext to the window itself for binding

            UpdateVolunteerList();
        }

        // Handle ComboBox selection change event to update the filter
        private void FilterVolunteerlistByCriteria(object _, SelectionChangedEventArgs e)
        {
            // Access the selected item directly from e
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is VolInList selectedItem)
            {
                // Update the SelectedFilter property based on the selected item
                SelectedFilter = selectedItem;
            }
        }


        // Update the volunteer list based on the selected filter
        private void UpdateVolunteerList()
        {
            try
            {
                // Query and retrieve the list of volunteers filtered by the selected filter
                //IEnumerable<BO.VolunteerInList> volunteers = queryVolunteerList();

                //// Use the dispatcher to update the UI thread with the new volunteer listdoubl
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    VolunteerInList = volunteers;
                //});
                QueryVolunteerList();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the volunteer list update
                MessageBox.Show($"An error occurred while loading the volunteer list: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // This method contains the filtering logic
        //private IEnumerable<BO.VolunteerInList> queryVolunteerList()
        //{
        //    IEnumerable<BO.VolunteerInList> volunteers;

        //    volunteers = s_bl.Volunteer.ReadAll(null, SelectedFilter);


        //    return volunteers;
        //}
        public void QueryVolunteerList()
        {
            VolunteerInList = (SelectedFilter == BO.VolInList.None) ?
                s_bl?.Volunteer.ReadAll(null, null)! :
                s_bl?.Volunteer.ReadAll(null, SelectedFilter)!;
        }

            //Handle Window loaded event to register the observer for updates
            private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register the observer to update the volunteer list when changes occur in the BL
            BlApi.Factory.Get().Volunteer.AddObserver(volunteerListObserver);
        }

        //Handle Window closed event to remove the observer
        private void Window_Closed(object sender, EventArgs e)
        {
            // Unregister the observer when the window is closed
            BlApi.Factory.Get().Volunteer.RemoveObserver(volunteerListObserver);

        }
       private void volunteerListObserver()
        {
            UpdateVolunteerList();  // Refresh the list after a change occurs
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void AddVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create and open the volunteer window to add a new volunteer
                VolunteerWindow volunteerWindow = new VolunteerWindow();
                volunteerWindow.Show();

                // After adding a volunteer, refresh the list automatically
                UpdateVolunteerList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the volunteer: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Double-click event to view a single volunteer's details

      

        private void VolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                try
                {
                    // Send the ID of the selected volunteer to the new window
                    VolunteerWindow volunteerWindow = new VolunteerWindow(SelectedVolunteer.Id);
                    volunteerWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while opening volunteer details: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

       
        private async void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieves the Button that was clicked (sender refers to the object that triggered the event).
            var button = sender as Button;

            // Safely extracts the CommandParameter from the Button, which represents the volunteer to be deleted.
            // The CommandParameter is expected to be of type BO.VolunteerInList. The '?' operator ensures that if the value is null, it doesn't throw an exception.
            var volunteerToDelete = button?.CommandParameter as BO.VolunteerInList;

            if (volunteerToDelete != null)
            {
                // אישור למחיקה
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this volunteer?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        BlApi.Factory.Get().Volunteer.Delete(volunteerToDelete.Id);

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            MessageBox.Show("Volunteer deleted successfully.",
                                         "Success",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Information);
                        });

                        // קריאה למחיקה ב-BL
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting the volunteer: {ex.Message}",
                                         "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // for bottom bake

        private Window _previousWindow; // Variable to store a reference to the previous window
        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            if (_previousWindow != null)
            {
                _previousWindow.Show(); // Show the previous window
                this.Hide(); // Close the current window
            }
            else
            {
                MessageBox.Show("Previous window is null!");
            }

        }
    }
}

