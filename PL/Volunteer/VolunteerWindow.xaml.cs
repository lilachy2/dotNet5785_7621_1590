using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        // Dependency Property for button text (Add/Update)
        public string ButtonText { get; set; }

        // The ID of the entity, which determines whether the screen is for adding or updating
        public int Id { get; set; }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Reference to the BL layer

        // Dependency Property to bind to Volunteer data
        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set
            {
                SetValue(CurrentVolunteerProperty, value);
                OnPropertyChanged(nameof(Volunteer));  // Notify of property change
            }
        }

        // The DependencyProperty that will hold the Volunteer data (to be used in XAML bindings)
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null, OnVolunteerChanged));

        public IEnumerable<BO.DistanceType> DistanceTypes
        {
            get { return Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>(); }
        }
        public IEnumerable<BO.Role> Roles
        {
            get { return Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>(); }
        }

        public VolunteerWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update"; // Set button text based on whether we are adding or updating
            DataContext = this;

            InitializeComponent(); // Initialize the XAML components

            try
            {
                // If Id is 0, create a new Volunteer; otherwise, retrieve an existing one from the BL
                Volunteer = (Id != 0)
                    ? s_bl.Volunteer.Read(Id) // Retrieve Volunteer data from the BL
                    : new BO.Volunteer() // Create a new Volunteer object with default values
                    {
                        Id = 0,
                        Name = string.Empty,
                        Number_phone = string.Empty,
                        Email = string.Empty,
                        FullCurrentAddress = null,
                        Password = null,
                        Latitude = null,
                        Longitude = null,
                        Role = BO.Role.Volunteer,
                        Active = false,
                        Distance = null,
                        DistanceType = BO.DistanceType.Aerial_distance,
                        TotalHandledCalls = 0,
                        TotalCancelledCalls = 0,
                        TotalExpiredCalls = 0,
                        CurrentCall = null
                    };

                // Register observer if volunteer data exists and has an ID
                if (Volunteer != null && Volunteer.Id != 0)
                {
                    SubscribeToVolunteerUpdates(Volunteer.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Callback method to handle the volunteer update when Volunteer data changes
        private static void OnVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (VolunteerWindow)d;
            var newVolunteer = (BO.Volunteer)e.NewValue;

            if (newVolunteer != null && newVolunteer.Id != 0)
            {
                // Here you can add code to manage observer logic for updating data
                window.SubscribeToVolunteerUpdates(newVolunteer.Id);
            }
        }

        // Method to subscribe to updates for a volunteer
        private void SubscribeToVolunteerUpdates(int volunteerId)
        {
            // Subscribe to updates for the volunteer using the AddObserver method
            s_bl.Volunteer.AddObserver(volunteerId, HandleVolunteerUpdate);
        }

        // Method to handle updates for volunteer data
        private void HandleVolunteerUpdate()
        {
            try
            {
                // Retrieve the updated volunteer data
                Volunteer = s_bl.Volunteer.Read(Volunteer.Id);
                // Optional: You can add UI-related updates here
                MessageBox.Show("Volunteer data updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the operation
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Button click handler for adding or updating the volunteer
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Id == 0)
            {
                // Add new Volunteer
                AddVolunteer();
            }
            else
            {
                // Update existing Volunteer
                UpdateVolunteer();
            }
        }

        private async void AddVolunteer()
        {
            try
            {
                // ביצוע הלוגיקה העסקית
                var volunteer = Volunteer;
                await Task.Run(() => s_bl.Volunteer.Create(volunteer));

                // חזרה ל-UI thread באופן מפורש
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async void UpdateVolunteer()
        {
            try
            {
                // ביצוע העדכון בלוגיקה העסקית
                var volunteer = Volunteer;
                await Task.Run(() => s_bl.Volunteer.Update(volunteer, volunteer.Id));

                // חזרה ל-UI thread והצגת הודעת הצלחה
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות על ה-UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        // Method to remove the observer when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Volunteer != null && Volunteer.Id != 0)
            {
                // Remove observer
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, HandleVolunteerUpdate);
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void cbDistanceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.DistanceType selectedDistanceType)
            {
                Console.WriteLine($"Selected DistanceType: {selectedDistanceType}");
            }
        }
        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Role selectedDistanceType)
            {
                Console.WriteLine($"Selected DistanceType: {selectedDistanceType}");
            }
        }
    }
}
