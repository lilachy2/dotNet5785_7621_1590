using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading.Tasks;
using DO;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        // Dependency with BL (Business Logic)
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Text for the button (Add/Update)
        public string ButtonText { get; set; }

        // Volunteer ID
        public int Id { get; set; }

        // Current volunteer data (used for binding to UI)
        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set
            {
                SetValue(CurrentVolunteerProperty, value);
                OnPropertyChanged(nameof(Volunteer)); // Notify when volunteer data changes
            }
        }

        // Dependency Property for Volunteer to support data binding
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow),
                new PropertyMetadata(null, OnVolunteerChanged));

        // Enum for available distance types
        public IEnumerable<BO.DistanceType> DistanceTypes =>
            Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

        // Enum for available roles
        public IEnumerable<BO.Role> Roles =>
            Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();

        // Constructor for VolunteerWindow, takes an optional volunteer ID
        public VolunteerWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update"; // Set button text based on ID
            DataContext = this; // Set DataContext to this object for data binding

            InitializeComponent();

            try
            {
                // Initialize the volunteer object if the ID is not 0
                Volunteer = (Id != 0)
                    ? s_bl.Volunteer.Read(Id)
                    : new BO.Volunteer()
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

                // Subscribe to updates for the volunteer if an ID exists
                if (Volunteer != null && Volunteer.Id != 0)
                {
                    SubscribeToVolunteerUpdates(Volunteer.Id);
                }
            }
            catch (Exception ex)
            {
                // Display error message if there's an issue
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Called when the volunteer data changes (for example, when the dependency property is updated)
        private static void OnVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (VolunteerWindow)d;
            var newVolunteer = (BO.Volunteer)e.NewValue;

            if (newVolunteer != null && newVolunteer.Id != 0)
            {
                // Subscribe to updates for the new volunteer
                window.SubscribeToVolunteerUpdates(newVolunteer.Id);
            }
        }

        // Method to subscribe to volunteer updates from the BL
        private void SubscribeToVolunteerUpdates(int volunteerId)
        {
            s_bl.Volunteer.AddObserver(volunteerId, HandleVolunteerUpdate);
        }

        // Method to handle updates to the volunteer data (called by the observer)
        private void HandleVolunteerUpdate()
        {
            try
            {
                // Reload data on the UI thread to ensure thread safety
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Fetch updated volunteer data
                    var updatedVolunteer = s_bl.Volunteer.Read(Volunteer.Id);

                    // Update volunteer properties
                    Volunteer.Name = updatedVolunteer.Name;
                    Volunteer.Number_phone = updatedVolunteer.Number_phone;
                    Volunteer.Email = updatedVolunteer.Email;
                    Volunteer.Active = updatedVolunteer.Active;
                    Volunteer.Distance = updatedVolunteer.Distance;
                    Volunteer.DistanceType = updatedVolunteer.DistanceType;
                    Volunteer.Role = updatedVolunteer.Role;
                });
            }
            catch (Exception ex)
            {
                // Handle error when updating the volunteer
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error updating volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        // Called when the window is closed to remove the observer and prevent further updates
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Volunteer != null && Volunteer.Id != 0)
            {
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, HandleVolunteerUpdate);
            }
        }

        // Button click event handler for adding or updating a volunteer
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Id == 0)
            {
                AddVolunteer();
            }
            else
            {
                UpdateVolunteer();
            }
        }

        // Method to add a new volunteer (asynchronous)
        private async void AddVolunteer()
        {
            try
            {
                var volunteer = Volunteer;
                await Task.Run(() => s_bl.Volunteer.Create(volunteer));

                // Show success message and close the window
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                // Display error message in case of failure
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        // Method to update an existing volunteer (asynchronous)
    private async void UpdateVolunteer()
{
    try
    {
        var volunteer = Volunteer;
        
        // קבלת המתנדב הנוכחי מה-BL
        var currentVolunteer = s_bl.Volunteer.Read(volunteer.Id);

        // השוואה בין הנתונים הנוכחיים לנתונים החדשים
        var updatedVolunteer = new BO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name != currentVolunteer.Name ? volunteer.Name : currentVolunteer.Name,
            Number_phone = volunteer.Number_phone != currentVolunteer.Number_phone ? volunteer.Number_phone : currentVolunteer.Number_phone,
            Email = volunteer.Email != currentVolunteer.Email ? volunteer.Email : currentVolunteer.Email,
            Active = volunteer.Active != currentVolunteer.Active ? volunteer.Active : currentVolunteer.Active,
            Distance = volunteer.Distance != currentVolunteer.Distance ? volunteer.Distance : currentVolunteer.Distance,
            DistanceType = volunteer.DistanceType != currentVolunteer.DistanceType ? volunteer.DistanceType : currentVolunteer.DistanceType,
            Role = volunteer.Role != currentVolunteer.Role ? volunteer.Role : currentVolunteer.Role,
            FullCurrentAddress = volunteer.FullCurrentAddress != currentVolunteer.FullCurrentAddress ? volunteer.FullCurrentAddress : currentVolunteer.FullCurrentAddress
        };

        // עדכון המתנדב ב-BL בצורה אסינכרונית
        await Task.Run(() =>
        {
            s_bl.Volunteer.Update(updatedVolunteer, volunteer.Id);
        });

        // הצגת הודעת הצלחה ב-UI thread אחרי סיום העדכון
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        });
    }
    catch (Exception ex)
    {
        // טיפול בשגיאה ב-UI thread
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        });
    }
}


        // PropertyChanged event to notify when a property changes
        public event PropertyChangedEventHandler? PropertyChanged;

        // Helper method to notify property change for data binding
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Event handler for DistanceType ComboBox selection change
        private void cbDistanceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.DistanceType selectedDistanceType)
            {
                Console.WriteLine($"Selected DistanceType: {selectedDistanceType}");
            }
        }

        // Event handler for Role ComboBox selection change
        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Role selectedRole)
            {
                Console.WriteLine($"Selected Role: {selectedRole}");
            }
        }
    }
}
