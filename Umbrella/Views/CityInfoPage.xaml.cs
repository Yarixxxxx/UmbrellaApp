using Xamarin.Forms;
using Umbrella.Models;

namespace Umbrella.Views
{
    public partial class CityInfoPage : ContentPage
    {
        public CityInfoPage(AboutPage.CityData cityInfo)
        {

            InitializeComponent();
            
            BindingContext = cityInfo;
        }
    }
}
