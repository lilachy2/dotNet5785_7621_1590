using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.main_volunteer
{
    public partial class CallDescriptionWindow : Window
    {

        public CallDescriptionWindow(OpenCallInList selectedCall)
        {
            InitializeComponent();
            this.DataContext = selectedCall;    
            //string address = selectedCall.FullAddress;

            //// יצירת ה-URL של המפה
            //string mapUrl = $"https://www.google.com/maps?q={Uri.EscapeDataString(address)}";

            //// גישה לפקד WebBrowser שנמצא ב-DockPanel
            //var webBrowser = (this.Content as DockPanel).Children[2] as WebBrowser; // מניחים שהוא הפקד השלישי
            //webBrowser?.Navigate(mapUrl);
        }
    }
}
