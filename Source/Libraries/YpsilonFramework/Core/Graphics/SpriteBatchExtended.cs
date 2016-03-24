using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics
{
    public class SpriteBatchExtended
    {
        private Game m_Game;
        private GraphicsDevice m_GraphicsDevice;
        private Effect m_BasicEffect, m_CRTEffect;
        private Texture2D m_Pixel;

        private Dictionary<Texture2D, List<VertexPositionTextureDataColor>> m_drawQueue;
        private Queue<List<VertexPositionTextureDataColor>> m_vertexListQueue;
        private short[] m_indexBuffer;

        private Vector3 m_zOffset;
        public float ZOffset { set { m_zOffset = new Vector3(0, 0, value); } }

        public GraphicsDevice Graphics => m_GraphicsDevice;

        public SpriteBatchExtended(Game game)
        {
            m_Game = game;
        }

        public void Initialize()
        {
            m_GraphicsDevice = m_Game.GraphicsDevice;
            m_BasicEffect = m_Game.Content.Load<Effect>("BasicEffect");
            m_CRTEffect = m_Game.Content.Load<Effect>("CRTEffect");
            m_drawQueue = new Dictionary<Texture2D, List<VertexPositionTextureDataColor>>(256);
            m_indexBuffer = createIndexBuffer(0x2000);
            m_vertexListQueue = new Queue<List<VertexPositionTextureDataColor>>(256);
        }

        private short[] createIndexBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 6];

            for (int i = 0; i < primitiveCount; i++)
            {
                indices[i * 6] = (short)(i * 4);
                indices[i * 6 + 1] = (short)(i * 4 + 1);
                indices[i * 6 + 2] = (short)(i * 4 + 2);
                indices[i * 6 + 3] = (short)(i * 4 + 2);
                indices[i * 6 + 4] = (short)(i * 4 + 1);
                indices[i * 6 + 5] = (short)(i * 4 + 3);
            }

            return indices;
        }

        public Texture2D NewTexture(int width, int height)
        {
            return new Texture2D(m_GraphicsDevice, width, height);
        }

        public void Begin(Color clear)
        {
            m_GraphicsDevice.Clear(clear);
        }

        public void End(Effects effect)
        {
            Effect fx;
            SamplerState sample;

            switch (effect)
            {
                case Effects.Basic:
                    fx = m_BasicEffect;
                    sample = SamplerState.LinearClamp;
                    break;
                case Effects.CRT:
                    fx = m_CRTEffect;
                    sample = SamplerState.AnisotropicClamp;
                    break;
                default:
                    return;
            }

            m_GraphicsDevice.BlendState = BlendState.AlphaBlend;
            m_GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            m_GraphicsDevice.SamplerStates[0] = sample;
            m_GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            Texture2D iTexture;
            List<VertexPositionTextureDataColor> iVertexList;

            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionTextureDataColor>>> keyValuePairs = m_drawQueue.GetEnumerator();

            fx.Parameters["ProjectionMatrix"].SetValue(GraphicsUtility.CreateProjectionMatrixScreenOffset(m_GraphicsDevice));
            fx.Parameters["ViewMatrix"].SetValue(Matrix.Identity);
            fx.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            fx.Parameters["Viewport"].SetValue(new Vector2(m_GraphicsDevice.Viewport.Width, m_GraphicsDevice.Viewport.Height));

            fx.CurrentTechnique.Passes[0].Apply();

            while (keyValuePairs.MoveNext())
            {
                iTexture = keyValuePairs.Current.Key;
                iVertexList = keyValuePairs.Current.Value;
                m_GraphicsDevice.Textures[0] = iTexture;
                m_GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, m_indexBuffer, 0, iVertexList.Count / 2);
                iVertexList.Clear();
                m_vertexListQueue.Enqueue(iVertexList);
            }

            m_drawQueue.Clear();
            m_GraphicsDevice.Textures[0] = null;
        }

        public void Dispose()
        {
            m_BasicEffect.Dispose();
            m_CRTEffect.Dispose();
            m_Pixel.Dispose();
        }


        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Color hue)
        {
            return DrawSprite(texture, position, area, new Vector4(0, 0, 1, 1), hue, Vector4.Zero);
        }

        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Vector4 uv, Color hue)
        {
            return DrawSprite(texture, position, area, uv, hue, Vector4.Zero);
        }

        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Vector4 uv, Color hue, Vector4 data)
        {
            List<VertexPositionTextureDataColor> vertexList;

            if (m_drawQueue.ContainsKey(texture))
            {
                vertexList = m_drawQueue[texture];
            }
            else
            {
                if (m_vertexListQueue.Count > 0)
                {
                    vertexList = m_vertexListQueue.Dequeue();

                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionTextureDataColor>(1024);
                }

                m_drawQueue.Add(texture, vertexList);
            }

            position += m_zOffset;

            PreTransformedQuad q = new PreTransformedQuad(position, area, uv, hue, data);

            for (int i = 0; i < q.Vertices.Length; i++)
            {
                vertexList.Add(q.Vertices[i]);
            }

            return true;
        }

        public void DrawRectangle(Vector3 position, Vector2 area, Color hue)
        {
            position += m_zOffset;

            DrawFilledRectangle(position, new Vector2(area.X, 1), hue);
            DrawFilledRectangle(position + new Vector3(0, area.Y - 1, 0), new Vector2(area.X, 1), hue);
            DrawFilledRectangle(position, new Vector2(1, area.Y), hue);
            DrawFilledRectangle(position + new Vector3(area.X - 1, 0, 0), new Vector2(1, area.Y), hue);

            /*Vectors.DrawPolygon(new VectorPolygon(new Vector3[] {
                position + new Vector3(0, 0, 0), 
                position + new Vector3(area.X, 0, 0),
                position + new Vector3(area.X, area.Y - 1, 0),
                position + new Vector3(0, area.Y - 1, 0) }, true), hue);*/
        }

        public void DrawFilledRectangle(Vector3 position, Vector2 area, Color hue)
        {
            if (m_Pixel == null)
            {
                m_Pixel = new Texture2D(m_GraphicsDevice, 1, 1);
                m_Pixel.SetData(new Color[] { Color.White });
            }
            DrawSprite(m_Pixel, position, area, hue);
        }
    }
}
