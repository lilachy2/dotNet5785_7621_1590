using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using PL.Volunteer; // Make sure this is the correct namespace for your business logic

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
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
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // The DependencyProperty that will hold the Volunteer data (to be used in XAML bindings)
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null, OnVolunteerChanged));

        // ObservableCollection for Roles (enum values for Role)
        public ObservableCollection<BO.Role> Roles { get; set; } = new ObservableCollection<BO.Role>();

        // ObservableCollection for Distance Types (enum values for DistanceType)
        public ObservableCollection<BO.DistanceType> DistanceTypes { get; set; } = new ObservableCollection<BO.DistanceType>();

        public VolunteerWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update"; // Set button text based on whether we are adding or updating
            this.DataContext = this;

            InitializeComponent(); // Initialize the XAML components
            // DataContext is already set in XAML via {Binding RelativeSource={RelativeSource Self}}

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

                // Initialize the Roles collection from the enum values
                Roles = new ObservableCollection<BO.Role>(Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>());

                // Initialize the DistanceTypes collection from the enum values
                DistanceTypes = new ObservableCollection<BO.DistanceType>(Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>());
                Console.WriteLine($"DistanceTypes: {string.Join(", ", DistanceTypes)}");

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
                this.DataContext = Volunteer;
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

        // Method for adding a new Volunteer
        private void AddVolunteer()
        {
            try
            {
                // Call to BL for adding the volunteer
                s_bl.Volunteer.Create(Volunteer); // Assuming Create method exists in the BL

                // Show success message and close the window
                MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the operation
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method for updating an existing Volunteer
        private void UpdateVolunteer()
        {
            try
            {
                // Call to BL for updating the volunteer
                s_bl.Volunteer.Update(Volunteer, Volunteer.Id); // Assuming Update method exists in the BL

                // Show success message and close the window
                MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during the operation
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Implement selection change logic if needed
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
    }
}
