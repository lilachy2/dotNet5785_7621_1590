using BO;
using System;
using System.Windows;

namespace PL.main_volunteer
{
    public partial class CallDescriptionWindow : Window
    {
        private OpenCallInList _selectedCall;
        private const string MapboxApiKey = "pk.eyJ1IjoieWFsaW4zMjQyIiwiYSI6ImNtNmhtMDBkbzBkaXQyanNiZXJoZGg0MHUifQ.PS2aiQxHiPx_2JJs7ix8kw";

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
                    string address = Uri.EscapeDataString(_selectedCall.FullAddress);
                    string mapUrl = $"https://www.google.com/maps/search/?api=1&query={address}";

                    // אופציה 2 - שימוש ב-Mapbox (דורש חיפוש כתובת ידני)
                    string mapboxUrl = $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/{address},14,0/600x400?access_token={MapboxApiKey}";

                    MapBrowser.Navigating += (s, e) =>
                    {
                        dynamic activeX = MapBrowser.GetType().InvokeMember("ActiveXInstance",
                            System.Reflection.BindingFlags.GetProperty |
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic, null, MapBrowser, new object[] { });

                        activeX.Silent = true; // משתיק שגיאות סקריפט
                    };

                    // טוען את המפה
                    MapBrowser.Navigate(new Uri(mapUrl));
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
