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

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginSystem.xaml
    /// </summary>
    public partial class LoginSystem : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginSystem()
        {
            DataContext = this; 
            InitializeComponent();
        }

        // מזהה מתנדב
        public int Id { get; set; }
        public string Password { get; set; }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            if (Id == null || Password==null)
            {
                MessageBox.Show("Error:Please enter Full details.");
                return;
            }

            var volunteer = s_bl.Volunteer.Read(Id);
            if (volunteer == null)
            {
                MessageBox.Show("Error:Volunteer not registered in the system.");
                return;
            }

            if (volunteer.Role == BO.Role.Volunteer)
            {
                

            }
            else if (volunteer.Role == BO.Role.Manager)
            {


            }

        }






    }
}
