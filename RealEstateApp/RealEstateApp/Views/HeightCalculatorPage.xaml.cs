using PropertyChanged;
using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class HeightCalculatorPage : ContentPage
  {
    public double SeaLevelPressure { get; set; } = 1013.25f;
    public double CurrentPressure { get; set; }
    public double CurrentAltitude { get; set; }
    public BarometerMeasurement CurrentBarometerMeasurement { get; set; } = new BarometerMeasurement();
    public ObservableCollection<BarometerMeasurement> BarometerMeasurements { get; set; } = new ObservableCollection<BarometerMeasurement>();
    
    public HeightCalculatorPage()
    {
      InitializeComponent();
      this.BindingContext = this;
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      Barometer.ReadingChanged += Barometer_ReadingChanged;
      StartBarometer();
    }

    protected override void OnDisappearing()
    {
      base.OnDisappearing();

      Barometer.ReadingChanged -= Barometer_ReadingChanged;
      StopBarometer();
    }

    public void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
    {
      CurrentPressure = e.Reading.PressureInHectopascals;
      CurrentAltitude = 44307.694 * (1 - Math.Pow(CurrentPressure / SeaLevelPressure, 0.190284));
    }

    private async void StartBarometer()
    {
      try
      {
        Barometer.Start(SensorSpeed.UI);
      }
      catch (FeatureNotSupportedException ex)
      {
        await DisplayAlert("Error", $"Feature not supported: {ex.Message}", "OK");
      }
      catch (Exception ex)
      {
        await DisplayAlert("Error", ex.Message, "OK");
      }
    }

    private async void StopBarometer()
    {
      try
      {
        if (Barometer.IsMonitoring) Barometer.Stop();
      }
      catch (FeatureNotSupportedException ex)
      {
        await DisplayAlert("Error", $"Feature not supported: {ex.Message}", "OK");
      }
      catch (Exception ex)
      {
        await DisplayAlert("Error", ex.Message, "OK");
      }
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
      CurrentBarometerMeasurement.Altitude = CurrentAltitude;
      CurrentBarometerMeasurement.Pressure = CurrentPressure;

      if (BarometerMeasurements.Count != 0)
      {
        CurrentBarometerMeasurement.HeightChange = CurrentBarometerMeasurement.Altitude - BarometerMeasurements[BarometerMeasurements.Count - 1].Altitude;
      }

      BarometerMeasurements.Add(CurrentBarometerMeasurement);
      CurrentBarometerMeasurement = new BarometerMeasurement();
    }

    private async void CalibrateButton_Clicked(object sender, EventArgs e)
    {
      try
      {
        var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10)));
        location.AltitudeReferenceSystem = AltitudeReferenceSystem.Geoid;

        if (location?.Altitude.HasValue ?? false)
        {
          SeaLevelPressure = 5336367864602676011468635241335234007.0 * Math.Pow(22153847.0, 12145.0/47571.0) * CurrentPressure / Math.Pow(22153847.0 - 500.0 * location.Altitude.Value, 250000.0/47571.0);
        }
        else
        {
          await DisplayAlert("Error", "Unable to calibrate", "OK");
        }
      }
      catch (FeatureNotSupportedException ex)
      {
        await DisplayAlert("Error", $"Feature not supported: {ex.Message}", "OK");
      }
      catch (FeatureNotEnabledException ex)
      {
        await DisplayAlert("Error", $"Feature not enabled: {ex.Message}", "OK");
      }
      catch (PermissionException ex)
      {
        await DisplayAlert("Error", $"Persmissions: {ex.Message}", "OK");
      }
      catch (Exception ex)
      {
        await DisplayAlert("Error", ex.Message, "OK");
      }
    }
  }
}