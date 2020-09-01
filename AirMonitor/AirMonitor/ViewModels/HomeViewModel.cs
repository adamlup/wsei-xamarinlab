using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using AirMonitor.Models;
using AirMonitor.Views;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Globalization;
using System.Web;
using System.Collections.ObjectModel;

namespace AirMonitor.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private bool _isBusy;
        private List<Measurement> _items;

        public ObservableCollection<Measurement> itemsList = new ObservableCollection<Measurement>();
        public ObservableCollection<Address> adreessList = new ObservableCollection<Address>();
        public ObservableCollection<Address> addressToBind { get { return adreessList; } }

        public HomeViewModel() { }
        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;

            Initialize();
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public List<Measurement> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private async Task Initialize()
        {
            //adreessList.Add(new Address { Street = "ulica1" });
            //adreessList.Add(new Address { Street = "ulica2" });
            //adreessList.Add(new Address { Street = "ulica3" });
            //adreessList.Add(new Address { Street = "ulica4" });

            itemsList.Add(new Measurement { Installation = new Installation { Address = new Address { Street = "to" } } });
            itemsList.Add(new Measurement { Installation = new Installation { Address = new Address { Street = "se" } } });
            itemsList.Add(new Measurement { Installation = new Installation { Address = new Address { Street = "ne" } } });
            itemsList.Add(new Measurement { Installation = new Installation { Address = new Address { Street = "wrati" } } });

            IsBusy = true;

            var location = await GetLocation();
            var installations = await GetInstallations(location, maxResults: 3);
            var data = await GetMeasurementsForInstallations(installations);

            _items = new List<Measurement>(data);

            IsBusy = false; 

        }

        private async Task<Location> GetLocation()
        {
            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    //Console.WriteLine("QQQQQQ location QQQQQQQ: " + location);
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    //Console.WriteLine("BBBBBBBBBBBB location BBBBBBBBB: " + location);
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                }

                return location;
            }
            // Handle different exceptions separately, for example to display different messages to the user
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (FeatureNotEnabledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (PermissionException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }

        private async Task<IEnumerable<Installation>> GetInstallations(Location location, double maxDistanceInKm = 3, int maxResults = -1)
        {
            if (location == null)
            {
                //Console.WriteLine("GET INSTALLATIONS, INSTALATIONS NULL");
                System.Diagnostics.Debug.WriteLine("No location data.");
                return null;
            }

            var query = GetQuery(new Dictionary<string, object>
            {
                { "lat", location.Latitude },
                { "lng", location.Longitude },
                { "maxDistanceKM", maxDistanceInKm },
                { "maxResults", maxResults }
            });
            var url = GetAirlyApiUrl(App.AirlyApiInstallationUrl, query);

            var response = await GetHttpResponseAsync<IEnumerable<Installation>>(url);
            //Console.WriteLine("GET INSTALLATIONS, INSTALATIONS NIE NULL, URL:  " + url);
            return response;
        }

        private async Task<IEnumerable<Measurement>> GetMeasurementsForInstallations(IEnumerable<Installation> installations)
        {
            if (installations == null)
            {
                //Console.WriteLine("GET MEASUREMNET, INSTALATIONS NULL");
                System.Diagnostics.Debug.WriteLine("No installations data.");
                return null;
            }

            var measurements = new List<Measurement>();

            foreach (var installation in installations)
            {
                var query = GetQuery(new Dictionary<string, object>
                {
                    { "installationId", installation.Id }
                });
                var url = GetAirlyApiUrl(App.AirlyApiMeasurementUrl, query);

                var response = await GetHttpResponseAsync<Measurement>(url);
                //Console.WriteLine("GET MEASUREMNET, URL:  " + url);
                if (response != null)
                {
                    //Console.WriteLine("GET MEASUREMNET, nie NULL:  " + response);
                    response.Installation = installation;
                    response.CurrentDisplayValue = (int)Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                    measurements.Add(response);
                }
            }
            //Console.WriteLine("GET MEASUREMENT, INSTALATIONS NIE NULL, measurements: " + measurements);
            return measurements;
        }

        private string GetQuery(IDictionary<string, object> args)
        {
            if (args == null) return null;

            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (var arg in args)
            {
                if (arg.Value is double number)
                {
                    query[arg.Key] = number.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    query[arg.Key] = arg.Value?.ToString();
                }
            }

            return query.ToString();
        }

        private string GetAirlyApiUrl(string path, string query)
        {
            var builder = new UriBuilder(App.AirlyApiUrl);
            builder.Port = -1;
            builder.Path += path;
            builder.Query = query;
            string url = builder.ToString();

            return url;
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(App.AirlyApiUrl);

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Add("apikey", App.AirlyApiKey);
            return client;
        }

        private async Task<T> GetHttpResponseAsync<T>(string url)
        {
            try
            {
                var client = GetHttpClient();
                var response = await client.GetAsync(url);

                if (response.Headers.TryGetValues("X-RateLimit-Limit-day", out var dayLimit) &&
                    response.Headers.TryGetValues("X-RateLimit-Remaining-day", out var dayLimitRemaining))
                {
                    System.Diagnostics.Debug.WriteLine($"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");
                }
                //Console.WriteLine("GET RESPONSE ASYNC, RESPONSE:  " + response);
                switch ((int)response.StatusCode)
                {
                    case 200:
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(content);
                        return result;
                    case 429: // too many requests
                        System.Diagnostics.Debug.WriteLine("Too many requests");
                        break;
                    default:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Response error: {errorContent}");
                        return default;
                }
            }
            catch (JsonReaderException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return default;
        }

        private ICommand _goToDetailsCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand ?? (_goToDetailsCommand = new Command(OnGoToDetails));

        private void OnGoToDetails()
        {
            _navigation.PushAsync(new DetailsPage());
        }
    }
}
