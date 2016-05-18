using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics {
    public class SpriteBatchExtended {
        public GraphicsDevice Graphics => m_GraphicsDevice;

        private readonly Game m_Game;
        private GraphicsDevice m_GraphicsDevice;
        private Effect m_BasicEffect, m_CRTEffect;
        private Texture2D m_Pixel;

        private Dictionary<Texture2D, List<VertexPositionTextureDataColor>> m_DrawQueue;
        private Queue<List<VertexPositionTextureDataColor>> m_VertexListQueue;
        private short[] m_IndexBuffer;

        public SpriteBatchExtended(Game game) {
            m_Game = game;
        }

        public void Initialize() {
            m_GraphicsDevice = m_Game.GraphicsDevice;
            m_BasicEffect = m_Game.Content.Load<Effect>("BasicEffect");
            m_CRTEffect = m_Game.Content.Load<Effect>("CRTEffect");
            m_DrawQueue = new Dictionary<Texture2D, List<VertexPositionTextureDataColor>>(256);
            m_IndexBuffer = CreateIndexBuffer(0x2000);
            m_VertexListQueue = new Queue<List<VertexPositionTextureDataColor>>(256);
        }

        private static short[] CreateIndexBuffer(int primitiveCount) {
            short[] indices = new short[primitiveCount * 6];

            for (int i = 0; i < primitiveCount; i++) {
                indices[i * 6] = (short)(i * 4);
                indices[i * 6 + 1] = (short)(i * 4 + 1);
                indices[i * 6 + 2] = (short)(i * 4 + 2);
                indices[i * 6 + 3] = (short)(i * 4 + 2);
                indices[i * 6 + 4] = (short)(i * 4 + 1);
                indices[i * 6 + 5] = (short)(i * 4 + 3);
            }

            return indices;
        }

        public Texture2D NewTexture(int width, int height) {
            return new Texture2D(m_GraphicsDevice, width, height);
        }

        public void SetScissorRect(Rectangle? r) {
            if (r == null)
                r = m_GraphicsDevice.Viewport.Bounds;
            m_GraphicsDevice.ScissorRectangle = r.Value;
        }

        public void Begin(Color? clear = null) {
            if (clear == null)
                return;
            m_GraphicsDevice.Clear(clear.Value);
        }

        public void End(Effects effect) {
            Effect fx;
            SamplerState sample;

            switch (effect) {
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
            m_GraphicsDevice.RasterizerState = new RasterizerState {
                ScissorTestEnable = true,
                CullMode = CullMode.None
            }; // RasterizerState.CullNone;

            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionTextureDataColor>>> keyValuePairs = m_DrawQueue.GetEnumerator();

            fx.Parameters["ProjectionMatrix"].SetValue(GraphicsUtility.CreateProjectionMatrixScreenOffset(m_GraphicsDevice));
            fx.Parameters["ViewMatrix"].SetValue(Matrix.Identity);
            fx.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            fx.Parameters["Viewport"].SetValue(new Vector2(m_GraphicsDevice.Viewport.Width, m_GraphicsDevice.Viewport.Height));

            fx.CurrentTechnique.Passes[0].Apply();

            while (keyValuePairs.MoveNext()) {
                List<VertexPositionTextureDataColor> iVertexList = keyValuePairs.Current.Value;
                m_GraphicsDevice.Textures[0] = keyValuePairs.Current.Key;
                m_GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, m_IndexBuffer, 0, iVertexList.Count / 2);
                iVertexList.Clear();
                m_VertexListQueue.Enqueue(iVertexList);
            }

            m_DrawQueue.Clear();
            m_GraphicsDevice.Textures[0] = null;
        }

        public void Dispose() {
            m_BasicEffect.Dispose();
            m_CRTEffect.Dispose();
            m_Pixel.Dispose();
        }


        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Color hue) {
            return DrawSprite(texture, position, area, new Vector4(0, 0, 1, 1), hue, Vector4.Zero);
        }

        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Vector4 uv, Color hue) {
            return DrawSprite(texture, position, area, uv, hue, Vector4.Zero);
        }

        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Vector4 uv, Color hue, Vector4 data) {
            List<VertexPositionTextureDataColor> vertexList;

            if (m_DrawQueue.ContainsKey(texture)) {
                vertexList = m_DrawQueue[texture];
            }
            else {
                if (m_VertexListQueue.Count > 0) {
                    vertexList = m_VertexListQueue.Dequeue();

                    vertexList.Clear();
                }
                else {
                    vertexList = new List<VertexPositionTextureDataColor>(1024);
                }

                m_DrawQueue.Add(texture, vertexList);
            }

            position += Depth.NextZ;

            PreTransformedQuad q = new PreTransformedQuad(position, area, uv, hue, data);

            for (int i = 0; i < q.Vertices.Length; i++) {
                vertexList.Add(q.Vertices[i]);
            }

            return true;
        }

        public void DrawRectangle(Vector3 position, Vector2 area, Color hue) {
            position += Depth.NextZ;

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

        public void DrawFilledRectangle(Vector3 position, Vector2 area, Color hue) {
            if (m_Pixel == null) {
                m_Pixel = new Texture2D(m_GraphicsDevice, 1, 1);
                m_Pixel.SetData(new[] {Color.White});
            }
            DrawSprite(m_Pixel, position, area, hue);
        }

        public void DrawFilledRectangle(Vector4 xywh, Color hue) {
            if (m_Pixel == null) {
                m_Pixel = new Texture2D(m_GraphicsDevice, 1, 1);
                m_Pixel.SetData(new[] {Color.White});
            }
            DrawSprite(m_Pixel, new Vector3(xywh.X, xywh.Y, 0), new Vector2(xywh.Z, xywh.W), hue);
        }
    }
}
