using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyServerLibCP4
{
    // 배열 식별자는 c++의 포인터랑 비슷하다. 즉 byte[] p = anotherbytearray 하면 p는 anotherbytearray를 가르키는 포인터인셈~
    // Read...함수를 통해서만 사용할것이니, 프로퍼티 정의할 필요없다.
    public class ReadOnlyPacket
    {
        private byte[] _Buffer;
        private int StartPos = 0;
        private int BufferPos = 0;
        private int PacketLen;

        public int REMAINBYTES
        {
            get { return PacketLen - BufferPos; }
        }

        public ReadOnlyPacket(byte[] buffer, int startpos, int len)
        {
            _Buffer = buffer;            
            PacketLen = len;
            BufferPos = 0;
            StartPos = startpos;
        }

        public byte[] ReadBytesAll()
        {
            int len = REMAINBYTES;
            byte[] msg = new byte[len];
            Array.Copy(_Buffer, BufferPos, msg, 0, msg.Length);
            BufferPos += len;
            return msg;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] msg = new byte[length];
            Array.Copy(_Buffer, BufferPos, msg, 0, length);
            BufferPos += length;
            return msg;
        }

        public uint ReadUint()
        {
            int size = sizeof(uint);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            uint read = BitConverter.ToUInt32(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(uint);
            return read;
        }

        public ushort ReadUshort()
        {
            int size = sizeof(ushort);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            ushort read = BitConverter.ToUInt16(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(ushort);
            return read;
        }

        public double ReadDouble()
        {
            int size = sizeof(double);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            double read = BitConverter.ToDouble(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(double);
            return read;
        }

        public float ReadFloat()
        {
            int size = sizeof(float);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            float read = BitConverter.ToSingle(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(float);
            return read;
        }

        public char ReadChar()
        {
            int size = sizeof(char);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            char read = BitConverter.ToChar(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(char);
            return read;
        }

        public bool ReadBool()
        {
            int size = sizeof(bool);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            bool read = BitConverter.ToBoolean(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(bool);
            return read;
        }

        public short ReadShort()
        {
            int size = sizeof(short);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            short read = BitConverter.ToInt16(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(short);
            return read;
        }

        public long ReadLong()
        {
            int size = sizeof(long);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            long read = BitConverter.ToInt64(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(long);
            return read;
        }

        public int ReadInt()
        {
            int size = sizeof(int);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            int read = BitConverter.ToInt32(_Buffer, StartPos + BufferPos);
            BufferPos += sizeof(int);
            return read;
        }

        public string ReadString()
        {
            int size = sizeof(int);
            if (BufferPos + size > PacketLen)
                throw new ArgumentOutOfRangeException();
            int Count = ReadInt();

            string s = System.Text.Encoding.Unicode.GetString(_Buffer, StartPos + BufferPos, Count);
            BufferPos += Count;
            return s;
        }

        public string ReadString(int Count)
        {
            string s = System.Text.Encoding.Unicode.GetString(_Buffer, StartPos + BufferPos, Count);
            BufferPos += Count;
            string ts = s.TrimEnd('\0');
            return ts;
        }

        public void ReadDatas(byte[] dest)
        {
            Array.Copy(_Buffer, BufferPos, dest, 0, dest.Length);
            BufferPos += dest.Length;          
        }
    }
}
