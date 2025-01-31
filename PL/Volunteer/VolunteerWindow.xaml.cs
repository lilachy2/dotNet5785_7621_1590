using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using DO;
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _updateOperation = null; // מעקב אחר עדכונים

        // טקסט הכפתור (הוספה/עדכון)
        public string ButtonText { get; set; }

        // מזהה מתנדב
        public int Id { get; set; }

        // הנתונים הנוכחיים
        public BO.Volunteer Volunteer
        {
            get
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (BO.Volunteer)GetValue(CurrentVolunteerProperty);
                }
                else
                {
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
                new PropertyMetadata(null));


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

                //// הרשמה למשקיפים אם קיים ID
                //if (Volunteer != null && Volunteer.Id != 0)
                //{
                //    SubscribeToVolunteerUpdates(Volunteer.Id);
                //}
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
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Volunteer != null && Volunteer.Id != 0)
            {
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, HandleVolunteerUpdate);
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Id == 0)
                {
                    // במידה ומדובר במתנדב חדש, נקרא ל- Create ב- BL.
                    var volunteer = Volunteer;

                    // קריאה לפונקציה Create ב- BL שתקרא לפונקציה Create ב- DAL
                    s_bl.Volunteer.Create(volunteer);

                    // הצגת הודעת הצלחה
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    });
                }
                else
                {
                    // במקרה של עדכון מתנדב קיים
                    s_bl.Volunteer.Update(Volunteer, Volunteer.Id);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Volunteer Update successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    });
                }
            }
            //catch (Exception ex)
            //{
            //    // טיפול בשגיאות
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    });
            //}
            catch (DO.DalDoesNotExistException ex)
            {
                MessageBox.Show($"Volunteer with ID {Volunteer.Id} does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.Incompatible_ID ex)
            {
                MessageBox.Show($"Volunteer with ID {Volunteer.Id} has an incompatible ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlInvalidaddress ex) // הודעה אסינכרונית
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Invalid address provided for volunteer with ID {Volunteer.Id}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            catch (BO.InvalidOperationException ex)
            {
                MessageBox.Show("Invalid operation attempted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlCheckIdException ex)
            {
                MessageBox.Show("Invalid ID format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlCheckPhonnumberException ex)
            {
                MessageBox.Show("Invalid phone number format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlEmailException ex)
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlIncorrectPasswordException ex)
            {
                MessageBox.Show("Incorrect password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlCan_chang_to_NotActivException ex)
            {
                MessageBox.Show("Cannot change status to 'Not Active'.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlWrongItemtException ex)
            {
                MessageBox.Show("There was an issue with the format or logic of the data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlPermissionException ex)
            {
                MessageBox.Show("Permission error occurred.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private void cbDistanceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.DistanceType selectedDistanceType)
        //    {
        //        Console.WriteLine($"Selected DistanceType: {selectedDistanceType}");
        //    }
        //}

        //private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count > 0 && e.AddedItems[0] is BO.Role selectedRole)
        //    {
        //        Console.WriteLine($"Selected Role: {selectedRole}");
        //    }
        //}


        private void HandleVolunteerUpdate()
        {
            if (_updateOperation is null || _updateOperation.Status == DispatcherOperationStatus.Completed)
            {
                _updateOperation = Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        // קריאת המתנדב המעודכן
                        //var updatedVolunteer 
                        Volunteer = s_bl.Volunteer.Read(Volunteer.Id);
                        //Volunteer = updatedVolunteer;

                        //// עדכון הנתונים
                        //Volunteer.Name = updatedVolunteer.Name;
                        //Volunteer.Number_phone = updatedVolunteer.Number_phone;
                        //Volunteer.Email = updatedVolunteer.Email;
                        //Volunteer.Active = updatedVolunteer.Active;
                        //Volunteer.Distance = updatedVolunteer.Distance;
                        //Volunteer.DistanceType = updatedVolunteer.DistanceType;
                        //Volunteer.Role = updatedVolunteer.Role;
                    }
                    catch (Exception ex)
                    {
                        // טיפול בשגיאה
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Error updating volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                });
            }
        }


    }
}
