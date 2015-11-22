using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.Modes.Space
{
    static class Messages
    {
        private static List<Tuple<MessageType, string>> m_Messages = new List<Tuple<MessageType, string>>();

        public static void Clear()
        {
            m_Messages.Clear();
        }

        public static void Add(MessageType msgType, string msg)
        {
            m_Messages.Add(new Tuple<MessageType, string>(msgType, msg));
        }

        public static bool Get(out MessageType msgType, out string msg)
        {
            if (m_Messages.Count == 0)
            {
                msgType = MessageType.None;
                msg = null;
                return false;
            }

            msgType = m_Messages[0].Item1;
            msg = m_Messages[0].Item2;
            m_Messages.RemoveAt(0);
            return true;
        }
    }

    enum MessageType
    {
        None,
        Error
    }
}
