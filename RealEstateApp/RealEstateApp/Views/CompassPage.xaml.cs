using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Essentials.Permissions;

namespace RealEstateApp.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class CompassPage : ContentPage
  {
    public string CurrentAspect { get; set; }
    public double RotationAngle { get; set; }
    public double CurrentHeading { get; set; }

    public CompassPage()
    {
      InitializeComponent();

      this.BindingContext = this;
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      Compass.ReadingChanged += Compass_ReadingChanged;
      StartCompass();
    }

    protected override void OnDisappearing()
    {
      base.OnDisappearing();

      Compass.ReadingChanged -= Compass_ReadingChanged;
      StopCompass();
    }

    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
      CurrentHeading = e.Reading.HeadingMagneticNorth;
      RotationAngle = CurrentHeading * -1.0;

      if (CurrentHeading < 360 + 45 && CurrentHeading >= 360 - 45)
      {
        CurrentAspect = "North";
      }
      else if (CurrentHeading < 90 + 45 && CurrentHeading >= 90 - 45)
      {
        CurrentAspect = "East";
      }
      else if (CurrentHeading < 180 + 45 && CurrentHeading >= 180 - 45)
      {
        CurrentAspect = "South";
      }
      else if (CurrentHeading < 270 + 45 && CurrentHeading >= 270 - 45)
      {
        CurrentAspect = "West";
      }
    }

    private async void StartCompass()
    {
      try
      {
        Compass.Start(SensorSpeed.UI);
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

    private async void StopCompass()
    {
      try
      {
        if (Compass.IsMonitoring) Compass.Stop();
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
  }
}
