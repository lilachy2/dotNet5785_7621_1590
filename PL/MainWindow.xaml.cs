using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // stage 5

        // Dependency Property for the Current Time
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        // Dependency Property for Risk Range (TimeSpan)
        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow),
                new PropertyMetadata(TimeSpan.FromHours(1), OnRiskRangeChanged));

        // Event handler when the RiskRange property changes
        private static void OnRiskRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainWindow window && e.NewValue is TimeSpan newRange)
            {
                s_bl.Admin.SetMaxRange(newRange); // Update Risk Range via business logic
            }
        }

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Set DataContext for Binding
            this.Loaded += MainWindow_Loaded; // Register Loaded event
        }

        // Button click handlers to manipulate the system clock
        private void AddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute); // Add one minute
        }

        private void AddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour); // Add one hour
        }

        private void AddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day); // Add one day
        }

        private void AddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month); // Add one month
        }

        private void AddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year); // Add one year
        }

        // Update RiskRange when the "Update Risk Range" button is clicked
        private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Risk Range updated to: {RiskRange}"); // Show confirmation message
            s_bl.Admin.SetMaxRange(RiskRange); // Update the Risk Range in business logic
        }



        private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
        {
            // Message to the user about starting the initialization
            var result = MessageBox.Show("Are you sure you want to initialize the database?",
                                          "Database Initialization",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Change the mouse cursor to an hourglass
                Mouse.OverrideCursor = Cursors.AppStarting;

                try
                {
                    // Close all open windows except for the main window
                    var openWindows = Application.Current.Windows.OfType<Window>().Where(w => w != this).ToList();
                    foreach (var window in openWindows)
                    {
                        window.Close();
                    }

                    // Call the method to initialize the database
                    s_bl.Admin.InitializeDB();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // Restore the mouse cursor to normal after completing the action
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private async void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            // Message to the user asking if they are sure they want to reset the database
            var result = MessageBox.Show("Are you sure you want to reset the database?",
                                          "Confirm Reset",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            // If the user clicks "Yes", proceed with the action
            if (result == MessageBoxResult.Yes)
            {
                // Change the cursor to a waiting hourglass
                Mouse.OverrideCursor = Cursors.Wait;

                // Close all open windows except for the main window
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this) // Don't close the main window
                    {
                        window.Close();
                    }
                }

                // Call the function to reset the database
                await Task.Run(() => s_bl.Admin.ResetDB());

                // End the action, restore the normal cursor
                Mouse.OverrideCursor = Cursors.Arrow;

                // Message to the user indicating the reset was successful
                MessageBox.Show("Database has been reset successfully!");
            }
        }

        private void HandleCalls_Click(object sender, RoutedEventArgs e)
        {
            // Logic to manage calls, implement as needed
            MessageBox.Show("Handling Calls...");
        }

        private void HandleVolunteers_Click(object sender, RoutedEventArgs e)
        {
            // Logic to manage volunteers, implement as needed
            MessageBox.Show("Handling Volunteers...");
            new VolunteerListWindow().Show();
        }

        private void StartSimulator_Click(object sender, RoutedEventArgs e)
        {
            // Logic to start the simulator, implement as needed
            MessageBox.Show("Starting Simulator...");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Additional actions if needed (not defined yet)
        }

        // Observer method to update CurrentTime when it changes
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock(); // Update the CurrentTime property
        }

        // Observer method to update RiskRange when it changes
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetMaxRange(); // Update the RiskRange property
        }

        // MainWindow Loaded event handler
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the CurrentTime and RiskRange properties when the window is loaded
            CurrentTime = s_bl.Admin.GetClock(); // Get initial system time
            RiskRange = s_bl.Admin.GetMaxRange(); // Get initial Risk Range

            // Register observers for clock and configuration changes
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        // Window Closed event handler to clean up observers
        private void Window_Closed(object sender, EventArgs e)
        {
            // Remove observers when the window is closed
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }
    }
}
