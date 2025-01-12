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

namespace PL.main_volunteer
{
    /// <summary>
    /// Interaction logic for CallDescriptionWindow.xaml
    /// </summary>
    public partial class CallDescriptionWindow : Window
    {
        public CallDescriptionWindow(OpenCallInList selectedCall)
        {
            InitializeComponent();
            this.DataContext = selectedCall;
        }
    }
}
