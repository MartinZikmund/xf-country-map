using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CountryMap.Map
{
    public class MapHighlight
    {
        public MapHighlight(MapPolygon[] polygons)
        {
            Polygons = polygons;
        }

        public MapPolygon[] Polygons { get; }

        public Color FillColor { get; set; }

        public Color StrokeColor { get; set; }

        public float StrokeThickness { get; set; }
    }
}
