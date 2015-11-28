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
        private VertexPositionNormalTextureData[] m_WorldVertices, m_ScreenVertices;
        private Texture2D m_Texture;
        private short[] m_Indices;
        private int m_CurrentIndex;
        private int m_LineCount;

        public VectorRenderer(GraphicsDevice g, ContentManager c)
        {
            m_Graphics = g;
            m_Effect = c.Load<Effect>("DataEffect");

            Color[] data = new Color[] { Color.White };
            m_Texture = new Texture2D(m_Graphics, 1, 1);
            m_Texture.SetData<Color>(data);

            // create the vertex and indices array
            m_WorldVertices = new VertexPositionNormalTextureData[c_MaxPrimitives * 2];
            m_ScreenVertices = new VertexPositionNormalTextureData[c_MaxPrimitives * 2];
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
        /// Draws the given polygon.
        /// </summary>
        public void DrawPolygon(Vector3[] polygon, Matrix matrix, Color color, bool closePolygon)
        {
            if (polygon == null)
                return;

            int length = polygon.Length - (closePolygon ? 0 : 1);
            for (int i = 0; i < length; i++)
            {
                if (m_LineCount >= c_MaxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                m_WorldVertices[m_CurrentIndex].Position = Vector3.Transform(polygon[i % polygon.Length], matrix);
                m_WorldVertices[m_CurrentIndex++].Data = color.ToVector4();
                m_WorldVertices[m_CurrentIndex].Position = Vector3.Transform(polygon[(i + 1) % polygon.Length], matrix);
                m_WorldVertices[m_CurrentIndex++].Data = color.ToVector4();
                m_LineCount++;
            }
        }

        public void DrawPolygon(VertexPositionNormalTextureData[] polygon, Vector3 translation, bool closePolygon)
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
            m_Graphics.DrawUserIndexedPrimitives<VertexPositionNormalTextureData>(PrimitiveType.LineList, m_WorldVertices, 0, m_CurrentIndex, m_Indices, 0, m_LineCount);

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
            m_Graphics.DrawUserIndexedPrimitives<VertexPositionNormalTextureData>(PrimitiveType.LineList, m_WorldVertices, 0, m_CurrentIndex, m_Indices, 0, m_LineCount);

            m_CurrentIndex = 0;
            m_LineCount = 0;
        }
    }
}
