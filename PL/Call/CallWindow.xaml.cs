using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        // טקסט הכפתור (הוספה/עדכון)
        public string ButtonText { get; set; }

        // מזהה מתנדב
        public int Id { get; set; }

        // הנתונים הנוכחיים
        public BO.Call call
        {
            get
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (BO.Call)GetValue(CurrentCallProperty);
                }
                else
                {
                    return (BO.Call)Application.Current.Dispatcher.Invoke(() => GetValue(CurrentCallProperty));
                }
            }

            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(call));
            }
        }

        public static readonly DependencyProperty CurrentCallProperty =
       DependencyProperty.Register("Call", typeof(BO.Call), typeof(CallWindow),
           new PropertyMetadata(null, OnCallChanged));

        public bool IsReadOnlyFields => Id != 0; // true אם זה עדכון, false אם זה הוספה


        public CallWindow(int? id = 0)
        {
            Id = id ?? 0;
            ButtonText = Id == 0 ? "Add" : "Update";
            DataContext = this;
            try
            {
                call = (Id != 0)
                    ? s_bl.Call.Read(Id)
                    : new BO.Call()
                    {
                        Id = 0,
                        Calltype = BO.Calltype.None,
                        Description = string.Empty,
                        FullAddress = string.Empty,
                        Latitude = 0.0,
                        Longitude = 0.0,
                        OpenTime = DateTime.Now,
                        MaxEndTime = null,
                        Status = BO.CallStatus.Open,
                        CallAssignments = null
                    };

                // הרשמה למשקיפים אם קיים ID
                if (call != null && call.Id != 0)
                {
                    SubscribeToCallUpdates(call.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            InitializeComponent();
        }

        private static void OnCallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (CallWindow)d;
            var newcall = (BO.Call)e.NewValue;

            if (newcall != null && newcall.Id != 0)
            {
                window.SubscribeToCallUpdates(newcall.Id);
            }
        }

        private void SubscribeToCallUpdates(int callId)
        {
            s_bl.Call.AddObserver(callId, HandleCallUpdate);
        }

        private void HandleCallUpdate()
        {
            try
            {
                // טעינה מחדש של הנתונים על ה-UI thread
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    // קריאת הקריאה המעודכנת
                //    var updatedCall = s_bl.Call.Read(call.Id);
                //    call = updatedCall;

                //    // עדכון הנתונים בשדות המתאימים
                //    call.Calltype = updatedCall.Calltype;
                //    call.Description = updatedCall.Description;
                //    call.FullAddress = updatedCall.FullAddress;
                //    call.Latitude = updatedCall.Latitude;
                //    call.Longitude = updatedCall.Longitude;
                //    call.OpenTime = updatedCall.OpenTime;
                //    call.MaxEndTime = updatedCall.MaxEndTime;
                //    call.Status = updatedCall.Status;
                //    call.CallAssignments = updatedCall.CallAssignments;
                //});
                OnPropertyChanged(nameof(call));

            }
            catch (Exception ex)
            {
                // טיפול בשגיאה ב-UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error updating call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (call != null && call.Id != 0)
            {
                s_bl.Call.RemoveObserver(call.Id, HandleCallUpdate);
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Id == 0)
                {
                    // במידה ומדובר במתנדב חדש, נקרא ל- Create ב- BL.
                    var call1 = call;

                    // קריאה לפונקציה Create ב- BL שתקרא לפונקציה Create ב- DAL
                    s_bl.Call.Create(call1);

                    // הצגת הודעת הצלחה
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    });
                }
                else
                {
                    // במקרה של עדכון מתנדב קיים
                    s_bl.Call.Update(call);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("call Update successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    });
                }
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        //void refresh()
        //{
        //    s_bl.Call.(null, null);
        //}

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void cbDistanceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.DistanceType selectedDistanceType)
            {
                Console.WriteLine($"Selected DistanceType: {selectedDistanceType}");
            }
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Role selectedRole)
            {
                Console.WriteLine($"Selected Role: {selectedRole}");
            }
        }
    }
}
