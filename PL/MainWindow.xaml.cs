using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // stage 5

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        // תכונת תלות עבור RiskRange
        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow),
                new PropertyMetadata(TimeSpan.FromHours(1), OnRiskRangeChanged));

        private static void OnRiskRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainWindow window && e.NewValue is TimeSpan newRange)
            {
                s_bl.Admin.SetMaxRange(newRange); // עדכון הסיכון דרך הלוגיקה העסקית
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // קביעת ה-DataContext עבור Binding
            this.Loaded += MainWindow_Loaded;

        }

        private void AddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }

        private void AddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }
        private void AddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }
        private void AddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }
        private void AddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }

        private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Risk Range updated to: {RiskRange}");
            s_bl.Admin.SetMaxRange(RiskRange);

        }
       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // פעולה נוספת לפי הצורך
        }

        private void clockObserver()
        {
            // עדכון CurrentTime כאשר השעון משתנה
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void configObserver()
        {
            // עדכון RiskRange כאשר טווח הסיכון משתנה
            RiskRange = s_bl.Admin.GetMaxRange();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // השמה של הערך הנוכחי של שעון המערכת לתכונת התלות CurrentTime
            CurrentTime = s_bl.Admin.GetClock();

            // השמה של ערך משתנה התצורה RiskRange לתכונת התלות RiskRange
            RiskRange = s_bl.Admin.GetMaxRange();

            // רישום המשקיפים
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            // הסרת המשקיפים
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

    }


    

}
