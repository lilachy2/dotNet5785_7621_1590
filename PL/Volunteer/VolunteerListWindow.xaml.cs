using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        // Declare the SelectedFilter property at the class level
        public VolInList SelectedFilter { get; set; } = VolInList.None; // Default to "None" for no filter

        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolInListProperty); }
            set { SetValue(VolInListProperty, value); }
        }

        public static readonly DependencyProperty VolInListProperty =
            DependencyProperty.Register(
                "VolunteerInList",
                typeof(IEnumerable<BO.VolunteerInList>),
                typeof(VolunteerListWindow),
                new PropertyMetadata(null));

        public VolunteerListWindow()
        {
            InitializeComponent();
            DataContext = this; // Set the DataContext to the window itself to bind properties

            // Initialize the ComboBox with Enum values
            FilterComboBox.ItemsSource = Enum.GetValues(typeof(VolInList));

            // Initially load the volunteer list (without filter)
            VolunteerInList = BlApi.Factory.Get().Volunteer.ReadAll(null, null);
        }

        // Method to handle ComboBox selection change for filtering
        private void FilterVolunteerlistByCriteria(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected value from ComboBox and update SelectedFilter accordingly
            var selectedItem = FilterComboBox.SelectedItem as VolInList?;
            if (selectedItem.HasValue)
            {
                SelectedFilter = selectedItem.Value;
                UpdateVolunteerList();
            }
        }

        // Update the volunteer list based on the selected filter criteria
        private void UpdateVolunteerList()
        {
            IEnumerable<BO.VolunteerInList> volunteers;

            // Apply filtering based on the selected filter
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

            // Update the VolunteerInList with the filtered volunteers
            VolunteerInList = volunteers;
        }

        // Handle Window loaded event to register the observer for updates
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register the observer to update the volunteer list when changes occur in the BL
            BlApi.Factory.Get().Volunteer.AddObserver(UpdateVolunteerList);

            // Load the volunteer list initially (without filter)
            UpdateVolunteerList();
        }

        // Handle Window closed event to remove the observer
        private void Window_Closed(object sender, EventArgs e)
        {
            // Unregister the observer when the window is closed
            BlApi.Factory.Get().Volunteer.RemoveObserver(UpdateVolunteerList);
        }
    }
}
