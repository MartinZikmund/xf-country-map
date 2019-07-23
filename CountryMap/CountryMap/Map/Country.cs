using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace CountryMap.Map
{
    public class Country
    {
        public Country(string id, string name) =>
            (Id, Name) = (id, name);

        public string Id { get; }

        public string Name { get; }
    }
}
