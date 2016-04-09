using Microsoft.Xna.Framework;

namespace Ypsilon.Core
{
    public class RectangleF
    {
        private float m_X;
        private float m_Y;
        private float m_Width;
        private float m_Height;
        private float m_X2;
        private float m_Y2;

        public RectangleF()
        {

        }

        public Rectangle ToRectangle()
        {
            Rectangle myReturn = new Rectangle((int)m_X, (int)m_Y, (int)m_Width, (int)m_Height);
            return myReturn;
        }

        public RectangleF(float pX, float pY, float pWidth, float pHeight)
        {
            m_X = pX;
            m_Y = pY;
            m_Width = pWidth;
            m_Height = pHeight;
            m_X2 = pX + pWidth;
            m_Y2 = pY + pHeight;
        }

        public bool Contains(Vector2 pPoint)
        {
            if ((pPoint.X > m_X) && (pPoint.X < m_X2) && (pPoint.Y > m_Y) && (pPoint.Y < m_Y2))
            {
                return true;
            }
            return false;
        }

        public RectangleF Union(RectangleF rect1, RectangleF rect2)
        {
            RectangleF tempRect = new RectangleF();

            if (rect1.m_X < rect2.m_X)
            {
                tempRect.m_X = rect1.m_X;
            }
            else
            {
                tempRect.m_X = rect2.m_X;
            }

            if (rect1.m_X2 > rect2.m_X2)
            {
                tempRect.m_X2 = rect1.m_X2;
            }
            else
            {
                tempRect.m_X2 = rect2.m_X2;
            }

            tempRect.m_Width = tempRect.m_X2 - tempRect.m_X;


            if (rect1.m_Y < rect2.m_Y)
            {
                tempRect.m_Y = rect1.m_Y;
            }
            else
            {
                tempRect.m_Y = rect2.m_Y;
            }

            if (rect1.m_Y2 > rect2.m_Y2)
            {
                tempRect.m_Y2 = rect1.m_Y2;
            }
            else
            {
                tempRect.m_Y2 = rect2.m_Y2;
            }

            tempRect.m_Height = tempRect.m_Y2 - tempRect.m_Y;
            return tempRect;
        }
        public float X
        {
            get { return m_X; }
            set
            {
                m_X = value;
                m_X2 = m_X + m_Width;
            }
        }

        public float Y
        {
            get { return m_Y; }
            set
            {
                m_Y = value;
                m_Y2 = m_Y + m_Height;
            }
        }

        public float Width
        {
            get { return m_Width; }
            set
            {
                m_Width = value;
                m_X2 = m_X + m_Width;
            }
        }

        public float Height
        {
            get { return m_Height; }
            set
            {
                m_Height = value;
                m_Y2 = m_Y + m_Height;
            }
        }

        public float X2 => m_X2;

        public float Y2 => m_Y2;

        public RectangleF Duplicate()
        {
            RectangleF myReturn = new RectangleF(X, Y, Width, Height);
            return myReturn;
        }

        public static bool IsCollision(RectangleF r2, RectangleF r1)
        {
            bool myReturn = false;

            if ((r1.X + r1.Width >= r2.X && r1.Y + r1.Height >= r2.Y && r1.X <= r2.X + r2.Width && r1.Y <= r2.Y + r2.Height))
            {
                myReturn = true;
            }

            return myReturn;
        }
    }
}
