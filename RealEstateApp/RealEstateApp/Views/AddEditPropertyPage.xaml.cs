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

    public string StatusMessage { get; set; }

    public Color StatusColor { get; set; } = Color.White;
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
        StatusMessage = "Please fill in all required fields";
        StatusColor = Color.Red;
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

          // Set address.
          var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
          var placemark = placemarks?.FirstOrDefault();
          if (placemarks != null)
          {
            Property.Address = $"{placemark.Thoroughfare} {placemark.FeatureName}, {placemark.PostalCode} {placemark.Locality}, {placemark.CountryName}";
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
  }
}
