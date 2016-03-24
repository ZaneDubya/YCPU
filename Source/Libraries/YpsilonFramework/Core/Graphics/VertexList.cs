﻿namespace Ypsilon.Core.Graphics
{
    internal class VertexList
    {
        public VertexPositionTextureDataColor[] Vertices;
        public int Index;
        public int Count;

        public VertexList(int maxPrimitives, int perPrimIndex)
        {
            Vertices = new VertexPositionTextureDataColor[maxPrimitives * perPrimIndex];
            Index = 0;
            Count = 0;
        }

        public void Reset()
        {
            Index = 0;
            Count = 0;
        }

        public int SizeOf => sizeof(int) * 2 + VertexPositionTextureDataColor.SizeInBytes * Vertices.Length;
    }
}