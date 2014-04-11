using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace SolarGames.Networking.Serialization
{
    public class Serializator
    {

        struct Headers
        {
            public string FullName;
            public int ContentLen;
        }

        public byte[] Serialize(ICustomSerialized obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    byte[] content;

                    using (MemoryStream ms_content = new MemoryStream())
                    {
                        using (SerializationWriter sw_content = new SerializationWriter(ms))
                        {
                            obj.GetObjectData(sw_content);
                        }

                        content = ms_content.ToArray();
                    }

                    bw.Write(obj.GetType().FullName);
                    bw.Write(content.Length);
                    bw.Write(content);
                }

                return ms.ToArray();
            }
        }

        public object Deserialize(byte[] array)
        {
            object result = null;
            using (MemoryStream ms = new MemoryStream(array))
            {
                using (SerializationReader sr = new SerializationReader(ms))
                {
                    Headers headers = ReadHeaders(sr);
                    Type type = Type.GetType(headers.FullName);

                    ConstructorInfo constructor_info = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(SerializationReader) }, null);

                    result = constructor_info.Invoke(new Object[] { sr });
                    //result = Activator.CreateInstance(type, sr);
                }
            }

            return result;
        }

        Headers ReadHeaders(SerializationReader reader)
        {
            Headers result = new Headers();
            result.FullName = reader.ReadString();
            result.ContentLen = reader.ReadInt32();

            return result;
        }

        public Serializator()
        {

        }
    }
}
