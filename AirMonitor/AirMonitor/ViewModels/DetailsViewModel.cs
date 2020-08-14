using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace AirMonitor.ViewModels
{
    class DetailsViewModel : INotifyPropertyChanged
    {
        private int _CAQI = 56;
        private int _pm25 = 34;
        private int _pm25Percent = 137;
        private int _pm10 = 67;
        private int _pm10Percent = 135;
        private double _humidity = 0.95;
        private int _pressure = 1027;
        public event PropertyChangedEventHandler PropertyChanged;

        public DetailsViewModel() { }

        public int CAQI
        {
            get
            {
                return _CAQI;
            }
            set
            {
                _CAQI = value;
                OnPropertyChanged("CAQI");
            }
        }

        public int Pm25
        {
            get
            {
                return _pm25;
            }
            set
            {
                _pm25 = value;
                OnPropertyChanged("Pm25");
            }
        }


        public int Pm25Percent
        {
            get
            {
                return _pm25Percent;
            }
            set
            {
                _pm25Percent = value;
                OnPropertyChanged("Pm25Percent");
            }
        }


        public int Pm10
        {
            get
            {
                return _pm10;
            }
            set
            {
                _pm10 = value;
                OnPropertyChanged("Pm10");
            }
        }

        public int Pm10Percent
        {
            get
            {
                return _pm10Percent;
            }
            set
            {
                _pm10Percent = value;
                OnPropertyChanged("Pm10Percent");
            }
        }
        public double Humidity
        {
            get
            {
                return _humidity;
            }
            set
            {
                _humidity = value;
                OnPropertyChanged("Humidity");
            }
        }

        public int Pressure
        {
            get
            {
                return _pressure;
            }
            set
            {
                _pressure = value;
                OnPropertyChanged("Pressure");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}