using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;

namespace Ypsilon.World.Views
{
    class Starfield
    {
        private Star[] m_Stars;
        private Rectangle m_Bounds;

        public Starfield()
        {

        }

        public void CreateStars(int count, Rectangle bounds)
        {
            m_Stars = new Star[count];
            m_Bounds = bounds;

            for (int i = 0; i < count; i++)
            {
                m_Stars[i] = new Star(
                    new Vector2(
                        (float)Utility.Random_GetNonpersistantDouble() * bounds.Width, 
                        (float)Utility.Random_GetNonpersistantDouble() * bounds.Height),
                    (float)Utility.Random_GetNonpersistantDouble() * 3 + 1, 
                    new Color(
                        (float)Utility.Random_GetNonpersistantDouble() / 2 + 0.5f,
                        (float)Utility.Random_GetNonpersistantDouble() / 2 + 0.5f,
                        (float)Utility.Random_GetNonpersistantDouble() / 2 + 0.5f)
                    );
            }
        }

        public void Update(Vector3 frameMovement)
        {
            for (int i = 0; i < m_Stars.Length; i++)
            {
                m_Stars[i].Position -= new Vector2(frameMovement.X, -frameMovement.Y) * m_Stars[i].MovementSpeed;
                if (!m_Bounds.Contains(new Point((int)m_Stars[i].Position.X, (int)m_Stars[i].Position.Y)))
                {
                    if (m_Stars[i].Position.X <= m_Bounds.Left)
                        m_Stars[i].Position.X += m_Bounds.Width;
                    if (m_Stars[i].Position.X >= m_Bounds.Right)
                        m_Stars[i].Position.X -= m_Bounds.Width;

                    if (m_Stars[i].Position.Y <= m_Bounds.Top)
                        m_Stars[i].Position.Y += m_Bounds.Height;
                    if (m_Stars[i].Position.Y >= m_Bounds.Bottom)
                        m_Stars[i].Position.Y -= m_Bounds.Height;
                }
            }
        }

        public void Draw(SpriteBatchExtended sb)
        {
            for (int i = 0; i < m_Stars.Length; i++)
                sb.DrawSprite(Data.Textures.Pixel, new Vector3(m_Stars[i].Position, 0f), Vector2.One, m_Stars[i].Color);
        }

        struct Star
        {
            public Vector2 Position;
            public float MovementSpeed;
            public float Unused;
            public Color Color;

            public Star(Vector2 pos, float spd, Color color)
            {
                Position = pos;
                MovementSpeed = spd;
                Unused = 0f;
                Color = color;
            }
        }
    }
}
