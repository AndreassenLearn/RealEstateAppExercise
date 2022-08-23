using PropertyChanged;
using Xamarin.Forms;

namespace RealEstateApp.Models
{
  [AddINotifyPropertyChangedInterface]
  public class Status
  {
    public string Message { get; set; }
    public Color Color { get; set; } = Color.White;
    public string Icon { get; set; }
  }
}
