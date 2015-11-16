using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ypsilon.Core.Graphics
{
    class VertexList
    {
        private VertexDeclaration _decl;
        private GraphicsDevice _device;
        private VertexPositionNormalTexture[] _vertices;
        private short[] _indices;
        private int _count;
        private int _index;

        public VertexList()
        {
            _index = 0;
        }

        public void Reset(GraphicsDevice device)
        {
            _device = device;
        }

        public void SetMaxCount(int count)
        {
            _indices = new short[count * 3];
            for (int i = 0; i < count * 3; i++)
                _indices[i] = (short)i;
            _vertices = new VertexPositionNormalTexture[count * 3];
            _index = 0;
        }

        public void AddTri(Vector3 v0, Vector3 v1, Vector3 v2, int texture)
        {
            Vector3 normal = v0;
            normal.Normalize();

            _vertices[_index++] = new VertexPositionNormalTexture(v0, normal, new Vector2(texture, 0));
            _vertices[_index++] = new VertexPositionNormalTexture(v1, normal, new Vector2(texture, 0));
            _vertices[_index++] = new VertexPositionNormalTexture(v2, normal, new Vector2(texture, 0));
            _count++;
        }

        public void Dispose()
        {
            if (_decl != null)
            {
                _decl.Dispose();
                _decl = null;
            }
        }

        public void Render()
        {
            _device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, _vertices, 0, _count * 3, _indices, 0, _count);
        }
    }
}
