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
        private IMKOverlay[] _currentOverlays;
        private MapHighlight _currentHighlight;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap != null)
                {
                    nativeMap.RemoveOverlays(nativeMap.Overlays);
                    nativeMap.OverlayRenderer = null;
                }
            }

            if (e.NewElement != null)
            {
                var formsMap = (HighlightableMap)e.NewElement;
                if (Control is MKMapView nativeMap)
                {
                    nativeMap.OverlayRenderer = OverlayRendererHandler;
                }
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
            var nativeMap = Control as MKMapView;
            if (highlightableMap == null || nativeMap == null) return;

            if (_currentOverlays != null)
            {
                nativeMap.RemoveOverlays(_currentOverlays);
            }

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

            _currentOverlays = overlays.ToArray();
            nativeMap.AddOverlays(_currentOverlays);
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