using FontAwesome;
using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AddEditPropertyPage : ContentPage
  {
    private IRepository Repository;

    #region PROPERTIES
    public ObservableCollection<Agent> Agents { get; }

    private Property _property;
    public Property Property
    {
      get => _property;
      set
      {
        _property = value;
        if (_property.AgentId != null)
        {
          SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
        }

      }
    }

    private Agent _selectedAgent;
    public Agent SelectedAgent
    {
      get => _selectedAgent;
      set
      {
        if (Property != null)
        {
          _selectedAgent = value;
          Property.AgentId = _selectedAgent?.Id;
        }
      }
    }

    public Status Status { get; set; } = new Status();
    public Status BatteryStatus { get; set; } = new Status();
    public bool HasInternetConnection { get; set; }
    public bool FlashlightIsOn { get; set; } = false;
    #endregion

    public AddEditPropertyPage(Property property = null)
    {
      InitializeComponent();

      Repository = TinyIoCContainer.Current.Resolve<IRepository>();
      Agents = new ObservableCollection<Agent>(Repository.GetAgents());

      if (property == null)
      {
        Title = "Add Property";
        Property = new Property();
      }
      else
      {
        Title = "Edit Property";
        Property = property;
      }

      BindingContext = this;
    }

    private async void SaveProperty_Clicked(object sender, System.EventArgs e)
    {
      if (IsValid() == false)
      {
        Status.Message = "Please fill in all required fields";
        Status.Color = (Color)Application.Current.Resources["CriticalColor"];

        try
        {
          Vibration.Vibrate(TimeSpan.FromSeconds(1));

          // As an alternative, the HapticFeedback class can be used to give feedback. However, the duration only as limited customizability.
          //HapticFeedback.Perform(HapticFeedbackType.Click);
          //HapticFeedback.Perform(HapticFeedbackType.LongPress);
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
      else
      {
        Repository.SaveProperty(Property);
        await Navigation.PopToRootAsync();
      }
    }

    public bool IsValid()
    {
      if (string.IsNullOrEmpty(Property.Address)
          || Property.Beds == null
          || Property.Price == null
          || Property.AgentId == null)
        return false;

      return true;
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      // Check battery.
      double chargeLevel = Battery.ChargeLevel;
      if (chargeLevel < 0.2f)
      {
        switch (Battery.State)
        {
          case BatteryState.Charging:
            if (Battery.EnergySaverStatus == EnergySaverStatus.On)
            {
              BatteryStatus.Color = (Color)Application.Current.Resources["OkayColor"];
              BatteryStatus.Icon = IconFont.Leaf;
            }
            else
            {
              BatteryStatus.Color = (Color)Application.Current.Resources["WarningColor"];
              BatteryStatus.Icon = "";
            }
            BatteryStatus.Message = $"Battery low: {chargeLevel * 100}% (Charging)";
            break;
          case BatteryState.Discharging:
          case BatteryState.NotCharging:
            BatteryStatus.Color = (Color)Application.Current.Resources["CriticalColor"];
            BatteryStatus.Message = $"Battery low: {chargeLevel * 100}%";
            BatteryStatus.Icon = "";
            break;
        }
      }
      else
      {
        BatteryStatus.Message = "";
      }

      // Check internet connection.
      HasInternetConnection = (Connectivity.NetworkAccess == NetworkAccess.Internet);

      if (!HasInternetConnection)
      {
        DisplayAlert("Attention", "No internet connection.", "OK");
      }
    }

    protected override void OnDisappearing()
    {
      base.OnDisappearing();

      Vibration.Cancel();
    }

    private async void CancelSave_Clicked(object sender, System.EventArgs e)
    {
      await Navigation.PopToRootAsync();
    }

    private async void MapPinButton_Clicked(object sender, System.EventArgs e)
    {
      try
      {
        var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
        var location = await Geolocation.GetLocationAsync(request);

        if (location != null)
        {
          // Set location.
          Property.Latitude = location.Latitude;
          Property.Longitude = location.Longitude;

          // Set address, if connected to the internet.
          if (HasInternetConnection)
          {
            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var placemark = placemarks?.FirstOrDefault();
            if (placemark != null)
            {
              Property.Address = $"{placemark.Thoroughfare} {placemark.FeatureName}, {placemark.PostalCode} {placemark.Locality}, {placemark.CountryName}";
            }
          }
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

    private async void HomeLocationButton_Clicked(object sender, System.EventArgs e)
    {
      if (Property.Address == null)
      {
        await DisplayAlert("Attention", "You need to enter an address first.", "OK");
        return;
      }

      try
      {
        // Get locations from address.
        var locations = await Geocoding.GetLocationsAsync(Property.Address);

        // Set first address from result, if any.
        var location = locations?.FirstOrDefault();
        if (location != null)
        {
          Property.Latitude = location.Latitude;
          Property.Longitude = location.Longitude;
        }
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

    private async void Flashlight_Clicked(object sender, System.EventArgs e)
    {
      try
      {
        HapticFeedback.Perform(HapticFeedbackType.Click); // Give haptic feedback when toggling light.

        if (FlashlightIsOn)
        {
          await Flashlight.TurnOffAsync();
          FlashlightIsOn = false;
        }
        else
        {
          await Flashlight.TurnOnAsync();
          FlashlightIsOn = true;
        }
      }
      catch (FeatureNotSupportedException ex)
      {
        await DisplayAlert("Error", $"Feature not supported: {ex.Message}", "OK");
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
