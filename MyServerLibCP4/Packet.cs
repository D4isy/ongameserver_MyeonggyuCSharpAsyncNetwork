using System;

namespace MyServerLibCP4
{
    public class Packet
    {
        readonly static private int BUFFER_SIZE = 10240;

        private byte[] Buffer;
        private int BufferPos = 0;
        public int PacketLen
        {
            get;
            set;
        }

        public byte[] GetBuffer() { return Buffer; }
        
        public Packet()
        {
            BufferPos = 0;
            PacketLen = 0;
            Buffer = new byte[BUFFER_SIZE];
        }

        public Packet(int type)
        {
            BufferPos = 0;
            PacketLen = 0;
            Buffer = new byte[BUFFER_SIZE];
            WriteInt((int)type);
        }

        public Packet(Packet p)
        {
            BufferPos = 0;
            PacketLen = p.PacketLen;
            Buffer = new Byte[BUFFER_SIZE];

            Array.Copy(p.Buffer, Buffer, BUFFER_SIZE);
        }

        public void Copy(Packet p)
        {
            BufferPos = 0;
            PacketLen = p.PacketLen;
            Buffer = new Byte[BUFFER_SIZE];

            Array.Copy(p.Buffer, Buffer, BUFFER_SIZE);
        }

        public Packet(byte[] buffer)
        {
            Buffer = buffer;
            BufferPos = 0;
        }

        public Packet(byte[] buffer, int startpos, int len)
        {
            Buffer = new byte[len];
            Array.Copy(buffer, startpos, Buffer, 0, len);
            PacketLen = len;
        }

        public void Reset()
        {
            BufferPos = 0;
            PacketLen = 0;
        }

        public int Position
        {
            get { return BufferPos; }
            set { BufferPos = value; }
        }

        public void SetInt(int data, int offset)
        {            
            BitConverter.GetBytes(data).CopyTo(Buffer, offset);
        }

        public void WriteDateTime(DateTime data) 
        {
            long d = data.ToBinary();
            Write(BitConverter.GetBytes(d)); 
        }

        public void WriteUint(uint data) { Write(BitConverter.GetBytes(data)); }
        public void WriteUshort(ushort data) { Write(BitConverter.GetBytes(data)); }
        public void WriteDouble(double data) { Write(BitConverter.GetBytes(data)); }
        public void WriteFloat(float data) { Write(BitConverter.GetBytes(data)); }
        public void WriteChar(char data) { Write(BitConverter.GetBytes(data)); }
        public void WriteBool(bool data) { Write(BitConverter.GetBytes(data)); }
        public void WriteShort(short data) { Write(BitConverter.GetBytes(data)); }
        public void WriteLong(long data) { Write(BitConverter.GetBytes(data)); }
        public void WriteInt(int data) { Write(BitConverter.GetBytes(data)); }
        public void WriteString(string data)
        {
            byte[] ConvData = System.Text.Encoding.Unicode.GetBytes(data);
            WriteInt(ConvData.Length);
            Write(ConvData);
        }

        private void Write(byte[] data)
        {
            data.CopyTo(Buffer, BufferPos);
            BufferPos += data.Length;
            PacketLen = BufferPos;
        }

        public void WriteData(byte[] buffer, int len)
        {
            Array.Copy(buffer, 0, Buffer, BufferPos, len);
            BufferPos += len;
            PacketLen = BufferPos;
        }

        public uint ReadUint()
        {
            uint read = BitConverter.ToUInt32(Buffer, BufferPos);
            BufferPos += sizeof(uint);
            return read;
        }

        public ushort ReadUshort()
        {
            ushort read = BitConverter.ToUInt16(Buffer, BufferPos);
            BufferPos += sizeof(ushort);
            return read;
        }

        public double ReadDouble()
        {
            double read = BitConverter.ToDouble(Buffer, BufferPos);
            BufferPos += sizeof(double);
            return read;
        }

        public float ReadFloat()
        {
            float read = BitConverter.ToSingle(Buffer, BufferPos);
            BufferPos += sizeof(float);
            return read;
        }

        public char ReadChar()
        {
            char read = BitConverter.ToChar(Buffer, BufferPos);
            BufferPos += sizeof(char);
            return read;
        }

        public bool ReadBool()
        {
            bool read = BitConverter.ToBoolean(Buffer, BufferPos);
            BufferPos += sizeof(bool);
            return read;
        }

        public short ReadShort()
        {
            short read = BitConverter.ToInt16(Buffer, BufferPos);
            BufferPos += sizeof(short);
            return read;
        }

        public long ReadLong()
        {
            long read = BitConverter.ToInt64(Buffer, BufferPos);
            BufferPos += sizeof(long);
            return read;
        }

        public int ReadInt()
        {
            int read = BitConverter.ToInt32(Buffer, BufferPos);
            BufferPos += sizeof(int);
            return read;
        }

        public string ReadString()
        {
            int Count = ReadInt();

            string s = System.Text.Encoding.Unicode.GetString(Buffer, BufferPos, Count);
            BufferPos += Count;
            return s;
        }

        public string ReadString(int Count)
        {
            string s = System.Text.Encoding.Unicode.GetString(Buffer, BufferPos, Count);
            BufferPos += Count;
            string ts = s.TrimEnd('\0');
            return ts;
        }

        public void AddPacket(Packet p)
        {
            Array.Copy(p.Buffer, 0, Buffer, BufferPos, p.PacketLen);
            BufferPos += p.PacketLen;
            PacketLen += p.PacketLen;
        }

    }

    //public static class PacketExtend
    //{
    //    public static void Read<T>(this System.Collections.Generic.List<T> list, ref Packet p) where T : Packable, new()
    //    {
    //        list.Clear();
    //        int Count = p.ReadInt();

    //        for (int Index = 0; Index < Count; ++Index)
    //        {
    //            T Src = new T();
    //            Src.Read(ref p);
    //            list.Add(Src);
    //        }
    //    }

    //    public static void Write<T>(this System.Collections.Generic.List<T> list, ref Packet p) where T : Packable, new()
    //    {
    //        p.WriteInt(list.Count);
    //        foreach (T item in list)
    //            item.Write(ref p);
    //    }
    //}

}