using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using SolarGames.Networking.Crypting;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace SolarGames.Networking
{

	public class TcpPacket : IPacket, IDisposable
	{
		
	    public int Type {
	        get { return type; }
	        set { type = value; }
	    }
	    public BinaryReader Reader { get { return reader; } }
		public BinaryWriter Writer { get { return writer; } }
	
	    int type;
	
	    MemoryStream stream;
	    BinaryWriter writer;
	    BinaryReader reader;
	
	    public TcpPacket(int type)
	    {
	        this.type = type;
	        this.stream = new MemoryStream();
            Init();
	    }

        public TcpPacket(byte[] body, int type)
	    {
            this.type = type;
	        stream = new MemoryStream(body);
            Init();
	    }

        void Init()
        {
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }

        //public void RLEDecode()
        //{
        //    byte[] data = stream.ToArray();
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (BinaryWriter b = new BinaryWriter(ms))
        //        {
        //            for (int i = 0; i < data.Length; i++)
        //            {
        //                if (data[i] != 144)
        //                {
        //                    b.Write((byte)data[i]);
        //                    continue;
        //                }

        //                byte cnt = data[++i];
        //                i++;
        //                if (cnt == 0)
        //                    b.Write((byte)144);
        //                else
        //                {
        //                    for (int c = 0; c < cnt; c++)
        //                        b.Write((byte)data[i]);
        //                }
        //            }
        //        }
        //    }
        //}

        //public void RLEEncode()
        //{
        //    byte[] data;
        //    lock (stream)
        //        data = stream.ToArray();
        //    bool finish = false;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (BinaryWriter bw = new BinaryWriter(ms))
        //        {
        //            byte count = 1;
        //            for (int i = 0; i < data.Length; i++)
        //            {
        //                if (i < data.Length - 1)
        //                {
        //                    if (data[i] == data[i + 1] && count < 255)
        //                        count++;
        //                    else
        //                        finish = true;
        //                }
        //                else
        //                    finish = true;
                        
        //                if (finish)
        //                {
        //                    finish = false;
        //                    if (count == 1)
        //                    {
        //                        if (data[i] == 144)
        //                        {
        //                            bw.Write(data[i]);
        //                            bw.Write((byte)0);
        //                        }
        //                        else
        //                            bw.Write(data[i]);
        //                    }
        //                    else
        //                    {
        //                        bw.Write((byte)144);
        //                        bw.Write((byte)count);
        //                        bw.Write(data[i]);
        //                        count = 1;
        //                    }
        //                }
        //            }
        //        }

        //        byte[] compressed = ms.ToArray();
        //        lock (stream)
        //        {
        //            stream.SetLength(0);
        //            stream.Write(compressed, 0, compressed.Length);
        //        }
        //    }
        //}
        
	
		
		public byte[] GetBody()
		{
			return stream.ToArray();
		}

        public byte[] ToByteArray(ICipher cipher)
        {
            using (MemoryStream data = new MemoryStream())
            {
                using (BinaryWriter data_writer = new BinaryWriter(data))
                {
                    byte[] body = stream.ToArray();
                    if (cipher != null)
                        cipher.Encrypt(ref body, body.Length);

                    data_writer.Write(CodePacketType((ushort)type, (uint)body.Length));
                    data_writer.Write((uint)body.Length);
                    data_writer.Write(body, 0, (int)body.Length);

                    return data.ToArray();
                }
            }
        }

        static ushort CodePacketType(ushort type, uint len)
        {
            int a1 = ((ushort)len) ^ 0xAC53;
            int a2 = ((ushort)len) ^ 0xAAAA;
            return (ushort)(type ^ a1 ^ a2);
        }

	
	    public byte[] ToByteArray()
	    {
            return ToByteArray(null);
	    }

        public static TcpPacket Parse(ref byte[] buffer)
        {
            return Parse(ref buffer, null);
        }

        public static TcpPacket Parse(ref byte[] buffer, ICipher cipher)
	    {
	        if (buffer.Length < 6) return null;
	        MemoryStream instream = new MemoryStream(buffer);
	        BinaryReader reader = new BinaryReader(instream);
            TcpPacket resultPacket;
            try
            {
                ushort type = reader.ReadUInt16();
                uint len = reader.ReadUInt32();
                if (buffer.Length - 6 < len) return null;

                type = CodePacketType(type, len);

                byte[] body = reader.ReadBytes((int)len);

                if (cipher != null)
                    cipher.Decrypt(ref body, body.Length);

                resultPacket = new TcpPacket(body, type);

                int tailsize = buffer.Length - 6 - (int)len;
                if (tailsize == 0)
                    buffer = new byte[0];
                else
                    buffer = reader.ReadBytes(tailsize);

            }
            catch
            {
                throw;
            }
            finally
            {
                reader.Close();
                instream.Close();
            }

	        return resultPacket;
	    }

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
                //reader.Dispose();
            }
            if (writer != null)
            {
                writer.Close();
                //writer.Dispose();
            }
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public void WriteSerialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                byte[] data = ms.ToArray();
                writer.Write(data.Length);
                writer.Write(data);
            }
        }

        public object ReadSerialized()
        {
            int len = reader.ReadInt32();
            byte[] data = reader.ReadBytes(len);

            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }
	
	    public override string ToString()
	    {
	        return string.Format("TcpPacket[Type={0},Size={1}]", type, stream.Length);
	    }
	
	}
	
}