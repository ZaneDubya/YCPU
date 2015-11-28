/***************************************************************************
 *   VertexPositionNormalTextureData.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics
{
    public struct VertexPositionNormalTextureData : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public Vector4 Data;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), // position
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), // normal
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), // tex coord
            new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1) // hue
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public VertexPositionNormalTextureData(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
            Data = Vector4.Zero;
        }

        public static readonly VertexPositionNormalTextureData[] PolyBuffer = {
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 1))
            };

        public static readonly VertexPositionNormalTextureData[] PolyBufferFlipped = {
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new VertexPositionNormalTextureData(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 1))
            };

        public static int SizeInBytes { get { return sizeof( float ) * 12; } }

        public override string ToString()
        {
            return string.Format("VPNTD: <{0}> <{1}>", Position.ToString(), UV.ToString());
        }
    }
}