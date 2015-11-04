using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Display
{
    class Model
    {
        public Quaternion Rotation = Quaternion.Identity;
        public Position3D Position = Position3D.Zero;
        public VertexPositionColor[] Vertices;

        public Model()
        {
            SetUpVertices();
        }

        private void SetUpVertices()
        {
            Vertices = new VertexPositionColor[3];

            Vertices[0].Position = new Vector3(-0.5f, -0.5f, 0f);
            Vertices[0].Color = Color.Red;
            Vertices[1].Position = new Vector3(0, 0.5f, 0f);
            Vertices[1].Color = Color.Green;
            Vertices[2].Position = new Vector3(0.5f, -0.5f, 0f);
            Vertices[2].Color = Color.Yellow;
        }

        public Vector2[] Draw()
        {
            return null;
            /*return new Vector2[2] {
                Position.X, Position.Y*/
        }
    }
}
