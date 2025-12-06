using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Extensions {
    public class TypedEnumerator<T> where T : class {
        IEnumerator m_Enumerator;

        public TypedEnumerator(IEnumerator enumerator) {
            m_Enumerator = enumerator;
        }

        public T Current {
            get {
                if (CheckCurrentIsT())
                    return (T)m_Enumerator.Current;
                return null;
            }
        }

        bool CheckCurrentIsT() {
            if (!(m_Enumerator.Current is T))
                while (!(m_Enumerator.Current is T))
                    if (!m_Enumerator.MoveNext())
                        return false;
            return true;
        }

        public bool MoveNext() {
            if (!m_Enumerator.MoveNext())
                return false;
            return CheckCurrentIsT();
        }

        public void Reset() {
            m_Enumerator.Reset();
            m_Enumerator.MoveNext();
            CheckCurrentIsT();
        }
    }
}
