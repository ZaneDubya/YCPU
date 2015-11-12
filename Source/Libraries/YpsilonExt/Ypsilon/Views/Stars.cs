using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ypsilon.Graphics;

namespace Ypsilon.Views
{
    class Stars
    {
        private Texture2D m_Texture;
        private Star[] m_Stars;
        private Rectangle m_Bounds;

        public Stars(GraphicsDevice graphics)
        {
            m_Texture = new Texture2D(graphics, 1, 1);
            m_Texture.SetData<Color>(new Color[1] { Color.White }); // white
        }

        public void CreateStars(int count, Rectangle bounds)
        {
            m_Stars = new Star[count];
            m_Bounds = bounds;

            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                m_Stars[i] = new Star(
                    new Vector2(
                        (float)rand.NextDouble() * bounds.Width, 
                        (float)rand.NextDouble() * bounds.Height),
                    (float)rand.NextDouble() * 3, 
                    new Color(
                        (float)rand.NextDouble() / 2 + 0.5f,
                        (float)rand.NextDouble() / 2 + 0.5f,
                        (float)rand.NextDouble() / 2 + 0.5f)
                    );
            }
        }

        public void Update(Vector3 frameMovement)
        {
            for (int i = 0; i < m_Stars.Length; i++)
                m_Stars[i].Position -= new Vector2(frameMovement.X, -frameMovement.Y) * m_Stars[i].MovementSpeed;
        }

        public void Draw(SpriteBatchExtended sb)
        {
            for (int i = 0; i < m_Stars.Length; i++)
                sb.DrawSprite(m_Texture, new Vector3(m_Stars[i].Position, 0f), Vector2.One, m_Stars[i].Color);
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
