using System;

namespace FootStone.FrontIce
{
    public interface IServantBase
    {
        Orleans.IClusterClient OrleansClient { get; }

        FrontOrleans.IObserverClient ObserverClient { get; }
    }
}
