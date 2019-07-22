using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace CountryMap.Map
{
    public class MapPolygon
    {
        public MapPolygon(Position[] positions)
        {
            Positions = positions;
        }

        public Position[] Positions { get; }
    }
}
