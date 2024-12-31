using Helpers;
using System;
using System.ComponentModel;

namespace BO
{
    public class Volunteer : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _numberPhone;
        private string _email;
        private string? _fullCurrentAddress;
        private string? _password;
        private double? _latitude;
        private double? _longitude;
        private Role _role;
        private bool _active;
        private double? _distance;
        private DistanceType _distanceType;
        private int _totalHandledCalls;
        private int _totalCancelledCalls;
        private int _totalExpiredCalls;
        private CallInProgress? _currentCall;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Number_phone
        {
            get => _numberPhone;
            set
            {
                if (_numberPhone != value)
                {
                    _numberPhone = value;
                    OnPropertyChanged(nameof(Number_phone));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string? FullCurrentAddress
        {
            get => _fullCurrentAddress;
            set
            {
                if (_fullCurrentAddress != value)
                {
                    _fullCurrentAddress = value;
                    OnPropertyChanged(nameof(FullCurrentAddress));
                }
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public double? Latitude
        {
            get => _latitude;
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    OnPropertyChanged(nameof(Latitude));
                }
            }
        }

        public double? Longitude
        {
            get => _longitude;
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    OnPropertyChanged(nameof(Longitude));
                }
            }
        }

        public Role Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged(nameof(Role));
                }
            }
        }

        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    OnPropertyChanged(nameof(Active));
                }
            }
        }

        public double? Distance
        {
            get => _distance;
            set
            {
                if (_distance != value)
                {
                    _distance = value;
                    OnPropertyChanged(nameof(Distance));
                }
            }
        }

        public DistanceType DistanceType
        {
            get => _distanceType;
            set
            {
                if (_distanceType != value)
                {
                    _distanceType = value;
                    OnPropertyChanged(nameof(DistanceType));
                }
            }
        }

        public int TotalHandledCalls
        {
            get => _totalHandledCalls;
            set
            {
                if (_totalHandledCalls != value)
                {
                    _totalHandledCalls = value;
                    OnPropertyChanged(nameof(TotalHandledCalls));
                }
            }
        }

        public int TotalCancelledCalls
        {
            get => _totalCancelledCalls;
            set
            {
                if (_totalCancelledCalls != value)
                {
                    _totalCancelledCalls = value;
                    OnPropertyChanged(nameof(TotalCancelledCalls));
                }
            }
        }

        public int TotalExpiredCalls
        {
            get => _totalExpiredCalls;
            set
            {
                if (_totalExpiredCalls != value)
                {
                    _totalExpiredCalls = value;
                    OnPropertyChanged(nameof(TotalExpiredCalls));
                }
            }
        }

        public CallInProgress? CurrentCall
        {
            get => _currentCall;
            set
            {
                if (_currentCall != value)
                {
                    _currentCall = value;
                    OnPropertyChanged(nameof(CurrentCall));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => this.ToStringProperty();
    }
}
