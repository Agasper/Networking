using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SolarGames.Networking
{
    public interface IPacket
    {
        BinaryReader Reader { get; }
        BinaryWriter Writer { get; }
        int Type { get; set;  }
        void WriteSerialize(object obj);
        object ReadSerialized();

        string ToString();
    }
}
