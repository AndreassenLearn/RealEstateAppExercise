using PropertyChanged;

namespace RealEstateApp.Models
{
  [AddINotifyPropertyChangedInterface]
  public class CompassInformation
  {
    public string Aspect { get; set; }
  }
}
