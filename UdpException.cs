using System;
using System.Collections.Generic;
using System.Text;

namespace SolarGames.Networking
{
    class UdpException : Exception
    {
        public UdpException()
        {
        }

        public UdpException(string message)
            : base(message)
        {
        }
    }
}
