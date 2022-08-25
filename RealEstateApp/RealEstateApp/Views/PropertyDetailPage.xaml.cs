using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class PropertyDetailPage : ContentPage
  {
    public PropertyDetailPage(PropertyListItem propertyListItem)
    {
      InitializeComponent();

      Property = propertyListItem.Property;

      IRepository Repository = TinyIoCContainer.Current.Resolve<IRepository>();
      Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

      BindingContext = this;
    }

    public Agent Agent { get; set; }
    public Property Property { get; set; }
    public SpeechOptions SpeechOptions { get; set; } = new SpeechOptions() { Volume = .5f, Pitch = 1.0f };
    public bool IsSpeaking { get; set; } = false;

    private CancellationTokenSource cts;

    private async void EditProperty_Clicked(object sender, System.EventArgs e)
    {
      await Navigation.PushAsync(new AddEditPropertyPage(Property));
    }

    private async void MainImage_Tapped(object sender, System.EventArgs e)
    {
      await Navigation.PushAsync(new ImageListPage(Property));
    }

    private async void TextToSpeechPlay_Clicked(object sender, System.EventArgs e)
    {
      cts = new CancellationTokenSource();
      IsSpeaking = true;
      await TextToSpeech.SpeakAsync(Property.Description, SpeechOptions, cancelToken: cts.Token);
      IsSpeaking = false;
    }

    private void TextToSpeechStop_Clicked(object sender, System.EventArgs e)
    {
      if (cts?.IsCancellationRequested ?? true)
      {
        return;
      }

      IsSpeaking = false;
      cts.Cancel();
    }

    private async void VendorEmail_Tapped(object sender, System.EventArgs e)
    {
      try
      {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var attachmentFilePath = Path.Combine(folder, "property.txt");
        File.WriteAllText(attachmentFilePath, $"{Property.Address}");

        var message = new EmailMessage
        {
          Subject = $"RealEstateApp: {Property.Address}",
          Body = $"Dear {Property.Vendor.FullName},\r\n\r\n",
          To = new List<string>() { Property.Vendor.Email }
          //Cc = ccRecipients,
          //Bcc = bccRecipients
        };
        message.Attachments.Add(new EmailAttachment(attachmentFilePath));

        await Email.ComposeAsync(message);
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

    private async void VendorPhone_Tapped(object sender, System.EventArgs e)
    {
      string[] options = { "SMS", "Call" };

      var selectedOption = await DisplayActionSheet(Property.Vendor.Phone.ToString(), "Cancel", null, options);

      try
      {
        if (selectedOption == options[0])
        {
          var message = new SmsMessage($"Hi {Property.Vendor.FirstName},\r\nI'm writing regarding the property for sale at this address: {Property.Address}.", new[] { Property.Vendor.Phone.ToString() });
          await Sms.ComposeAsync(message);
        }
        else if (selectedOption == options[1])
        {
          PhoneDialer.Open(Property.Vendor.Phone);
        }
      }
      catch (ArgumentNullException ex)
      {
        await DisplayAlert("Error", $"Couldn't read number: {ex.Message}", "OK");
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

    private void MapsButton_Clicked(object sender, EventArgs e)
    {
      OpenMaps(new MapLaunchOptions { Name = Property.Address });
    }

    private void MapsDirectionButton_Clicked(object sender, EventArgs e)
    {
      OpenMaps(new MapLaunchOptions { Name = Property.Address, NavigationMode = NavigationMode.Default });
    }

    private async void OpenMaps(MapLaunchOptions options)
    {
      if (Property.Latitude != null && Property.Longitude != null)
      {
        var location = new Location((double)Property.Latitude, (double)Property.Longitude);
        
        try
        {
          await Map.OpenAsync(location, options);
        }
        catch (Exception ex)
        {
          await DisplayAlert("Error", ex.Message, "OK");
        }
      }
      else
      {
        await DisplayAlert("Error", "Couldn't read coordinates.", "OK");
      }
    }
  }
}
