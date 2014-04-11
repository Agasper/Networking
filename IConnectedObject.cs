using System;
using System.Collections.Generic;
using System.Text;

namespace SolarGames.Networking
{
    public interface IConnectedObject
    {
        void Dispatch(IPacket packet);
        void ConnectionDropped();
    }
}
