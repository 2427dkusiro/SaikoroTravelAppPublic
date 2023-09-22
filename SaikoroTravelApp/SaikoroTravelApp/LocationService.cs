using Android.Content;

using SaikoroTravelApp.Backends;

using SaikoroTravelCommon.HttpModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace SaikoroTravelApp
{
    internal class LocationService : IDisposable
    {
        private readonly Logger logger;

        public LocationService(Context context)
        {
            logger = Logger.GetInstance(context, DependencyProvider.Default.RequestTimer(context), DependencyProvider.Default.RequestConfigManager(context).User);
        }

        private CancellationTokenSource cts;

        public async Task GetCurrentLocation()
        {
            logger.Info("ログ同期を開始");
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                Location location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(location);
                    Placemark place = placemarks.FirstOrDefault();
                    var locationReport = new LocationReport()
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Altitude = location.Altitude,
                        PostCode = place?.PostalCode,
                        Name = string.Concat(place?.CountryName, place?.AdminArea, place?.Locality, place?.SubLocality, place?.Thoroughfare),
                    };
                    await logger.ReportLocation(locationReport);
                }
            }
            catch (Exception ex)
            {
                logger.Error("位置情報エラー", ex);
                await logger.PostLog();
            }
        }

        public void Dispose()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }
        }
    }
}