using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.main_volunteer
{
    public partial class CallDescriptionWindow : Window
    {
        private OpenCallInList _selectedCall;

        public CallDescriptionWindow(OpenCallInList selectedCall)
        {
            InitializeComponent();
            _selectedCall = selectedCall;
            this.DataContext = selectedCall;
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        // בדיקה אם הכתובת קיימת והצגת מפה (מושבתת כרגע)
        //        /*
        //        if (!string.IsNullOrEmpty(_selectedCall?.FullAddress))
        //        {
        //            string address = _selectedCall.FullAddress;
        //            string mapUrl = $"https://www.openstreetmap.org/search?query={Uri.EscapeDataString(address)}";

        //            WebBrowser browser = ((DockPanel)Content).Children[2] as WebBrowser;
        //            if (browser != null)
        //            {
        //                browser.Navigate(new Uri(mapUrl));
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("No address available for this call.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        */
        //    }
        //    catch (Exception ex)
        //    {
        //        // טיפול בשגיאות (בהקשר של טעינת מפה - לא רלוונטי כרגע)
        //        /*
        //        MessageBox.Show($"Error loading map: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        */
        //    }
        //}
    }
}
