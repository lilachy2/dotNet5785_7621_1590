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
        public VolunteerListWindow()
        {
            InitializeComponent();
            DataContext = this; // קביעת הקשר נתונים למסך
                                // טוען את רשימת המתנדבים
            VolInList = BlApi.Factory.Get().Volunteer.ReadAll(null, null);


        }
        // תכונת תלות מסוג IEnumerable<BO.VolunteerInList>
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
    }










}
