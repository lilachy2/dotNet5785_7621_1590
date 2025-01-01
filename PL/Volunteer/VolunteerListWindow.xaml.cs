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
        public VolunteerListWindow()
        {
            InitializeComponent();
            DataContext = this;  // Set the DataContext to the window itself for binding

            // Initialize ComboBox with Enum values for filtering
            FilterComboBox.ItemsSource = Enum.GetValues(typeof(VolInList));

            // Load the volunteer list without any filter initially
            UpdateVolunteerList();
        }

        // Handle ComboBox selection change event to update the filter
        private void FilterVolunteerlistByCriteria(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected filter from ComboBox and update the SelectedFilter property
            var selectedItem = FilterComboBox.SelectedItem as VolInList?;
            if (selectedItem.HasValue)
            {
                SelectedFilter = selectedItem.Value;
            }
        }

        // Update the volunteer list based on the selected filter
 
        private void UpdateVolunteerList()
        {
            try
            {
                IEnumerable<BO.VolunteerInList> volunteers = queryVolunteerList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    VolunteerInList = volunteers;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the volunteer list: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // This method contains the filtering logic
        private IEnumerable<BO.VolunteerInList> queryVolunteerList()
        {
            IEnumerable<BO.VolunteerInList> volunteers;

            switch (SelectedFilter)
            {
                case VolInList.Id:
                    volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, VolInList.Id).OrderBy(v => v.Id);
                    break;
                case VolInList.Name:
                    volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, VolInList.Name).OrderBy(v => v.FullName);
                    break;
                case VolInList.IsActive:
                    volunteers = BlApi.Factory.Get().Volunteer.ReadAll(true, VolInList.IsActive).Where(v => v.IsActive);
                    break;
                case VolInList.None:  // No filter (default, show all)
                    volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, null);
                    break;
                default:
                    volunteers = BlApi.Factory.Get().Volunteer.ReadAll(null, null);
                    break;
            }

            return volunteers;
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
                volunteerWindow.ShowDialog();

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
                    volunteerWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while opening volunteer details: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Delete button click handler
        //private void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SelectedVolunteer != null)
        //    {
        //        // Confirm deletion with the user
        //        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this volunteer?",
        //                                                  "Confirm Deletion",
        //                                                  MessageBoxButton.YesNo,
        //                                                  MessageBoxImage.Warning);

        //        if (result == MessageBoxResult.Yes)
        //        {
        //            try
        //            {
        //                // Call the Delete method in the BL to delete the selected volunteer
        //                BlApi.Factory.Get().Volunteer.Delete(SelectedVolunteer.Id);

        //                // Refresh the list after deletion (thanks to observer)
        //                MessageBox.Show("Volunteer deleted successfully.",
        //                                "Success",
        //                                MessageBoxButton.OK,
        //                                MessageBoxImage.Information);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"An error occurred while deleting the volunteer: {ex.Message}",
        //                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //}

        // Handle Delete button click event to delete a volunteer
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

        
    }
}
