using Microsoft.Xna.Framework;

namespace Ypsilon.Platform.Support
{
    internal class FPS
    {
        // Maintain an accurate count of frames per second.
        float m_FPS = 0, m_Frames = 0, m_ElapsedSeconds = 0;
        public int CurrentFPS { get { return (int)(System.Math.Round(m_FPS)); } }
        public bool Update(GameTime gameTime)
        {
            m_Frames++;
            m_ElapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (m_ElapsedSeconds >= 1.0f)
            {
                m_FPS = m_Frames / m_ElapsedSeconds;
                m_ElapsedSeconds = 0;
                m_Frames = 0;
                return true;
            }
            return false;
        }

        int m_DesiredFPS = 60;
        public int DesiredFPS
        {
            get { return m_DesiredFPS; }
            set { m_DesiredFPS = value; }
        }
        public bool LimitFPS = true;
    }
}
