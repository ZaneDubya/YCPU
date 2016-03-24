using Microsoft.Xna.Framework;

namespace Ypsilon.Core
{
    public class RectangleF
    {
        protected float m_x = 0.0F;
        protected float m_y = 0.0F;
        protected float m_width = 0.0F;
        protected float m_height = 0.0F;
        protected float m_x2 = 0.0F;
        protected float m_y2 = 0.0F;

        public RectangleF()
        {

        }

        public Rectangle toRectangle()
        {
            Rectangle myReturn = new Rectangle((int)m_x, (int)m_y, (int)m_width, (int)m_height);
            return myReturn;
        }

        public RectangleF(float pX, float pY, float pWidth, float pHeight)
        {
            m_x = pX;
            m_y = pY;
            m_width = pWidth;
            m_height = pHeight;
            m_x2 = pX + pWidth;
            m_y2 = pY + pHeight;
        }

        public bool Contains(Vector2 pPoint)
        {
            if ((pPoint.X > m_x) && (pPoint.X < m_x2) && (pPoint.Y > m_y) && (pPoint.Y < m_y2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public RectangleF Union(RectangleF rect1, RectangleF rect2)
        {
            RectangleF tempRect = new RectangleF();

            if (rect1.m_x < rect2.m_x)
            {
                tempRect.m_x = rect1.m_x;
            }
            else
            {
                tempRect.m_x = rect2.m_x;
            }

            if (rect1.m_x2 > rect2.m_x2)
            {
                tempRect.m_x2 = rect1.m_x2;
            }
            else
            {
                tempRect.m_x2 = rect2.m_x2;
            }

            tempRect.m_width = tempRect.m_x2 - tempRect.m_x;


            if (rect1.m_y < rect2.m_y)
            {
                tempRect.m_y = rect1.m_y;
            }
            else
            {
                tempRect.m_y = rect2.m_y;
            }

            if (rect1.m_y2 > rect2.m_y2)
            {
                tempRect.m_y2 = rect1.m_y2;
            }
            else
            {
                tempRect.m_y2 = rect2.m_y2;
            }

            tempRect.m_height = tempRect.m_y2 - tempRect.m_y;
            return tempRect;
        }
        public float X
        {
            get { return m_x; }
            set
            {
                m_x = value;
                m_x2 = m_x + m_width;
            }
        }

        public float Y
        {
            get { return m_y; }
            set
            {
                m_y = value;
                m_y2 = m_y + m_height;
            }
        }

        public float Width
        {
            get { return m_width; }
            set
            {
                m_width = value;
                m_x2 = m_x + m_width;
            }
        }

        public float Height
        {
            get { return m_height; }
            set
            {
                m_height = value;
                m_y2 = m_y + m_height;
            }
        }

        public float X2
        {
            get { return m_x2; }
        }

        public float Y2
        {
            get { return m_y2; }
        }

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
