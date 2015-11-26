#region File Description
//-----------------------------------------------------------------------------
// LineBatch.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ypsilon.Core.Graphics
{
    /// <summary>
    /// Batches line "draw" calls from the game, and renders them at one time.
    /// </summary>
    public class VectorRenderer
    {
        GraphicsDevice m_Graphics;
        Effect m_Effect;

        private const int c_MaxPrimitives = 0x1000;
        private VertexPositionColorTexture[] m_WorldVertices, m_ScreenVertices;
        private Texture2D m_Texture;
        private short[] m_Indices;
        private int m_CurrentIndex;
        private int m_LineCount;

        public VectorRenderer(GraphicsDevice g, ContentManager c)
        {
            m_Graphics = g;
            m_Effect = c.Load<Effect>("Basic");

            Color[] data = new Color[] { Color.White };
            m_Texture = new Texture2D(m_Graphics, 1, 1);
            m_Texture.SetData<Color>(data);

            // create the vertex and indices array
            m_WorldVertices = new VertexPositionColorTexture[c_MaxPrimitives * 2];
            m_ScreenVertices = new VertexPositionColorTexture[c_MaxPrimitives * 2];
            m_Indices = CreateIndexBuffer(c_MaxPrimitives);
            m_CurrentIndex = 0;
            m_LineCount = 0;
        }

        private short[] CreateIndexBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 2];
            for (int i = 0; i < primitiveCount; i++)
            {
                indices[i * 2] = (short)(i * 2);
                indices[i * 2 + 1] = (short)(i * 2 + 1);
            }
            return indices;
        }

        /// <summary>
        /// Draw a line from one point to another with the same color.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="color">The color throughout the line.</param>
        public void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            DrawLine(
                new VertexPositionColorTexture(start, color, new Vector2()),
                new VertexPositionColorTexture(end, color, new Vector2()));
        }


        /// <summary>
        /// Draw a line from one point to another with different colors at each end.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="startColor">The color at the starting point.</param>
        /// <param name="endColor">The color at the ending point.</param>
        public void DrawLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
        {
            DrawLine(
                new VertexPositionColorTexture(start, startColor, new Vector2()),
                new VertexPositionColorTexture(end, endColor, new Vector2()));
        }


        /// <summary>
        /// Draws a line from one vertex to another.
        /// </summary>
        /// <param name="start">The starting vertex.</param>
        /// <param name="end">The ending vertex.</param>
        public void DrawLine(VertexPositionColorTexture start, VertexPositionColorTexture end)
        {
            if (m_LineCount >= c_MaxPrimitives)
                throw new Exception("Raster graphics count has exceeded limit.");

            m_WorldVertices[m_CurrentIndex++] = start;
            m_WorldVertices[m_CurrentIndex++] = end;

            m_LineCount++;
        }

        /// <summary>
        /// Draws the given polygon.
        /// </summary>
        public void DrawPolygon(Vector3[] polygon, Color color, float drawSize, Matrix matrix, bool closePolygon)
        {
            if (polygon == null)
                return;

            int length = polygon.Length - (closePolygon ? 0 : 1);
            for (int i = 0; i < length; i++)
            {
                if (m_LineCount >= c_MaxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                m_WorldVertices[m_CurrentIndex].Position = Vector3.Transform(polygon[i % polygon.Length] * drawSize, matrix);
                m_WorldVertices[m_CurrentIndex++].Color = color;
                m_WorldVertices[m_CurrentIndex].Position = Vector3.Transform(polygon[(i + 1) % polygon.Length] * drawSize, matrix);
                m_WorldVertices[m_CurrentIndex++].Color = color;
                m_LineCount++;
            }
        }

        public void DrawPolygon(VertexPositionColorTexture[] polygon, Vector3 translation, bool closePolygon)
        {
            if (polygon == null)
                return;

            int length = polygon.Length - (closePolygon ? 0 : 1);
            for (int i = 0; i < length; i++)
            {
                if (m_LineCount >= c_MaxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                m_WorldVertices[m_CurrentIndex] = polygon[i % polygon.Length];
                m_WorldVertices[m_CurrentIndex++].Position += translation;
                m_WorldVertices[m_CurrentIndex] = polygon[(i + 1) % polygon.Length];
                m_WorldVertices[m_CurrentIndex++].Position += translation;
                m_LineCount++;
            }
        }

        public void Render(Matrix projection, Matrix view, Matrix world)
        {
            // if we don't have any vertices, then we can exit early
            if (m_CurrentIndex == 0)
                return;

            m_Graphics.BlendState = BlendState.AlphaBlend;
            m_Graphics.DepthStencilState = DepthStencilState.Default;
            m_Graphics.SamplerStates[0] = SamplerState.PointClamp;
            m_Graphics.RasterizerState = RasterizerState.CullNone;

            m_Effect.Parameters["ProjectionMatrix"].SetValue(projection);
            m_Effect.Parameters["ViewMatrix"].SetValue(view);
            m_Effect.Parameters["WorldMatrix"].SetValue(world);
            m_Effect.Parameters["Viewport"].SetValue(new Vector2(m_Graphics.Viewport.Width, m_Graphics.Viewport.Height));

            m_Effect.CurrentTechnique.Passes[0].Apply();

            m_Graphics.Textures[0] = m_Texture;
            m_Graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, m_WorldVertices, 0, m_CurrentIndex, m_Indices, 0, m_LineCount);

            m_CurrentIndex = 0;
            m_LineCount = 0;
        }

        public void Render_ViewportSpace()
        {
            // if we don't have any vertices, then we can exit early
            if (m_CurrentIndex == 0)
                return;

            m_Graphics.BlendState = BlendState.AlphaBlend;
            m_Graphics.DepthStencilState = DepthStencilState.Default;
            m_Graphics.SamplerStates[0] = SamplerState.PointClamp;
            m_Graphics.RasterizerState = RasterizerState.CullNone;

            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            m_Effect.Parameters["ProjectionMatrix"].SetValue(GraphicsUtility.ProjectionMatrixScreen);
            m_Effect.Parameters["ViewMatrix"].SetValue(view);
            m_Effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            m_Effect.Parameters["Viewport"].SetValue(new Vector2(m_Graphics.Viewport.Width, m_Graphics.Viewport.Height));

            m_Effect.CurrentTechnique.Passes[0].Apply();

            m_Graphics.Textures[0] = m_Texture;
            m_Graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, m_WorldVertices, 0, m_CurrentIndex, m_Indices, 0, m_LineCount);

            m_CurrentIndex = 0;
            m_LineCount = 0;
        }
    }
}
