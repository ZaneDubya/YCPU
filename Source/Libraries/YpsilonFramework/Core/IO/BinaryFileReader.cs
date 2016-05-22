using System;
using System.IO;
using System.Net;
using System.Text;

namespace Ypsilon.Core.IO {
    public sealed class BinaryFileReader {
        private readonly BinaryReader m_File;

        public BinaryFileReader(string path) {
            m_File = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
        }

        public BinaryFileReader(MemoryStream stream) {
            m_File = new BinaryReader(stream);
        }

        public BinaryFileReader(BinaryReader br) {
            m_File = br;
        }

        public long Position {
            get { return m_File.BaseStream.Position; }
            set { m_File.BaseStream.Position = value; }
        }

        public Stream Stream {
            get { return m_File.BaseStream; }
        }

        public void Close() {
            m_File.Close();
        }

        public long Seek(long offset, SeekOrigin origin) {
            return m_File.BaseStream.Seek(offset, origin);
        }

        public string ReadLine() {
            StringBuilder sb = new StringBuilder();
            bool reading = true;
            while (reading && !End()) {
                char c = this.ReadChar();
                if (c == '\n') {
                    reading = false;
                }
                else if (c == '\r') {
                    // discard
                }
                else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string ReadString() {
            return m_File.ReadString();
        }

        public string ReadStringNullTerminated() {
            StringBuilder sb = new StringBuilder();
            byte b;
            while ((b = ReadByte()) != 0) {
                sb.Append((char)b);
            }
            return sb.ToString();
        }

        public string ReadStringKnownLength(int length) {
            byte[] data = ReadBytes(length);
            return Encoding.ASCII.GetString(data);
        }

        public DateTime ReadDeltaTime() {
            long ticks = m_File.ReadInt64();
            long now = DateTime.Now.Ticks;

            if (ticks > 0 && (ticks + now) < 0) {
                return DateTime.MaxValue;
            }
            if (ticks < 0 && (ticks + now) < 0) {
                return DateTime.MinValue;
            }

            try {
                return new DateTime(now + ticks);
            }
            catch {
                if (ticks > 0) {
                    return DateTime.MaxValue;
                }
                return DateTime.MinValue;
            }
        }

        public IPAddress ReadIPAddress() {
            return new IPAddress(m_File.ReadInt64());
        }

        public int ReadEncodedInt() {
            int v = 0, shift = 0;
            byte b;

            do {
                b = m_File.ReadByte();
                v |= (b & 0x7F) << shift;
                shift += 7;
            } while (b >= 0x80);

            return v;
        }

        public DateTime ReadDateTime() {
            return new DateTime(m_File.ReadInt64());
        }

        public TimeSpan ReadTimeSpan() {
            return new TimeSpan(m_File.ReadInt64());
        }

        public decimal ReadDecimal() {
            return m_File.ReadDecimal();
        }

        public long ReadLong() {
            return m_File.ReadInt64();
        }

        public ulong ReadULong() {
            return m_File.ReadUInt64();
        }

        public int ReadInt() {
            return m_File.ReadInt32();
        }

        public uint ReadUInt() {
            return m_File.ReadUInt32();
        }

        public uint ReadUInt_BigEndian() {
            byte[] data = m_File.ReadBytes(4);
            return (uint)(data[3] + (data[2] << 8) + (data[1] << 16) + (data[0] << 24));
        }

        public ushort ReadUShort_BigEndian() {
            byte[] data = m_File.ReadBytes(2);
            return (ushort)((data[1]) + (data[0] << 8));
        }

        public short ReadShort() {
            return m_File.ReadInt16();
        }

        public ushort ReadUShort() {
            return m_File.ReadUInt16();
        }

        public double ReadDouble() {
            return m_File.ReadDouble();
        }

        public float ReadFloat() {
            return m_File.ReadSingle();
        }

        public char ReadChar() {
            return m_File.ReadChar();
        }

        public byte ReadByte() {
            return m_File.ReadByte();
        }

        public byte[] ReadBytes(int count) {
            return m_File.ReadBytes(count);
        }

        public ushort[] ReadUShorts(int count) {
            byte[] data = ReadBytes(count * 2);
            ushort[] data_out = new ushort[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 2);
            return data_out;
        }

        public int[] ReadInts(int count) {
            byte[] data = ReadBytes(count * 4);
            int[] data_out = new int[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 4);
            return data_out;
        }

        public uint[] ReadUInts(int count) {
            byte[] data = ReadBytes(count * 4);
            uint[] data_out = new uint[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 4);
            return data_out;
        }

        public int Read7BitEncodedInt() {
            int value = 0;
            int shift = 0;
            while (true) {
                byte temp = ReadByte();
                value += (temp & 0x7F) << shift;
                if ((temp & 0x80) == 0x80) {
                    shift += 7;
                }
                else {
                    return value;
                }
            }
        }

        /// <summary>
        /// WARNING: INCOMPLETE, ONLY READS 2-byte UTF8 chars.
        /// </summary>
        /// <returns></returns>
        public char ReadCharUTF8() {
            int value = 0;
            byte b0 = ReadByte();
            if ((b0 & 0x80) == 0x00) {
                value = (b0 & 0x7F);
            }
            else {
                value = (b0 & 0x3F);
                byte b1 = ReadByte();
                if ((b1 & 0xE0) == 0xC0) {
                    value += (b1 & 0x1F) << 6;
                }
            }
            return (char)value;
        }

        public sbyte ReadSByte() {
            return m_File.ReadSByte();
        }

        public bool ReadBool() {
            return m_File.ReadBoolean();
        }

        public bool End() {
            return m_File.PeekChar() == -1;
        }
    }
}
