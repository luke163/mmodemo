using System;
using System.Collections.Generic;

namespace FootStone.FrontIce
{
    public struct Facet
    {
        public Type type;
        public string name;
    }

    public class IceOptions
    {
        public string ConfigFile { get; set; }

        public List<Facet> FacetTypes = new List<Facet>();

        public Ice.Logger Logger { get; set; }

        public void AddFacetType(Type t, string n)
        {
            var facetType = new Facet() { type = t, name = n };
            FacetTypes.Add(facetType);
        }
    }
}