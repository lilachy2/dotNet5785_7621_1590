using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading.Tasks;
using DO;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        // תלות עם BL
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        // טקסט הכפתור (הוספה/עדכון)
        public string ButtonText { get; set; }

        // מזהה מתנדב
        public int Id { get; set; }

        // הנתונים הנוכחיים
        public BO.Volunteer Volunteer
        {
            //get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            get
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    // אם אנחנו כבר ב-UI thread, ניתן לגשת ישירות
                    return (BO.Volunteer)GetValue(CurrentVolunteerProperty);
                }
                else
                {
                    // אם אנחנו לא ב-UI thread, נעשה קריאה ל-UI thread דרך ה-Dispatcher
                    return (BO.Volunteer)Application.Current.Dispatcher.Invoke(() => GetValue(CurrentVolunteerProperty));
                }
            }

            set
            {
                SetValue(CurrentVolunteerProperty, value);
                OnPropertyChanged(nameof(Volunteer));
            }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow),
                new PropertyMetadata(null, OnVolunteerChanged));

        public IEnumerable<BO.DistanceType> DistanceTypes =>
            Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

        public IEnumerable<BO.Role> Roles =>
            Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();

        public VolunteerWindow(int id = 0)
        {
            Id = id;
            ButtonText = Id == 0 ? "Add" : "Update";
            DataContext = this;
            try
            {
                Volunteer = (Id != 0)
                    ? s_bl.Volunteer.Read(Id)
                    : new BO.Volunteer()
                    {
                        Id = 0,
                        Name = string.Empty,
                        Number_phone = string.Empty,
                        Email = string.Empty,
                        FullCurrentAddress = null,
                        Password = null,
                        Latitude = null,
                        Longitude = null,
                        Role = BO.Role.Volunteer,
                        Active = false,
                        Distance = null,
                        DistanceType = BO.DistanceType.Aerial_distance,
                        TotalHandledCalls = 0,
                        TotalCancelledCalls = 0,
                        TotalExpiredCalls = 0,
                        CurrentCall = null
                    };

                // הרשמה למשקיפים אם קיים ID
                if (Volunteer != null && Volunteer.Id != 0)
                {
                    SubscribeToVolunteerUpdates(Volunteer.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            InitializeComponent();

        }

        private static void OnVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (VolunteerWindow)d;
            var newVolunteer = (BO.Volunteer)e.NewValue;

            if (newVolunteer != null && newVolunteer.Id != 0)
            {
                window.SubscribeToVolunteerUpdates(newVolunteer.Id);
            }
        }

        private void SubscribeToVolunteerUpdates(int volunteerId)
        {
            s_bl.Volunteer.AddObserver(volunteerId, HandleVolunteerUpdate);
        }
        private void HandleVolunteerUpdate()
        {
            try
            {
                // טעינה מחדש של הנתונים על ה-UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // קריאת המתנדב המעודכן
                    var updatedVolunteer = s_bl.Volunteer.Read(Volunteer.Id);
                    Volunteer = updatedVolunteer;
                    // עדכון הנתונים ב-Volunteer
                    Volunteer.Name = updatedVolunteer.Name;
                    Volunteer.Number_phone = updatedVolunteer.Number_phone;
                    Volunteer.Email = updatedVolunteer.Email;
                    Volunteer.Active = updatedVolunteer.Active;
                    Volunteer.Distance = updatedVolunteer.Distance;
                    Volunteer.DistanceType = updatedVolunteer.DistanceType;
                    Volunteer.Role = updatedVolunteer.Role;
                });
            }
            catch (Exception ex)
            {
                // טיפול בשגיאה ב-UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error updating volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            if (Volunteer != null && Volunteer.Id != 0)
            {
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, HandleVolunteerUpdate);
            }
        }




        private async void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Id == 0)
                {
                    // במידה ומדובר במתנדב חדש, נקרא ל- Create ב- BL.
                    var volunteer = Volunteer;

                    // קריאה לפונקציה Create ב- BL שתקרא לפונקציה Create ב- DAL
                    await Task.Run(() => s_bl.Volunteer.Create(volunteer));

                    // הצגת הודעת הצלחה
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    });
                }
                else
                {
                    // במקרה של עדכון מתנדב קיים
                    await Task.Run(() => s_bl.Volunteer.Update(Volunteer, Volunteer.Id));

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MessageBox.Show("Volunteer Update successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    });
                }
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async void AddVolunteer()
        {
            try
            {
                var volunteer = Volunteer;
                await Task.Run(() => s_bl.Volunteer.Create(volunteer));

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }

        }

        //private async void UpdateVolunteer()
        //{
        //    try
        //    {
        //        var volunteer = Volunteer;
        //        await Task.Run(() => s_bl.Volunteer.Update(volunteer, volunteer.Id));

        //        await Application.Current.Dispatcher.InvokeAsync(() =>
        //        {
        //            MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //            this.Close();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        await Application.Current.Dispatcher.InvokeAsync(() =>
        //        {
        //            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        });
        //    }
        //}

        private async void UpdateVolunteer1()
        {
            try
            {
                var volunteer = Volunteer;

                // עדכון המתנדב ב-BL בצורה אסינכרונית
                await Task.Run(() =>
                {
                    s_bl.Volunteer.Update(volunteer, volunteer.Id);
                    //s_bl.Volunteer.Read(volunteer.Id);

                });


                // הצגת הודעת הצלחה ב-UI thread אחרי סיום העדכון
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();

                });
            }
            catch (Exception ex)
            {
                // טיפול בשגיאה ב-UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }
        private async void UpdateVolunteer2()
        {
            try
            {
                var volunteer = Volunteer;

                // עדכון המתנדב ב-BL בצורה אסינכרונית
                await Task.Run(() =>
                {
                    s_bl.Volunteer.Update(volunteer, volunteer.Id);
                });

                // הצגת הודעת הצלחה ב-UI thread אחרי סיום העדכון
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    refresh();

                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // כאן אתה יכול לעדכן את ה-Volunteer על ה-UI
                    //Volunteer = s_bl.Volunteer.Read(volunteer.Id); // טעינה מחדש של המתנדב
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                // טיפול בשגיאה ב-UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }


        private async void UpdateVolunteer()
        {
            try
            {
                var volunteer = Volunteer;
                await Task.Run(() => s_bl.Volunteer.Update(volunteer, volunteer.Id));

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Volunteer Update successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }

        }

        void refresh()
        {
            s_bl.Volunteer.ReadAll(null, null);

        }

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