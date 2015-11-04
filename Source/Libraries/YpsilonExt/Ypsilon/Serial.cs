using System;

namespace Ypsilon
{
    public struct Serial : IComparable, IComparable<Serial>
    {
        private int _serial;

        public static Serial Null
        {
            get { return (Serial)0; }
        }

        private Serial(int serial)
        {
            _serial = serial;
        }

        public int Value
        {
            get
            {
                return _serial;
            }
        }

        public bool IsValid
        {
            get
            {
                return (_serial > 0);
            }
        }

        public override int GetHashCode()
        {
            return _serial;
        }

        public int CompareTo(Serial other)
        {
            return _serial.CompareTo(other._serial);
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

            return ((Serial)o)._serial == _serial;
        }

        public static bool operator ==(Serial l, Serial r)
        {
            return l._serial == r._serial;
        }

        public static bool operator !=(Serial l, Serial r)
        {
            return l._serial != r._serial;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}", _serial);
        }

        public static implicit operator int (Serial a)
        {
            return a._serial;
        }

        public static implicit operator Serial(int a)
        {
            return new Serial(a);
        }
    }
}
