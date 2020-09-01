using AirMonitor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace AirMonitor.ViewModels
{
    class DetailsViewModel : BaseViewModel
    {
        private int _CAQI = 56;
        private int _pm25 = 34;
        private int _pm25Percent = 137;
        private int _pm10 = 67;
        private int _pm10Percent = 135;
        private double _humidityPercent = 29;
        private int _pressure = 1027;
        private string _caqiTitle = "Świetna jakość!";
        private string _caqiDescription = "Możesz bezpiecznie wyjść z domu bez swojej maski anty-smogowej i nie bać się o swoje zdrowie.";
        private Measurement _item;

        public DetailsViewModel() { }

        public int CAQI
        {
            get => _CAQI;
            set => SetProperty(ref _CAQI, value);
        }

        public string CaqiTitle
        {
            get => _caqiTitle;
            set => SetProperty(ref _caqiTitle, value);
        }

        public string CaqiDescription
        {
            get => _caqiDescription;
            set => SetProperty(ref _caqiDescription, value);
        }

        public int Pm25
        {
            get => _pm25;
            set => SetProperty(ref _pm25, value);
        }


        public int Pm25Percent
        {
            get => _pm25Percent;
            set => SetProperty(ref _pm25Percent, value);
        }

        public int Pm10
        {
            get => _pm10;
            set => SetProperty(ref _pm10, value);
        }

        public int Pm10Percent
        {
            get => _pm10Percent;
            set => SetProperty(ref _pm10Percent, value);
        }
        public double HumidityPercent
        {
            get => _humidityPercent;
            set => SetProperty(ref _humidityPercent, value);
        }

        public int Pressure
        {
                get => _pressure;
                set => SetProperty(ref _pressure, value);
        }

        public Measurement Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);

                UpdateProperties();
            }
        }

        private void UpdateProperties()
        {
            if (Item?.Current == null) return;
            var current = Item?.Current;
            var index = current.Indexes?.FirstOrDefault(c => c.Name == "AIRLY_CAQI");
            var values = current.Values;
            var standards = current.Standards;

            CAQI = (int)Math.Round(index?.Value ?? 0);
            CaqiTitle = index.Description;
            CaqiDescription = index.Advice;
            Pm25 = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "PM25")?.Value ?? 0);
            Pm10 = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "PM10")?.Value ?? 0);
            HumidityPercent = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "HUMIDITY")?.Value ?? 0);
            Pressure = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "PRESSURE")?.Value ?? 0);
            Pm25Percent = (int)Math.Round(standards?.FirstOrDefault(s => s.Pollutant == "PM25")?.Percent ?? 0);
            Pm10Percent = (int)Math.Round(standards?.FirstOrDefault(s => s.Pollutant == "PM10")?.Percent ?? 0);
        }
    }
}