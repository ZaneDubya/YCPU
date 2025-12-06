using System;
using System.IO;
using System.Text;

namespace Ypsilon.Core.IO {
    public sealed class BinaryFileWriter {
        private const int LARGE_BYTE_BUFFER_SIZE = 256;
        private readonly bool m_PrefixStrings;

        private readonly byte[] m_Buffer;

        private readonly Encoding m_Encoding;
        private readonly Stream m_File;
        private readonly char[] m_SingleCharBuffer = new char[1];
        private byte[] m_CharacterBuffer;
        private int m_Index;
        private int m_MaxBufferChars;
        private long m_Position;

        public BinaryFileWriter(Stream strm, bool prefixStr) {
            m_PrefixStrings = prefixStr;
            m_Encoding = Encoding.UTF8;
            m_Buffer = new byte[BufferSize];
            m_File = strm;
        }

        public BinaryFileWriter(string filename, bool prefixStr) {
            m_PrefixStrings = prefixStr;
            m_Buffer = new byte[BufferSize];
            m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            m_Encoding = Encoding.UTF8;
        }

        private static int BufferSize => 64 * 1024;

        public long Position => m_Position + m_Index;

        public Stream UnderlyingStream {
            get {
                if (m_Index > 0) {
                    Flush();
                }

                return m_File;
            }
        }

        public void Flush() {
            if (m_Index > 0) {
                m_Position += m_Index;

                m_File.Write(m_Buffer, 0, m_Index);
                m_Index = 0;
            }
        }

        public void Close() {
            if (m_Index > 0) {
                Flush();
            }

            m_File.Close();
        }

        public void WriteEncodedInt(int value) {
            uint v = (uint)value;

            while (v >= 0x80) {
                if ((m_Index + 1) > m_Buffer.Length) {
                    Flush();
                }

                m_Buffer[m_Index++] = (byte)(v | 0x80);
                v >>= 7;
            }

            if ((m_Index + 1) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index++] = (byte)v;
        }

        private void InternalWriteString(string value) {
            if (value == null) {
                value = string.Empty;
            }
            int length = m_Encoding.GetByteCount(value);

            WriteEncodedInt(length);

            if (m_CharacterBuffer == null) {
                m_CharacterBuffer = new byte[LARGE_BYTE_BUFFER_SIZE];
                m_MaxBufferChars = LARGE_BYTE_BUFFER_SIZE / m_Encoding.GetMaxByteCount(1);
            }

            if (length > LARGE_BYTE_BUFFER_SIZE) {
                int current = 0;
                int charsLeft = value.Length;

                while (charsLeft > 0) {
                    int charCount = (charsLeft > m_MaxBufferChars) ? m_MaxBufferChars : charsLeft;
                    int byteLength = m_Encoding.GetBytes(value, current, charCount, m_CharacterBuffer, 0);

                    if ((m_Index + byteLength) > m_Buffer.Length) {
                        Flush();
                    }

                    Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
                    m_Index += byteLength;

                    current += charCount;
                    charsLeft -= charCount;
                }
            }
            else {
                int byteLength = m_Encoding.GetBytes(value, 0, value.Length, m_CharacterBuffer, 0);

                if ((m_Index + byteLength) > m_Buffer.Length) {
                    Flush();
                }

                Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
                m_Index += byteLength;
            }
        }

        public void Write(string value) {
            if (m_PrefixStrings) {
                if (value == null) {
                    if ((m_Index + 1) > m_Buffer.Length) {
                        Flush();
                    }

                    m_Buffer[m_Index++] = 0;
                }
                else {
                    if ((m_Index + 1) > m_Buffer.Length) {
                        Flush();
                    }

                    m_Buffer[m_Index++] = 1;

                    InternalWriteString(value);
                }
            }
            else {
                InternalWriteString(value);
            }
        }

        public void Write(DateTime value) {
            Write(value.Ticks);
        }

        public void WriteDeltaTime(DateTime value) {
            long ticks = value.Ticks;
            long now = DateTime.Now.Ticks;

            TimeSpan d;

            try {
                d = new TimeSpan(ticks - now);
            }
            catch {
                if (ticks < now) {
                    d = TimeSpan.MaxValue;
                }
                else {
                    d = TimeSpan.MaxValue;
                }
            }

            Write(d);
        }

        public void Write(TimeSpan value) {
            Write(value.Ticks);
        }

        public void Write(decimal value) {
            int[] bits = Decimal.GetBits(value);

            for (int i = 0; i < bits.Length; ++i) {
                Write(bits[i]);
            }
        }

        public void Write(long value) {
            if ((m_Index + 8) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Buffer[m_Index + 4] = (byte)(value >> 32);
            m_Buffer[m_Index + 5] = (byte)(value >> 40);
            m_Buffer[m_Index + 6] = (byte)(value >> 48);
            m_Buffer[m_Index + 7] = (byte)(value >> 56);
            m_Index += 8;
        }

        public void Write(ulong value) {
            if ((m_Index + 8) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Buffer[m_Index + 4] = (byte)(value >> 32);
            m_Buffer[m_Index + 5] = (byte)(value >> 40);
            m_Buffer[m_Index + 6] = (byte)(value >> 48);
            m_Buffer[m_Index + 7] = (byte)(value >> 56);
            m_Index += 8;
        }

        public void Write(int value) {
            if ((m_Index + 4) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Index += 4;
        }

        public void Write(uint value) {
            if ((m_Index + 4) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Index += 4;
        }

        public void Write(short value) {
            if ((m_Index + 2) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Index += 2;
        }

        public void Write(ushort value) {
            if ((m_Index + 2) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Index += 2;
        }

        public unsafe void Write(double value) {
            if ((m_Index + 8) > m_Buffer.Length) {
                Flush();
            }

            fixed (byte* pBuffer = m_Buffer) {
                *((double*)(pBuffer + m_Index)) = value;
            }

            m_Index += 8;
        }

        public unsafe void Write(float value) {
            if ((m_Index + 4) > m_Buffer.Length) {
                Flush();
            }

            fixed (byte* pBuffer = m_Buffer) {
                *((float*)(pBuffer + m_Index)) = value;
            }

            m_Index += 4;
        }

        public void Write(char value) {
            if ((m_Index + 8) > m_Buffer.Length) {
                Flush();
            }

            m_SingleCharBuffer[0] = value;

            int byteCount = m_Encoding.GetBytes(m_SingleCharBuffer, 0, 1, m_Buffer, m_Index);
            m_Index += byteCount;
        }

        public void Write(byte value) {
            if ((m_Index + 1) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index++] = value;
        }

        public void Write(byte[] value) {
            for (int i = 0; i < value.Length; i++) {
                Write(value[i]);
            }
        }

        public void Write(byte[] value, int offset, int count) {
            for (int i = 0; i < count; i++) {
                Write(value[i + offset]);
            }
        }

        public void Write(sbyte value) {
            if ((m_Index + 1) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index++] = (byte)value;
        }

        public void Write(bool value) {
            if ((m_Index + 1) > m_Buffer.Length) {
                Flush();
            }

            m_Buffer[m_Index++] = (byte)(value ? 1 : 0);
        }
    }
}
