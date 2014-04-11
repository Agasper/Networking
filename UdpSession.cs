using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace SolarGames.Networking
{
    public class UdpSession
    {
        public IPAddress address;
        public int port;
        public int sessionId;
        public uint inSequence;
        public uint outSequence;
        public DateTime lastData;
        public IConnectedObjectUdp obj;

        public override int GetHashCode()
        {
            return sessionId;
        }

        public string GetKey()
        {
            return address.ToString() + ":" + port.ToString() + ":" + sessionId.ToString();
        }
    }
}
