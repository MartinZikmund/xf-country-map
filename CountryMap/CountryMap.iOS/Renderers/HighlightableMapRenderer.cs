using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoreLocation;
using CountryMap.iOS.Renderers;
using CountryMap.Map;
using MapKit;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HighlightableMap), typeof(HighlightableMapRenderer))]
namespace CountryMap.iOS.Renderers
{
    public class HighlightableMapRenderer : MapRenderer
    {
        private MapHighlight _currentHighlight;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (Control is MKMapView nativeMap)
                {
                    nativeMap.RemoveOverlays(nativeMap.Overlays);
                    nativeMap.OverlayRenderer = null;
                }
            }

            if (e.NewElement != null)
            {
                if (Control is MKMapView nativeMap)
                {
                    nativeMap.OverlayRenderer = OverlayRendererHandler;
                    OnUpdateHighlight();
                }
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
            var nativeMap = Control as MKMapView;
            if (highlightableMap == null || nativeMap == null) return;

            nativeMap.RemoveOverlays(nativeMap.Overlays);

            if (highlightableMap?.Highlight == null) return;

            _currentHighlight = highlightableMap.Highlight;

            var overlays = new List<IMKOverlay>();
            foreach (var polygon in highlightableMap.Highlight.Polygons)
            {
                var coordinates = new List<CLLocationCoordinate2D>();
                foreach (var position in polygon.Positions)
                {
                    coordinates.Add(new CLLocationCoordinate2D(position.Latitude, position.Longitude));
                }

                var blockOverlay = MKPolygon.FromCoordinates(coordinates.ToArray());
                overlays.Add(blockOverlay);
            }

            nativeMap.AddOverlays(overlays.ToArray());
        }

        private MKOverlayRenderer OverlayRendererHandler(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            if (_currentHighlight != null)
            {
                var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
                return new MKPolygonRenderer(overlay as MKPolygon)
                {
                    FillColor = _currentHighlight.FillColor.ToUIColor(),
                    StrokeColor = _currentHighlight.StrokeColor.ToUIColor(),
                    LineWidth = (nfloat)_currentHighlight.StrokeThickness
                };
            }
            return null;
        }
    }
}