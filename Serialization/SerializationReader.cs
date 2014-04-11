using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SolarGames.Networking.Serialization
{
    public class SerializationReader : BinaryReader
    {
        public SerializationReader(Stream stream)
            : base(stream)
        {
        }

        public object ReadArray<T>()
        {
            int rank = ReadInt32();
            int[] dimensions = new int[rank];
            for (int i = 0; i < rank; i++)
                dimensions[i] = ReadInt32();

            Array result = Array.CreateInstance(typeof(T), dimensions);

            int[] positions = new int[rank];
            FillArray(result, typeof(T), 0, ref positions);

            return result;
        }

        void FillArray(Array array, Type elementType, int dimensionIndex, ref int[] positions)
        {
            int len = array.GetLength(dimensionIndex);
            for (int i = 0; i < len; i++)
            {
                positions[dimensionIndex] = i;

                if (dimensionIndex == array.Rank - 1)
                {
                    array.SetValue(ReadObject(elementType), positions);
                }
                else
                {
                    FillArray(array, elementType, dimensionIndex + 1, ref positions);
                }
            }
        }

        object ReadObject(Type T)
        {
            if (T == typeof(System.Byte))
                return ReadByte();
            else if (T == typeof(System.UInt16))
                return ReadUInt16();
            else
                throw new Exception("Can't read object type " + T.Name);
        }
    }
}
