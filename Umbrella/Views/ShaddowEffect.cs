using Xamarin.Forms;

namespace Umbrella.Views
{
    public class ShadowEffect : RoutingEffect
    {
        public Color Color { get; set; }
        public float DistanceX { get; set; }
        public float DistanceY { get; set; }
        public float Radius { get; set; }

        public ShadowEffect() : base($"Umbrella.Views.{nameof(ShadowEffect)}")
        {
            Color = Color.Black;
            DistanceX = 20;
            DistanceY = 30;
            Radius = 50;
        }
    }
}