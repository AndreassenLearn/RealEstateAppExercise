using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

    private async void ResetPreferencesButton_Clicked(object sender, EventArgs e)
    {
      Preferences.Clear();

      await DisplayAlert("Success", "Preferences cleared.", "OK");
    }
  }
}