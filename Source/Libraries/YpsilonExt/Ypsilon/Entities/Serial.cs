using System;

namespace Ypsilon.Entities
{
    public struct Serial : IComparable, IComparable<Serial>
    {
        private static int m_NextSerial = Serial.First;

        public static Serial GetNext()
        {
            return m_NextSerial++;
        }

        public static Serial Null
        {
            get
            {
                return 0;
            }
        }

        public static Serial First
        {
            get
            {
                return 1;
            }
        }

        private int m_Serial;

        private Serial(int serial)
        {
            m_Serial = serial;
        }

        public int Value
        {
            get
            {
                return m_Serial;
            }
        }

        public bool IsValid
        {
            get
            {
                return (m_Serial > 0);
            }
        }

        public override int GetHashCode()
        {
            return m_Serial;
        }

        public int CompareTo(Serial other)
        {
            return m_Serial.CompareTo(other.m_Serial);
        }

        public int CompareTo(object other)
        {
            if (other is Serial)
                return this.CompareTo((Serial)other);
            else if (other == null)
                return -1;

            throw new ArgumentException();
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Serial)) return false;

            return ((Serial)o).m_Serial == m_Serial;
        }

        public static bool operator ==(Serial l, Serial r)
        {
            return l.m_Serial == r.m_Serial;
        }

        public static bool operator !=(Serial l, Serial r)
        {
            return l.m_Serial != r.m_Serial;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}", m_Serial);
        }

        public static implicit operator int (Serial a)
        {
            return a.m_Serial;
        }

        public static implicit operator Serial(int a)
        {
            return new Serial(a);
        }
    }
}
