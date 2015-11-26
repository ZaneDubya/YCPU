using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;

namespace Ypsilon.Modes.Space.Entities
{
    class AEntitySpaceComponent : AEntityComponent
    {
        public Position3D Position
        {
            get;
            set;
        }

        public bool IsVisible
        {
            get
            {
                return true;
            }
        }

        public float ViewSize = 1f;
        protected Vector3[] DrawVertices = null;
        protected Color DrawColor = Color.White;
        protected Matrix DrawMatrix = Matrix.Identity;

        public AEntitySpaceComponent(AEntity entity)
            : base(entity)
        {
            Position = Position3D.Zero;
        }

        public virtual void Draw(VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            renderer.DrawPolygon(DrawVertices, DrawColor, ViewSize, DrawMatrix, true);
            mouseOverList.AddEntityIfMouseIsOver(Entity, DrawMatrix.Translation);
        }

        public void DrawSelection(VectorRenderer renderer)
        {
            Matrix drawMatrixNoRotation = Matrix.Identity;
            drawMatrixNoRotation.Translation = DrawMatrix.Translation;
            renderer.DrawPolygon(Vertices.SelectionLeft, Color.Yellow, ViewSize, drawMatrixNoRotation, false);
            renderer.DrawPolygon(Vertices.SelectionRight, Color.Yellow, ViewSize, drawMatrixNoRotation, false);
        }
    }
}
