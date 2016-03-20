﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ypsilon.Core.Graphics
{
    /// <summary>
    /// A 3D line and tri renderer. Currently no support for textures, but could be extended.
    /// </summary>
    public class VectorRenderer
    {
        private GraphicsDevice m_Graphics;
        private Effect m_Effect;

        private const int c_MaxPrimitives = 0x1000;
        private VertexList m_WorldLines, m_WorldTris;
        private short[] m_LineIndices, m_TriIndices;

        public VectorRenderer(GraphicsDevice g, ContentManager c)
        {
            m_Graphics = g;
            m_Effect = c.Load<Effect>("DataEffect");

            // create vertex and index collections
            m_WorldLines = new VertexList(c_MaxPrimitives, 2);
            m_WorldTris = new VertexList(c_MaxPrimitives, 3);
            m_LineIndices = CreateLineBuffer(c_MaxPrimitives);
            m_TriIndices = CreateTriBuffer(c_MaxPrimitives);
        }

        private short[] CreateLineBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 2];
            for (int i = 0; i < primitiveCount; i++)
            {
                indices[i * 2] = (short)(i * 2);
                indices[i * 2 + 1] = (short)(i * 2 + 1);
            }
            return indices;
        }

        private short[] CreateTriBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 3];
            for (int i = 0; i < primitiveCount; i++)
            {
                indices[i * 3] = (short)(i * 3);
                indices[i * 3 + 1] = (short)(i * 3 + 1);
                indices[i * 3 + 2] = (short)(i * 3 + 2);
            }
            return indices;
        }

        /// <summary>
        /// Draws the given poly line (wireframe).
        /// </summary>
        public void DrawLines(Vector3[] polygon, Matrix matrix, Color color, bool closePolygon)
        {
            if (polygon == null)
                return;

            int length = polygon.Length - (closePolygon ? 0 : 1);
            for (int i = 0; i < length; i++)
            {
                if (m_WorldLines.Count >= c_MaxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                m_WorldLines.Vertices[m_WorldLines.Index].Position = Vector3.Transform(polygon[i % polygon.Length], matrix);
                m_WorldLines.Vertices[m_WorldLines.Index++].Hue = color;
                m_WorldLines.Vertices[m_WorldLines.Index].Position = Vector3.Transform(polygon[(i + 1) % polygon.Length], matrix);
                m_WorldLines.Vertices[m_WorldLines.Index++].Hue = color;
                m_WorldLines.Count++;
            }
        }

        /// <summary>
        /// Draws the given poly line (wireframe).
        /// </summary>
        public void DrawLines(VertexPositionTextureDataColor[] polygon, Vector3 translation, bool closePolygon)
        {
            if (polygon == null)
                return;

            int length = polygon.Length - (closePolygon ? 0 : 1);
            for (int i = 0; i < length; i++)
            {
                if (m_WorldLines.Count >= c_MaxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                m_WorldLines.Vertices[m_WorldLines.Index] = polygon[i % polygon.Length];
                m_WorldLines.Vertices[m_WorldLines.Index++].Position += translation;
                m_WorldLines.Vertices[m_WorldLines.Index] = polygon[(i + 1) % polygon.Length];
                m_WorldLines.Vertices[m_WorldLines.Index++].Position += translation;
                m_WorldLines.Count++;
            }
        }

        /// <summary>
        /// Draws the given tri list.
        /// </summary>
        public void DrawTris(Vector3[] polygon, Matrix matrix, Color color)
        {
            if (polygon == null)
                return;

            int count = polygon.Length / 3;
            for (int i = 0; i < count; i++)
            {
                if (m_WorldTris.Count >= c_MaxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                m_WorldTris.Vertices[m_WorldTris.Index].Position = Vector3.Transform(polygon[i * 3], matrix);
                m_WorldTris.Vertices[m_WorldTris.Index++].Hue = color;
                m_WorldTris.Vertices[m_WorldTris.Index].Position = Vector3.Transform(polygon[i * 3 + 1], matrix);
                m_WorldTris.Vertices[m_WorldTris.Index++].Hue = color;
                m_WorldTris.Vertices[m_WorldTris.Index].Position = Vector3.Transform(polygon[i * 3 + 2], matrix);
                m_WorldTris.Vertices[m_WorldTris.Index++].Hue = color;
                m_WorldTris.Count++;
            }
        }

        public void Render(Matrix projection, Matrix view, Matrix world, Texture2D texture)
        {
            m_Graphics.BlendState = BlendState.AlphaBlend;
            m_Graphics.DepthStencilState = DepthStencilState.Default;
            m_Graphics.SamplerStates[0] = SamplerState.PointClamp;
            m_Graphics.RasterizerState = RasterizerState.CullNone;

            m_Effect.Parameters["ProjectionMatrix"].SetValue(projection);
            m_Effect.Parameters["ViewMatrix"].SetValue(view);
            m_Effect.Parameters["WorldMatrix"].SetValue(world);
            m_Effect.Parameters["Viewport"].SetValue(new Vector2(m_Graphics.Viewport.Width, m_Graphics.Viewport.Height));

            m_Effect.CurrentTechnique.Passes[0].Apply();

            m_Graphics.Textures[0] = texture;

            if (m_WorldLines.Count > 0)
            {
                m_Graphics.DrawUserIndexedPrimitives<VertexPositionTextureDataColor>(PrimitiveType.LineList, m_WorldLines.Vertices, 0, m_WorldLines.Index, m_LineIndices, 0, m_WorldLines.Count);
                m_WorldLines.Reset();
            }
            if (m_WorldTris.Count > 0)
            {
                m_Graphics.DrawUserIndexedPrimitives<VertexPositionTextureDataColor>(PrimitiveType.TriangleList, m_WorldTris.Vertices, 0, m_WorldTris.Index, m_TriIndices, 0, m_WorldTris.Count);
                m_WorldTris.Reset();
            }
        }
    }
}