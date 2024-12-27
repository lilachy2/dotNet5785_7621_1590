using System;
using System.Collections.ObjectModel;
using System.Windows;
using PL.Volunteer; // שים לב שזו התייחסות לשכבת PL, יש להוסיף את השם המתאים למרחב שמכיל את הישויות שלך


namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        public string ButtonText { get; set; } // תכונת תלות עבור טקסט הכפתור
        public int Id { get; set; } // מזהה הישות

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // stage 5

      
        public BO.Volunteer? Volunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentCourseProperty); }
            set { SetValue(CurrentCourseProperty, value); }
        }

        public static readonly DependencyProperty CurrentCourseProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public VolunteerWindow(int id = 0)
        {
            Id = id; // Storing the value received in the id parameter to the Id variable
            ButtonText = Id == 0 ? "Add" : "Update"; // Setting the button text based on the status

            InitializeComponent(); // Initializing the XAML components

            DataContext = this; // Binding the window's DataContext to the current data

            // Creating or retrieving the object from the BL based on the Id
            try
            {
                // If Id is not 0, retrieve the data from the BL
                Volunteer = (Id != 0)
                    ? s_bl.Volunteer.Read(Id) // Calling the BL
                    : new BO.Volunteer() // Creating a new object with default values
                    {
                        Id = 0,
                        Name = string.Empty, // Default full name
                        Number_phone = string.Empty, // Default phone number
                        Email = string.Empty, // Default email address
                        FullCurrentAddress = null, // Full address (null initially)
                        Password = null, // Password (null initially)
                        Latitude = null, // Geographic coordinates (null initially)
                        Longitude = null,
                        Role = BO.Role.Volunteer, // Default role (volunteer)
                        Active = false, // Is the volunteer active (false initially)
                        Distance = null, // Distance (null initially)
                        DistanceType = BO.DistanceType.Aerial_distance, // Distance type (default: aerial)
                        TotalHandledCalls = 0, // Number of handled calls (0 initially)
                        TotalCancelledCalls = 0, // Number of cancelled calls (0 initially)
                        TotalExpiredCalls = 0, // Number of expired calls (0 initially)
                        CurrentCall = null // Current call (null initially)
                    };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Id == 0)
            {
                // הוספה
                AddVolunteer();
            }
            else
            {
                // עדכון
                UpdateVolunteer();
            }
        }

        private void AddVolunteer()
        {

            MessageBox.Show("Volunteer added!- להוסיף מימוש");
        }

        private void UpdateVolunteer()
        {
            MessageBox.Show("Volunteer updated!- להוסיף מימוש");
        }

// בתוך מחלקת VolunteerWindow
private ObservableCollection<BO.Role> _roles;
    public ObservableCollection<BO.Role> Roles
    {
        get => _roles;
        set => _roles = value;
    }




}
}
