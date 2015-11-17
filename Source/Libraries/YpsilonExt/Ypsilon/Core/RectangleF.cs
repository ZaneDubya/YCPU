using Microsoft.Xna.Framework;

namespace Ypsilon.Core
{
    public class RectangleF
    {
        protected float _x = 0.0F;
        protected float _y = 0.0F;
        protected float _width = 0.0F;
        protected float _height = 0.0F;
        protected float _x2 = 0.0F;
        protected float _y2 = 0.0F;

        public RectangleF()
        {

        }

        public Rectangle toRectangle()
        {
            Rectangle myReturn = new Rectangle((int)_x, (int)_y, (int)_width, (int)_height);
            return myReturn;
        }

        public RectangleF(float pX, float pY, float pWidth, float pHeight)
        {
            _x = pX;
            _y = pY;
            _width = pWidth;
            _height = pHeight;
            _x2 = pX + pWidth;
            _y2 = pY + pHeight;
        }

        public bool Contains(Vector2 pPoint)
        {
            if ((pPoint.X > this._x) && (pPoint.X < this._x2) && (pPoint.Y > this._y) && (pPoint.Y < this._y2))
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

            if (rect1._x < rect2._x)
            {
                tempRect._x = rect1._x;
            }
            else
            {
                tempRect._x = rect2._x;
            }

            if (rect1._x2 > rect2._x2)
            {
                tempRect._x2 = rect1._x2;
            }
            else
            {
                tempRect._x2 = rect2._x2;
            }

            tempRect._width = tempRect._x2 - tempRect._x;


            if (rect1._y < rect2._y)
            {
                tempRect._y = rect1._y;
            }
            else
            {
                tempRect._y = rect2._y;
            }

            if (rect1._y2 > rect2._y2)
            {
                tempRect._y2 = rect1._y2;
            }
            else
            {
                tempRect._y2 = rect2._y2;
            }

            tempRect._height = tempRect._y2 - tempRect._y;
            return tempRect;
        }
        public float X
        {
            get { return _x; }
            set
            {
                _x = value;
                _x2 = _x + _width;
            }
        }

        public float Y
        {
            get { return _y; }
            set
            {
                _y = value;
                _y2 = _y + _height;
            }
        }

        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                _x2 = _x + _width;
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                _y2 = _y + _height;
            }
        }

        public float X2
        {
            get { return _x2; }
        }

        public float Y2
        {
            get { return _y2; }
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
