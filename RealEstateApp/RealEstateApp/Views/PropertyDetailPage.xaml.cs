using RealEstateApp.Models;
using RealEstateApp.Services;
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
  }
}
