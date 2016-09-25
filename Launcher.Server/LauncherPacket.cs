using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Server
{
    public class LauncherPacket
    {
        private List<byte> TempBuffer;
        private object syncLock = new object();

        public byte[] Buffer
        {
            get { lock (syncLock) return TempBuffer.ToArray(); }
            set { lock (syncLock) TempBuffer = new List<byte>(value); }
        }
        public int ID { get; set; }
        public int Position { get; set; }

        public LauncherPacket(int ID)
        {
            this.ID = ID;
            Position = 0;
            TempBuffer = new List<byte>();
        }

        public LauncherPacket(int ID, byte[] Buffer) : this(ID)
        {
            this.Buffer = Buffer;
        }

        public void Clear()
        {
            this.TempBuffer.Clear();
            this.Position = 0;
        }

        public byte[] ReadBuffer(int Size)
        {
            byte[] Buffer = TempBuffer.Skip(Position).Take(Size).ToArray();

            Position += Size;
            return Buffer;
        }

        public byte ReadByte()
        {
            byte[] Buffer = ReadBuffer(1);
            return Buffer[0];
        }

        public bool ReadBool()
        {
            return ReadByte() == 0x01;
        }

        public char ReadChar()
        {
            byte[] Buffer = ReadBuffer(sizeof(char));
            return BitConverter.ToChar(Buffer, 0);
        }

        public short ReadShort()
        {
            byte[] Buffer = ReadBuffer(sizeof(short));
            return BitConverter.ToInt16(Buffer, 0);
        }

        public int ReadInt()
        {
            byte[] Buffer = ReadBuffer(sizeof(int));
            return BitConverter.ToInt32(Buffer, 0);
        }

        public long ReadLong()
        {
            byte[] Buffer = ReadBuffer(sizeof(long));
            return BitConverter.ToInt64(Buffer, 0);
        }

        public string ReadString()
        {
            int Length = ReadInt();
            byte[] Buffer = ReadBuffer(Length);

            return Encoding.UTF8.GetString(Buffer);
        }

        public ushort ReadUShort()
        {
            byte[] Buffer = ReadBuffer(sizeof(ushort));
            return BitConverter.ToUInt16(Buffer, 0);
        }

        public uint ReadUInt()
        {
            byte[] Buffer = ReadBuffer(sizeof(uint));
            return BitConverter.ToUInt32(Buffer, 0);
        }

        public ulong ReadULong()
        {
            byte[] Buffer = ReadBuffer(sizeof(ulong));
            return BitConverter.ToUInt64(Buffer, 0);
        }

        public void WriteBuffer(byte[] Buffer)
        {
            TempBuffer.AddRange(Buffer);
            Position += Buffer.Length;
        }

        public void WriteByte(byte Value)
        {
            WriteBuffer(new byte[1] { Value });
        }

        public void WriteBool(bool Value)
        {
            if (Value)
                WriteByte(0x01);
            else
                WriteByte(0x00);
        }

        public void WriteChar(char Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            WriteBuffer(Buffer);
        }

        public void WriteShort(short Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            WriteBuffer(Buffer);
        }

        public void WriteInt(int Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            WriteBuffer(Buffer);
        }

        public void WriteLong(long Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            WriteBuffer(Buffer);
        }

        public void WriteString(string Value)
        {
            byte[] Buffer = Encoding.UTF8.GetBytes(Value);

            WriteInt(Buffer.Length);
            WriteBuffer(Buffer);
        }

        public void WriteUShort(ushort Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            WriteBuffer(Buffer);
        }

        public void WriteUInt(uint Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            WriteBuffer(Buffer);
        }

        public void WriteULong(ulong Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            WriteBuffer(Buffer);
        }
    }
}
