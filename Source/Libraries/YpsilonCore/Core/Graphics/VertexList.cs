﻿﻿namespace Ypsilon.Core.Graphics
{
    internal class VertexList
    {
        public VertexPositionNormalTextureData[] Vertices;
        public int Index;
        public int Count;

        public VertexList(int maxPrimitives, int perPrimIndex)
        {
            Vertices = new VertexPositionNormalTextureData[maxPrimitives * perPrimIndex];
            Index = 0;
            Count = 0;
        }

        public void Reset()
        {
            Index = 0;
            Count = 0;
        }

        public int SizeOf
        {
            get
            {
                return sizeof(int) * 2 + VertexPositionNormalTextureData.SizeInBytes * Vertices.Length;
            }
        }
    }
}