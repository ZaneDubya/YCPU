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

        public bool IsSelected
        {
            get
            {
                return Entity.Serial == SpaceModel.SelectedSerial;
            }
        }

        public bool IsVisible
        {
            get
            {
                return true;
            }
        }

        protected float ViewSize = 1f;
        protected Vector3[] DrawVertices = null;
        protected Color DrawColor = Color.White;
        protected Matrix DrawMatrix = Matrix.Identity;

        public AEntitySpaceComponent(AEntity entity)
            : base(entity)
        {
            Position = Position3D.Zero;
        }

        public void Draw(VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Vector3[] v = new Vector3[DrawVertices.Length];
            for (int i = 0; i < v.Length; i++)
                v[i] = Vector3.Transform(DrawVertices[i] * ViewSize, DrawMatrix);
            renderer.DrawPolygon(v, true, DrawColor, false);

            mouseOverList.AddEntityIfMouseIsOver(Entity, DrawMatrix.Translation);

            if (IsSelected)
            {
                Matrix drawMatrixNoRotation = Matrix.Identity;
                drawMatrixNoRotation.Translation = DrawMatrix.Translation;
                Vector3[] selection = new Vector3[Vertices.SelectionLeft.Length];
                for (int i = 0; i < selection.Length; i++)
                    selection[i] = Vector3.Transform(Vertices.SelectionLeft[i] * ViewSize, drawMatrixNoRotation);
                renderer.DrawPolygon(selection, false, Color.Yellow, false);
                for (int i = 0; i < selection.Length; i++)
                    selection[i] = Vector3.Transform(Vertices.SelectionRight[i] * ViewSize, drawMatrixNoRotation);
                renderer.DrawPolygon(selection, false, Color.Yellow, false);
            }
        }
    }
}
