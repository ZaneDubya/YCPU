using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics {
    public class SpriteBatchExtended {
        private Dictionary<Texture2D, List<VertexPositionTextureDataColor>> m_DrawQueue;

        private readonly Game m_Game;
        private short[] m_IndexBuffer;
        private Texture2D m_Pixel;
        private Queue<List<VertexPositionTextureDataColor>> m_VertexListQueue;
        public GraphicsDevice Graphics { get; private set; }

        public SpriteBatchExtended(Game game) {
            m_Game = game;
        }

        public Effect LoadEffect(string contentName)
        {
            return m_Game.Content.Load<Effect>(contentName);
        }

        public void Begin(Color? clear = null) {
            if (clear == null)
                return;
            Graphics.Clear(clear.Value);
        }

        public void Dispose() {
            m_Pixel.Dispose();
        }

        public void DrawFilledRectangle(Vector3 position, Vector2 area, Color hue) {
            if (m_Pixel == null) {
                m_Pixel = new Texture2D(Graphics, 1, 1);
                m_Pixel.SetData(new[] {Color.White});
            }
            DrawSprite(m_Pixel, position, area, hue);
        }

        public void DrawFilledRectangle(Vector4 xywh, Color hue) {
            if (m_Pixel == null) {
                m_Pixel = new Texture2D(Graphics, 1, 1);
                m_Pixel.SetData(new[] {Color.White});
            }
            DrawSprite(m_Pixel, new Vector3(xywh.X, xywh.Y, 0), new Vector2(xywh.Z, xywh.W), hue);
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

        public void End(EffectState effect, Matrix projection, Matrix view, Matrix world) {
            Graphics.BlendState = BlendState.AlphaBlend;
            Graphics.DepthStencilState = DepthStencilState.Default;
            Graphics.SamplerStates[0] = effect.Sampler;
            Graphics.RasterizerState = new RasterizerState {
                ScissorTestEnable = true,
                CullMode = CullMode.None
            }; // RasterizerState.CullNone;
            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionTextureDataColor>>> keyValuePairs = m_DrawQueue.GetEnumerator();
            effect.Effect.Parameters["ProjectionMatrix"].SetValue(projection);
            effect.Effect.Parameters["ViewMatrix"].SetValue(view);
            effect.Effect.Parameters["WorldMatrix"].SetValue(world);
            effect.Effect.Parameters["Viewport"].SetValue(new Vector2(Graphics.Viewport.Width, Graphics.Viewport.Height));
            effect.Effect.CurrentTechnique.Passes[0].Apply();
            while (keyValuePairs.MoveNext()) {
                List<VertexPositionTextureDataColor> iVertexList = keyValuePairs.Current.Value;
                Graphics.Textures[0] = keyValuePairs.Current.Key;
                Graphics.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, m_IndexBuffer, 0, iVertexList.Count / 2);
                iVertexList.Clear();
                m_VertexListQueue.Enqueue(iVertexList);
            }
            m_DrawQueue.Clear();
            Graphics.Textures[0] = null;
        }

        public void Initialize() {
            Graphics = m_Game.GraphicsDevice;
            m_DrawQueue = new Dictionary<Texture2D, List<VertexPositionTextureDataColor>>(256);
            m_IndexBuffer = CreateIndexBuffer(0x2000);
            m_VertexListQueue = new Queue<List<VertexPositionTextureDataColor>>(256);
        }

        public Texture2D NewTexture(int width, int height) {
            return new Texture2D(Graphics, width, height);
        }

        public void SetScissorRect(Rectangle? r) {
            if (r == null)
                r = Graphics.Viewport.Bounds;
            Graphics.ScissorRectangle = r.Value;
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
    }
}