using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using PL.Volunteer; // Make sure this is the correct namespace for your business logic

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        // Dependency Property for button text (Add/Update)
        public string ButtonText { get; set; }

        // The ID of the entity, which determines whether the screen is for adding or updating
        public int Id { get; set; }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Reference to the BL layer

        // Dependency Property to bind to Volunteer data
        public BO.Volunteer? Volunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // The DependencyProperty that will hold the Volunteer data (to be used in XAML bindings)
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        // ObservableCollection for Roles (enum values for Role)
        public ObservableCollection<BO.Role> Roles { get; set; } = new ObservableCollection<BO.Role>();

        public VolunteerWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update"; // Set button text based on whether we are adding or updating

            InitializeComponent(); // Initialize the XAML components
            DataContext = this; // Bind the window's DataContext to this object

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
            }
            catch (Exception ex)
            {
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
            MessageBox.Show("Volunteer added! - To implement");
        }

        // Method for updating an existing Volunteer
        private void UpdateVolunteer()
        {
            MessageBox.Show("Volunteer updated! - To implement");
        }
    }
}
