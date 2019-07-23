using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CountryMap;
using CountryMap.Droid.Renderers;
using CountryMap.Map;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Position = GeoJSON.Net.Geometry.Position;

[assembly: ExportRenderer(typeof(HighlightableMap), typeof(HighlightableMapRenderer))]
namespace CountryMap.Droid.Renderers
{
    public class HighlightableMapRenderer : MapRenderer
    {
        public HighlightableMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(HighlightableMap.Highlight))
            {
                OnUpdateHighlight();
            }
        }

        private void OnUpdateHighlight()
        {
            var highlightableMap = (HighlightableMap)Element;
            if (highlightableMap == null || NativeMap == null) return;

            NativeMap.Clear();

            if (highlightableMap?.Highlight == null) return;

            var fillColor = highlightableMap.Highlight.FillColor.ToAndroid().ToArgb();
            var strokeColor = highlightableMap.Highlight.StrokeColor.ToAndroid().ToArgb();
            var strokeThickness = highlightableMap.Highlight.StrokeThickness;

            foreach (var polygon in highlightableMap.Highlight.Polygons)
            {
                var polygonOptions = new PolygonOptions();
                
                polygonOptions.InvokeFillColor(fillColor);
                polygonOptions.InvokeStrokeColor(strokeColor);
                polygonOptions.InvokeStrokeWidth(strokeThickness);

                foreach (var position in polygon.Positions)
                {
                    polygonOptions.Add(new LatLng(position.Latitude, position.Longitude));
                }

                NativeMap.AddPolygon(polygonOptions);
            }
        }

        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            base.OnMapReady(map);

            OnUpdateHighlight();
        }
    }
}