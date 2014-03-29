using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace YCPU.Library.Support
{
    class Settings
    {
        private List<Setting> m_Updates = new List<Setting>();
        private bool m_Squelch = false;
        private Point m_Resolution = new Point(640, 480);

        public bool HasUpdates
        {
            get
            {
                if (m_Updates.Count > 0)
                    return true;
                return false;
            }
        }

        public Setting NextUpdate()
        {
            if (m_Updates.Count > 0)
            {
                Setting s = m_Updates[0];
                m_Updates.RemoveAt(0);
                return s;
            }
            else
            {
                return Setting.None;
            }
        }

        public void ClearUpdates()
        {
            m_Updates.Clear();
        }

        public bool SquelchUpdates
        {
            get { return m_Squelch; }
            set { m_Squelch = value; }
        }

        private void addUpdate(Setting setting)
        {
            if (!m_Squelch && !m_Updates.Contains(setting))
                m_Updates.Add(setting);
        }

        public Point Resolution
        {
            get { return m_Resolution; }
            set { m_Resolution = value; addUpdate(Setting.Resolution); }
        }

        public enum Setting
        {
            None,
            Resolution
        }

        public static float SecondsForDoubleClick = 0.5f;
    }
}
