﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BO;
using BlApi;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.main_volunteer
{
    /// <summary>
    ///  תיקנו פה אובזרברים- לבדוק
    /// </summary>
    
    public partial class ChooseCallWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int _volunteerId;
        private BO.Volunteer _volunteer;
        private IEnumerable<BO.OpenCallInList> _openCallList;
        private BO.OpenCallInList _selectedCall;  // This will hold the selected call
        private BO.Calltype _selectedFilter = BO.Calltype.None;  // Default to None (no filter)
        private BO.OpenCallInListEnum _selectedSort = BO.OpenCallInListEnum.None; // Default to None (no sorting)

        private volatile DispatcherOperation? _observerOperation = null; //stage 7


        public event PropertyChangedEventHandler PropertyChanged;

        // DependencyProperty for OpenCallList
        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(ChooseCallWindow), new PropertyMetadata(null));

        // DependencyProperty for SelectedCall
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(BO.OpenCallInList), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get => (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty);
            set => SetValue(OpenCallListProperty, value);
        }

        public BO.OpenCallInList SelectedCall
        {
            get => _selectedCall;
            set
            {
                if (_selectedCall != value)
                {
                    _selectedCall = value;
                    OnPropertyChanged(nameof(SelectedCall));
                }
            }
        }
        public BO.Calltype SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));  // Notify the UI of the property change
                    LoadOpenCallsObserve();  // Update the list when the filter changes
                }
            }
        } 
        public BO.OpenCallInListEnum SelectedSort
        {
            get { return _selectedSort; }
            set
            {
                if (_selectedSort != value)
                {
                    _selectedSort = value;
                    OnPropertyChanged(nameof(_selectedSort));  // Notify the UI of the property change
                    LoadOpenCallsObserve();  // Update the list when the filter changes
                }
            }
        }
        public ChooseCallWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            LoadVolunteerData();
            this.DataContext = this;  // Set the DataContext to this window for binding
        }

        // Property for Volunteer with INotifyPropertyChanged
        public BO.Volunteer Volunteer
        {
            get => _volunteer;
            set
            {
                if (_volunteer != value)
                {
                    _volunteer = value;
                    OnPropertyChanged(nameof(Volunteer));
                    LoadOpenCallsObserve(); // Reload open calls when volunteer changes
                }
            }
        }

        // Method to load volunteer data
        private void LoadVolunteerData()
        {
            try
            {
                Volunteer = s_bl.Volunteer.Read(_volunteerId);  // Get volunteer data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading volunteer data: {ex.Message}");
            }
        }

        // Method to load open calls for the volunteer
        public void LoadOpenCallsObserve()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() => { LoadOpenCalls(); });
        }
        private void LoadOpenCalls()
        {
            if (Volunteer != null)
            {
                var calls = s_bl.Call.GetOpenCall(_volunteerId, SelectedFilter, SelectedSort);  // Get open calls
                OpenCallList = calls.ToList();  // Bind the list to the OpenCallList DependencyProperty

            }
        }

        // Method to handle the Select button click
        public void SelectCall(BO.OpenCallInList selectedCall)
        {
            // Set the SelectedCall to the clicked call
            SelectedCall = selectedCall;

        }

        // Method to notify property change
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // בודקים אם נבחר פריט (קריאה)
            var selectedCall = (OpenCallInList)((ListBox)sender).SelectedItem;

            // אם יש פריט שנבחר
            if (selectedCall != null)
            {
                // פותחים את החלון עם תיאור הקריאה
                var callDescriptionWindow = new CallDescriptionWindow(selectedCall);
                callDescriptionWindow.Show();
            }
        }

        private void FilterCallListByCriteria(object _, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Calltype selectedItem)
            {
                SelectedFilter = selectedItem;
                LoadOpenCallsObserve();

            }
        }
        private void SortCallListByCriteria(object _, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.OpenCallInListEnum selectedItem)
            {
                SelectedSort = selectedItem;
                LoadOpenCallsObserve();
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            Button button = sender as Button;

            // Get the DataContext of the button (which is the OpenCallInList object)
            var selectedCall = button?.DataContext as OpenCallInList;

            // Ensure a valid selected call exists
            if (selectedCall != null)
            {
                try
                {
                    // Call the ChooseCall method
                    s_bl.Call.ChooseCall(Volunteer.Id, selectedCall.Id);
                    queryCall();
                    // Notify the user that the call was successfully selected
                    MessageBox.Show($"Successfully selected call with ID: {selectedCall.Id} for treatment.");
                  this.Close();//observers need to cheak

                }
                catch (BO.BlCallStatusNotOKException ex)
                {
                    // Handle invalid call status exceptions
                    MessageBox.Show($"Error: {ex.Message}", "Invalid Status", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.Incompatible_ID ex)
                {
                    // Handle invalid ID exceptions
                    MessageBox.Show($"Error: {ex.Message}", "Invalid ID", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    // Handle any other unexpected errors
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register the observer to update the volunteer list when changes occur in the BL
            s_bl.Call.AddObserver(queryCall);
        }

        //Handle Window closed event to remove the observer
        private void Window_Closed(object sender, EventArgs e)
        {
            // Unregister the observer when the window is closed
            s_bl.Call.RemoveObserver(queryCall);

        }
        public void queryCall()
        {
            OpenCallList = (_selectedFilter == BO.Calltype.None) ?
                s_bl?.Call.GetOpenCall(_volunteerId, null, null)! :
                s_bl?.Call.GetOpenCall(_volunteerId, _selectedFilter, _selectedSort)!;
        }

    }
}
