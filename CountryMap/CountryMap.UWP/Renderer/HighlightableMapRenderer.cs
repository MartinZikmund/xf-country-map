using System.Collections.Generic;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using CountryMap.Map;
using CountryMap.UWP.Renderer;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;
using MapPolygon = Windows.UI.Xaml.Controls.Maps.MapPolygon;

[assembly: ExportRenderer(typeof(HighlightableMap), typeof(HighlightableMapRenderer))]
namespace CountryMap.UWP.Renderer
{
    public class HighlightableMapRenderer : MapRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                UpdateHighlight();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(HighlightableMap.Highlight))
            {
                UpdateHighlight();
            }
        }

        private void UpdateHighlight()
        {
            var highlightableMap = (HighlightableMap)Element;
            var nativeMap = Control as MapControl;
            if (highlightableMap == null || nativeMap == null) return;

            nativeMap.MapElements.Clear();

            if (highlightableMap?.Highlight == null) return;

            foreach (var polygon in highlightableMap.Highlight.Polygons)
            {
                var coordinates = new List<BasicGeoposition>();
                foreach (var position in polygon.Positions)
                {
                    coordinates.Add(new BasicGeoposition()
                        {Latitude = position.Latitude, Longitude = position.Longitude});
                }

                var fillColor = highlightableMap.Highlight.FillColor.ToWindowsColor();
                var strokeColor = highlightableMap.Highlight.StrokeColor.ToWindowsColor();
                var strokeThickness = highlightableMap.Highlight.StrokeThickness;
                var nativePolygon = new MapPolygon
                {

                    FillColor = fillColor,
                    StrokeColor = strokeColor,
                    StrokeThickness = strokeThickness,
                    Path = new Geopath(coordinates)
                };
                nativeMap.MapElements.Add(nativePolygon);
            }
        }
    }
}
