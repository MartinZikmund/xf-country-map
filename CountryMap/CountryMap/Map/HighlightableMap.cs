using Xamarin.Forms;

namespace CountryMap.Map
{
    public class HighlightableMap : Xamarin.Forms.Maps.Map
    {
        public static readonly BindableProperty HighlightProperty =
            BindableProperty.Create(nameof(Highlight), typeof(MapHighlight), typeof(HighlightableMap), null);

        public MapHighlight Highlight
        {
            get => (MapHighlight)GetValue(HighlightProperty);
            set => SetValue(HighlightProperty, value);
        }
    }
}
