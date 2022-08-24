using PropertyChanged;
using RealEstateApp.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ImageListPage : ContentPage
  {
    public Property Property { get; set; }
    public int Position { get; set; }

    public ImageListPage(Property property)
    {
      InitializeComponent();

      Property = property;

      this.BindingContext = this;
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      Accelerometer.ShakeDetected += Accelerometer_ShakeDetected;
      StartAccelerometer();
    }

    protected override void OnDisappearing()
    {
      base.OnDisappearing();

      Accelerometer.ShakeDetected -= Accelerometer_ShakeDetected;
      StopAccelerometer();
    }

    private void AdvanceCarousel(bool reverse = false)
    {
      if (Property.ImageUrls.Count <= 1)
      {
        return;
      }

      if (reverse)
      {
        // Go one back.
        if (Position == 0)
        {
          Position = Property.ImageUrls.Count - 1;
        }
        else
        {
          Position--;
        }
      }
      else
      {
        // Go one forward.
        if (Position == Property.ImageUrls.Count - 1)
        {
          Position = 0;
        }
        else
        {
          Position++;
        }
      }
    }

    private void Accelerometer_ShakeDetected(object sender, EventArgs e)
    {
      HapticFeedback.Perform(HapticFeedbackType.LongPress);
      AdvanceCarousel();
    }

    private async void StartAccelerometer()
    {
      try
      {
        Accelerometer.Start(SensorSpeed.Game);
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

    private async void StopAccelerometer()
    {
      try
      {
        if (Accelerometer.IsMonitoring) Accelerometer.Stop();
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