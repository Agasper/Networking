using System;
using System.Collections.Generic;
using System.Text;

namespace SolarGames.Networking.Serialization
{
    public interface ICustomSerialized
    {
        void GetObjectData(SerializationWriter writer);
    }
}
