﻿using PL.Volunteer;
using PL.login;
using PL.main_volunteer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PL.Call;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///testttt
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

        private int? volId; // to get volunterrID to oter Window

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        // Constructor
        public MainWindow(int? volId1)
        {
            InitializeComponent();
            DataContext = this; // Set DataContext for Binding
            this.Loaded += MainWindow_Loaded; // Register Loaded event
            this.volId = volId1;// to get volunterrID to oter Window

            CallStatusesCounts = new ObservableCollection<KeyValuePair<string, int>>();


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

        private async void InitializeDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?",
                                          "Database Initialization",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.AppStarting;  // Change the cursor to hourglass

                try
                {
                    var openWindows = Application.Current.Windows.OfType<Window>().Where(w => w != this).ToList();
                    foreach (var window in openWindows)
                    {
                        window.Close(); // Close all other windows
                    }

                    await Task.Run(() =>
                    {
                        s_bl.Admin.InitializeDB(); // Initialize the database
                    });

                    MessageBox.Show("The database has been successfully initialized.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;  // Restore the cursor to normal
                }
            }
        }

        private async void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the database?",
                                          "Confirm Reset",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;  // Change the cursor to wait

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                    {
                        window.Close(); // Close all other windows
                    }
                }

                try
                {
                    await Task.Run(() => s_bl.Admin.ResetDB()); // Reset the database

                    MessageBox.Show("The database has been successfully reset.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;  // Restore the cursor to normal
                }
            }
        }

        private void HandleCalls_Click(object sender, RoutedEventArgs e)
        {
            // Logic to manage calls, implement as needed
            //MessageBox.Show("Handling Calls...");
            new CallListWindow(volId).Show();

        }

        private void HandleVolunteers_Click(object sender, RoutedEventArgs e)
        {
            // Logic to manage volunteers, implement as needed
            //MessageBox.Show("Handling Volunteers...");
            new VolunteerListWindow().Show();
        }

        private void StartSimulator_Click(object sender, RoutedEventArgs e)
        {
            // Logic to start the simulator, implement as needed
            //MessageBox.Show("Starting Simulator...");
        }

        // Observer method to update CurrentTime when it changes
        private void clockObserver()
        {
            // Update the CurrentTime property on the UI thread
            Dispatcher.Invoke(() => CurrentTime = s_bl.Admin.GetClock()); // Ensure it's on the UI thread
        }


        // Observer method to update RiskRange when it changes
        private void configObserver()
        {
            // Update the RiskRange property on the UI thread
            Dispatcher.Invoke(() => RiskRange = s_bl.Admin.GetMaxRange()); // Ensure it's on the UI thread
        }

        // MainWindow Loaded event handler
        public ObservableCollection<KeyValuePair<string, int>> CallStatusesCounts { get; set; }
        //private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // Initialize the CurrentTime and RiskRange properties when the window is loaded
        //    CurrentTime = s_bl.Admin.GetClock(); // Get initial system time
        //    RiskRange = s_bl.Admin.GetMaxRange(); // Get initial Risk Range

        //    var counts = s_bl.Call.GetCallStatusesCounts();
        //    CallStatusesCounts.Clear();

        //    CallStatusesCounts.Add(new KeyValuePair<string, int>("Open", counts[0]));
        //    CallStatusesCounts.Add(new KeyValuePair<string, int>("Closed", counts[1]));
        //    CallStatusesCounts.Add(new KeyValuePair<string, int>("InProgress", counts[2]));
        //    CallStatusesCounts.Add(new KeyValuePair<string, int>("Expired", counts[3]));
        //    CallStatusesCounts.Add(new KeyValuePair<string, int>("InProgressAtRisk", counts[4]));
        //    CallStatusesCounts.Add(new KeyValuePair<string, int>("OpenAtRisk", counts[5]));
        //    CallStatusesCounts.Add(new KeyValuePair<string, int>("Total", counts[6]));

        //    // Register observers for clock and configuration changes
        //    s_bl.Admin.AddClockObserver(clockObserver);
        //    s_bl.Admin.AddConfigObserver(configObserver);
        //}


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentTime = s_bl.Admin.GetClock();
                RiskRange = s_bl.Admin.GetMaxRange();

                var counts = s_bl.Call.GetCallStatusesCounts();
                System.Diagnostics.Debug.WriteLine($"Loading counts: {counts?.Length ?? 0} items");

                CallStatusesCounts.Clear();
                if (counts != null)
                {
                    CallStatusesCounts.Add(new KeyValuePair<string, int>("Open", counts[0]));
                    CallStatusesCounts.Add(new KeyValuePair<string, int>("Closed", counts[1]));
                    CallStatusesCounts.Add(new KeyValuePair<string, int>("InProgress", counts[2]));
                    CallStatusesCounts.Add(new KeyValuePair<string, int>("Expired", counts[3]));
                    CallStatusesCounts.Add(new KeyValuePair<string, int>("InProgressAtRisk", counts[4]));
                    CallStatusesCounts.Add(new KeyValuePair<string, int>("OpenAtRisk", counts[5]));
                    CallStatusesCounts.Add(new KeyValuePair<string, int>("Total", counts[6]));

                    System.Diagnostics.Debug.WriteLine($"Added {CallStatusesCounts.Count} items to collection");
                }

                s_bl.Admin.AddClockObserver(clockObserver);
                s_bl.Admin.AddConfigObserver(configObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading window: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error: {ex}");
            }
        }



        // Window Closed event handler to clean up observers
        private void Window_Closed(object sender, EventArgs e)
        {
            // Remove observers when the window is closed
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        private void RiskRangeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void CallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is KeyValuePair<string, int> selectedItem)
            {
                // המרת המפתח לסוג הסטטוס (בהנחה שהמפתח מתאים ל-BO.Calltype)
                if (Enum.TryParse(typeof(BO.CallStatus), selectedItem.Key, out var status))
                {
                    var statusFilter = (BO.CallStatus)status;

                    // פתיחת חלון חדש עם הסינון
                    var callListWindow = new CallListWindow(null, statusFilter);
                    callListWindow.Show();
                }
                else
                {
                    MessageBox.Show("Invalid status type selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
