using System.Collections.Generic;

namespace FootStone.Client
{

    public struct FacetPu
    {
        public Ice.Object push;
        public string name;
    }

    public class IceClientOptions
    {
        public Ice.Properties Properties;

        public List<FacetPu> PushObjects;

        public bool EnableDispatcher { get;  set; }

        public IceClientOptions()
        {
            EnableDispatcher = false;
            PushObjects = new List<FacetPu>();
        }
    }
}
