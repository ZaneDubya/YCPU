using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics {
    /// <summary>
    /// A 3D tri renderer. Currently no support for textures, but could be extended.
    /// </summary>
    public class VectorRenderer {
        private const int c_MaxPrimitives = 0x1000;
        private readonly Effect m_Effect;
        private readonly GraphicsDevice m_Graphics;
        private readonly short[] m_TriIndices;
        private readonly VertexList m_WorldTris;

        public VectorRenderer(GraphicsDevice g, ContentManager c) {
            m_Graphics = g;
            m_Effect = c.Load<Effect>("SpriteEffect");

            // create vertex and index collections
            m_WorldTris = new VertexList(c_MaxPrimitives, 3);
            m_TriIndices = CreateTriBuffer(c_MaxPrimitives);
        }

        /// <summary>
        /// Draws the given tri list. This can be massively optimized, I'm sure.
        /// </summary>
        public void DrawTris(VertexPositionTextureDataColor[] polygon, ushort[] indexes, Matrix matrix, Color color, Vector4 data) {
            if (polygon == null)
                return;
            int count = indexes.Length / 3;
            Vector3 z = Depth.NextZ;
            for (int i = 0; i < count; i++) {
                if (m_WorldTris.Count >= c_MaxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit");
                m_WorldTris.Vertices[m_WorldTris.Index] = polygon[indexes[i * 3 + 0]];
                m_WorldTris.Vertices[m_WorldTris.Index].Data = data;
                m_WorldTris.Vertices[m_WorldTris.Index].Hue = color;
                m_WorldTris.Vertices[m_WorldTris.Index++].Position = Vector3.Transform(polygon[indexes[i * 3 + 0]].Position, matrix) + z;
                m_WorldTris.Vertices[m_WorldTris.Index] = polygon[indexes[i * 3 + 1]];
                m_WorldTris.Vertices[m_WorldTris.Index].Data = data;
                m_WorldTris.Vertices[m_WorldTris.Index].Hue = color;
                m_WorldTris.Vertices[m_WorldTris.Index++].Position = Vector3.Transform(polygon[indexes[i * 3 + 1]].Position, matrix) + z;
                m_WorldTris.Vertices[m_WorldTris.Index] = polygon[indexes[i * 3 + 2]];
                m_WorldTris.Vertices[m_WorldTris.Index].Data = data;
                m_WorldTris.Vertices[m_WorldTris.Index].Hue = color;
                m_WorldTris.Vertices[m_WorldTris.Index++].Position = Vector3.Transform(polygon[indexes[i * 3 + 2]].Position, matrix) + z;
                m_WorldTris.Count++;
            }
        }

        public void Render(Matrix projection, Matrix view, Matrix world, Texture2D texture) {
            // set up graphics state
            m_Graphics.BlendState = BlendState.AlphaBlend;
            m_Graphics.DepthStencilState = DepthStencilState.Default;
            m_Graphics.SamplerStates[0] = SamplerState.PointClamp;
            m_Graphics.RasterizerState = new RasterizerState {
                ScissorTestEnable = true,
                CullMode = CullMode.None
            }; // RasterizerState.CullNone;
            m_Graphics.Textures[0] = texture;
            // set up effect state
            m_Effect.Parameters["ProjectionMatrix"].SetValue(projection);
            m_Effect.Parameters["ViewMatrix"].SetValue(view);
            m_Effect.Parameters["WorldMatrix"].SetValue(world);
            m_Effect.Parameters["Viewport"].SetValue(new Vector2(m_Graphics.Viewport.Width, m_Graphics.Viewport.Height));
            m_Effect.CurrentTechnique.Passes[0].Apply();
            if (m_WorldTris.Count <= 0)
                return;
            m_Graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, m_WorldTris.Vertices, 0, m_WorldTris.Index, m_TriIndices, 0, m_WorldTris.Count);
            m_WorldTris.Reset();
        }

        private static short[] CreateTriBuffer(int primitiveCount) {
            short[] indices = new short[primitiveCount * 3];
            for (int i = 0; i < primitiveCount; i++) {
                indices[i * 3] = (short)(i * 3);
                indices[i * 3 + 1] = (short)(i * 3 + 1);
                indices[i * 3 + 2] = (short)(i * 3 + 2);
            }
            return indices;
        }
    }
}