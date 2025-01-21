using BO;
using System;
using System.Windows;
using System.Windows.Controls;

//namespace PL.main_volunteer
//{
//    public partial class CallDescriptionWindow : Window
//    {

//        public CallDescriptionWindow(OpenCallInList selectedCall)
//        {
//            InitializeComponent();
//            this.DataContext = selectedCall;    
//            //string address = selectedCall.FullAddress;

//            //// יצירת ה-URL של המפה
//            //string mapUrl = $"https://www.google.com/maps?q={Uri.EscapeDataString(address)}";

//            //// גישה לפקד WebBrowser שנמצא ב-DockPanel
//            //var webBrowser = (this.Content as DockPanel).Children[2] as WebBrowser; // מניחים שהוא הפקד השלישי
//            //webBrowser?.Navigate(mapUrl);
//        }
//    }
//}

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_selectedCall?.FullAddress))
                {
                    string address = _selectedCall.FullAddress;
                    string mapUrl = $"https://www.openstreetmap.org/search?query={Uri.EscapeDataString(address)}";

                    WebBrowser browser = ((DockPanel)Content).Children[2] as WebBrowser;
                    if (browser != null)
                    {
                        browser.Navigate(new Uri(mapUrl));
                    }
                }
                else
                {
                    MessageBox.Show("No address available for this call.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading map: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}