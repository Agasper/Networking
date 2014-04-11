using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarGames.Networking
{
    public interface IConnectedObjectUdp
    {
        UdpSession UdpSession { get; set; }
        void Dispatch(IPacket packet);
    }
}
