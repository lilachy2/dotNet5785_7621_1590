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
            VolInList = BlApi.Factory.Get().Volunteer.ReadAll(null, null).Cast<BO.VolInList>();


        }
        // תכונת תלות מסוג IEnumerable<BO.VolInList>
        public IEnumerable<BO.VolInList> VolInList
        {
            get { return (IEnumerable<BO.VolInList>)GetValue(VolInListProperty); }
            set { SetValue(VolInListProperty, value); }
        }

        public static readonly DependencyProperty VolInListProperty =
            DependencyProperty.Register(
                "VolInList",
                typeof(IEnumerable<BO.VolInList>),
                typeof(VolunteerListWindow),
                new PropertyMetadata(null));
    }










}

    

    //public IEnumerable<BO.VolInList> VolInList
    //{
    //    get { return (IEnumerable<BO.VolInList>)GetValue(VolInListProperty); }
    //    set { SetValue(VolInListProperty, value); }
    //}

    //public static readonly DependencyProperty VolInListListProperty =
    //    DependencyProperty.Register("CourseList", typeof(IEnumerable<BO.VolInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

