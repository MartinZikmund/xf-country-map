using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CountryMap.Map;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Position = Xamarin.Forms.Maps.Position;

namespace CountryMap
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private FeatureCollection _countryPolygons = null;
        private Country _selectedCountry;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            InitData();
        }

        private void InitData()
        {
            _countryPolygons = JsonConvert.DeserializeObject<FeatureCollection>(ReadCountryGeoJson());
            Countries =
                _countryPolygons.Features
                    .Select(f => new Country(f.Id, f.Properties["name"].ToString()))
                    .OrderBy(c => c.Name)
                    .ToArray();
            OnPropertyChanged(nameof(Countries));
        }

        public Country[] Countries { get; private set; }

        public Country SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                if (value != null)
                {
                    HighlightCountry(_selectedCountry.Id);
                }
            }
        }

        private void HighlightCountry(string country)
        {
            var feature = _countryPolygons.Features.FirstOrDefault(f =>
                f.Id.Equals(country, StringComparison.CurrentCultureIgnoreCase));
            if (feature == null) return;

            //Create map polygons
            var polygons = new List<MapPolygon>();
            if (feature.Geometry.Type == GeoJSONObjectType.MultiPolygon)
            {
                //Country area consists of multiple polygons
                var multiPolygonGeometry = feature.Geometry as MultiPolygon;
                foreach (var polygonGeometry in multiPolygonGeometry.Coordinates)
                {
                    var polygon = GeoJsonPolygonToMapPolygon(polygonGeometry);
                    if (polygon != null)
                    {
                        polygons.Add(polygon);
                    }
                }
            }
            else if (feature.Geometry.Type == GeoJSONObjectType.Polygon)
            {
                //Single polygon
                var polygonGeometry = feature.Geometry as Polygon;
                var polygon = GeoJsonPolygonToMapPolygon(polygonGeometry);
                if (polygon != null)
                {
                    polygons.Add(polygon);
                }
            }

            //Create the map highlight
            var highlight = new MapHighlight(polygons.ToArray())
            {
                FillColor = Color.FromRgba(255, 0, 0, 128),
                StrokeColor = Color.FromRgba(0, 0, 255, 128),
                StrokeThickness = 3
            };

            Map.Highlight = highlight;

            //Show the user the appropriate map area
            var minLat = polygons.SelectMany(p => p.Positions).Min(p => p.Latitude);
            var maxLat = polygons.SelectMany(p => p.Positions).Max(p => p.Latitude);
            var minLon = polygons.SelectMany(p => p.Positions).Min(p => p.Longitude);
            var maxLon = polygons.SelectMany(p => p.Positions).Max(p => p.Longitude);
            var centerLat = (minLat + maxLat) / 2;
            var centerLon = (minLon + maxLon) / 2;
            
            Map.MoveToRegion(
                new MapSpan(
                    new Position(centerLat, centerLon),
                    Math.Abs(maxLat - minLat) * 1.2,
                    Math.Abs(maxLon - minLon) * 1.2));
        }

        private MapPolygon GeoJsonPolygonToMapPolygon(Polygon geoJsonPolygon)
        {
            var outer = geoJsonPolygon?.Coordinates.FirstOrDefault();
            if (outer != null)
            {
                var polygon = new MapPolygon(
                    outer.Coordinates
                        .Select(point => new Xamarin.Forms.Maps.Position(point.Latitude, point.Longitude)).ToArray());
                return polygon;
            }
            return null;
        }

        private string ReadCountryGeoJson()
        {
            var assembly = typeof(App).Assembly;
            var stream = assembly.GetManifestResourceStream("CountryMap.Resources.countries.geo.json");
            if (stream == null) throw new InvalidOperationException("Countries GeoJSON resource file missing");
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
