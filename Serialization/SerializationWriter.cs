using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace SolarGames.Networking.Serialization
{
    public class SerializationWriter : BinaryWriter
    {
        public SerializationWriter(Stream stream)
            : base(stream)
        {
        }

        void Write(IList list)
        {
            int len = list.Count;
            Write((int)len);
            for (int i = 0; i < len; i++)
            {
                WriteObject(list[i]);
            }
        }

        void Write(IDictionary dict)
        {
            int len = dict.Count;
            Write((int)len);
            foreach(object key in dict.Keys)
            {
                WriteObject(key);
                WriteObject(dict[key]);
            }
        }

        void Write(Array array)
        {
            int[] positions = new int[array.Rank];
            Write((int)array.Rank);
            for (int i = 0; i < array.Rank; i++)
                Write((int)array.GetLength(i));
            WriteArray(array, array.GetType().GetElementType(), 0, ref positions);
        }

        void WriteArray(Array array, Type elementType, int dimensionIndex, ref int[] positions)
        {
            int len = array.GetLength(dimensionIndex);
            for (int i = 0; i < len; i++)
            {
                positions[dimensionIndex] = i;

                if (dimensionIndex == array.Rank - 1)
                {
                    WriteObject(array.GetValue(positions));
                }
                else
                {
                    WriteArray(array, elementType, dimensionIndex + 1, ref positions);
                }
            }
        }

        public void WriteObject(object obj)
        {
            if (obj is System.Byte)
                Write((System.Byte)obj);
            else if (obj is System.UInt16)
                Write((System.UInt16)obj);
            else if (obj is Array)
                Write(obj as Array);
            else if (obj is IList)
                Write(obj as IList);
            else if (obj is IDictionary)
                Write(obj as IDictionary);
            else if (obj is ICustomSerialized)
                ((ICustomSerialized)obj).GetObjectData(this);
            else
                throw new Exception("Can't write object type " + obj.GetType().Name);
        }
    }
}
