using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        public BO.Calltype Calltype { get; set; } = BO.Calltype.None; // 8 b
        public VolunteerListWindow()
        {
            InitializeComponent();
            DataContext = this; // קביעת הקשר נתונים למסך
                                // טוען את רשימת המתנדבים
            VolInList = BlApi.Factory.Get().Volunteer.ReadAll(null, null);


        }
        public IEnumerable<BO.VolunteerInList> VolInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolInListProperty); }
            set { SetValue(VolInListProperty, value); }
        }

        public static readonly DependencyProperty VolInListProperty =
            DependencyProperty.Register(
                "VolInList",
                typeof(IEnumerable<BO.VolunteerInList>),
                typeof(VolunteerListWindow),
                new PropertyMetadata(null));


        private void FilterVolunteerlistByCalltype(object sender, SelectionChangedEventArgs e)
        {
            ///אפשר להוסיף בCOMBOBOX סינון גם לפי שם , ID וכו 
            // סינון הרשימה לפי הערך הנבחר ב-ComboBox
            VolInList = (Calltype == BO.Calltype.None)
                ? BlApi.Factory.Get().Volunteer.ReadAll(null, null)
                : BlApi.Factory.Get().Volunteer.ReadAll(true, null,Calltype);
        }


        // מתודת השקפה שתשמור את הרשימה מעודכנת
        private void UpdateVolunteerList()
        {
            // אם יש שדות סינון (כמו Calltype), נעביר את זה לפונקציה
            VolInList = (Calltype == BO.Calltype.None)
                ? BlApi.Factory.Get().Volunteer.ReadAll(null, null)
                : BlApi.Factory.Get().Volunteer.ReadAll(true, null, Calltype);
        }
        // נרשמים לאירוע של טעינת המסך
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // נרשמים לעדכון הרשימה כשיש שינוי ב-BL
            BlApi.Factory.Get().Volunteer.AddObserver(UpdateVolunteerList);

            // טוענים את רשימת המתנדבים בהתחלה
            UpdateVolunteerList();
        }

        // מסירים את המשקיף כשסוגרים את החלון
        private void Window_Closed(object sender, EventArgs e)
        {
            // מסירים את המשקיף מהעדכונים
            BlApi.Factory.Get().Volunteer.RemoveObserver(UpdateVolunteerList);
        }

    }

}
