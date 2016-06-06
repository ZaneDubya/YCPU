using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics {
    public class SpriteBatchExtended {
        public GraphicsDevice Graphics { get; private set; }

        private readonly Game m_Game;
        private short[] m_IndexBuffer;
        private Dictionary<EffectState, Dictionary<Texture2D, List<VertexPositionTextureDataColor>>> m_DrawCommands;
        private Queue<List<VertexPositionTextureDataColor>> m_QueuedVertexLists;

        public SpriteBatchExtended(Game game) {
            m_Game = game;
        }

        public void Initialize()
        {
            Graphics = m_Game.GraphicsDevice;
            m_DrawCommands = new Dictionary<EffectState, Dictionary<Texture2D, List<VertexPositionTextureDataColor>>>();
            m_IndexBuffer = CreateIndexBuffer(0x2000);
            m_QueuedVertexLists = new Queue<List<VertexPositionTextureDataColor>>(256);
        }

        public void Dispose()
        {

        }

        public Effect LoadEffectContent(string contentName)
        {
            return m_Game.Content.Load<Effect>(contentName);
        }

        public Texture2D LoadTextureContent(string contentName)
        {
            return m_Game.Content.Load<Texture2D>(contentName);
        }

        public void Begin(Color? clear = null) {
            if (clear == null)
                return;
            Graphics.Clear(clear.Value);
        }

        public void End(Matrix projection, Matrix view, Matrix world) {
            Graphics.DepthStencilState = DepthStencilState.Default;
            EndUnderlying(projection, view, world, false);
            Graphics.DepthStencilState = DepthStencilState.DepthRead;
            EndUnderlying(projection, view, world, true);
            EndClearAllVertexLists();
            Graphics.Textures[0] = null;
        }

        private void EndUnderlying(Matrix projection, Matrix view, Matrix world, bool drawTransparent)
        {
            foreach (EffectState effect in m_DrawCommands.Keys)
            {
                Dictionary<Texture2D, List<VertexPositionTextureDataColor>> vls = m_DrawCommands[effect];
                Graphics.BlendState = BlendState.AlphaBlend;
                Graphics.SamplerStates[0] = effect.Sampler;
                Graphics.RasterizerState = new RasterizerState
                {
                    ScissorTestEnable = true,
                    CullMode = CullMode.None
                };
                IEnumerator<KeyValuePair<Texture2D, List<VertexPositionTextureDataColor>>> vlKeyIsTexture = vls.GetEnumerator();
                effect.Effect.Parameters["ProjectionMatrix"].SetValue(projection);
                effect.Effect.Parameters["ViewMatrix"].SetValue(view);
                effect.Effect.Parameters["WorldMatrix"].SetValue(world);
                effect.Effect.Parameters["Viewport"].SetValue(new Vector2(Graphics.Viewport.Width, Graphics.Viewport.Height));
                effect.Effect.Parameters["DrawTransparentPixels"].SetValue(drawTransparent);
                effect.Effect.CurrentTechnique.Passes[0].Apply();
                while (vlKeyIsTexture.MoveNext())
                {
                    List<VertexPositionTextureDataColor> iVertexList = vlKeyIsTexture.Current.Value;
                    Graphics.Textures[0] = vlKeyIsTexture.Current.Key;
                    Graphics.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, m_IndexBuffer, 0, iVertexList.Count / 2);
                }
            }
        }

        private void EndClearAllVertexLists()
        {
            foreach (EffectState effect in m_DrawCommands.Keys)
            {
                Dictionary<Texture2D, List<VertexPositionTextureDataColor>> vls = m_DrawCommands[effect];
                IEnumerator<KeyValuePair<Texture2D, List<VertexPositionTextureDataColor>>> vlKeyIsTexture = vls.GetEnumerator();
                while (vlKeyIsTexture.MoveNext())
                {
                    List<VertexPositionTextureDataColor> iVertexList = vlKeyIsTexture.Current.Value;
                    iVertexList.Clear();
                    m_QueuedVertexLists.Enqueue(iVertexList);
                }
                vls.Clear();
            }
        }

        public bool DrawSprite(EffectState effect, Texture2D texture, Vector3 position, Vector2 area, Color hue)
        {
            return DrawSprite(effect, texture, position, area, new Vector4(0, 0, 1, 1), hue, Vector4.Zero);
        }

        public bool DrawSprite(EffectState effect, Texture2D texture, Vector3 position, Vector2 area, Vector4 uv, Color hue)
        {
            return DrawSprite(effect, texture, position, area, uv, hue, Vector4.Zero);
        }

        public bool DrawSprite(EffectState effect, Texture2D texture, Vector3 position, Vector2 area, Vector4 uv, Color hue, Vector4 data)
        {
            List<VertexPositionTextureDataColor> vl = GetVLForThisEffectAndTexture(effect, texture);
            position += Depth.NextZ;
            PreTransformedQuad q = new PreTransformedQuad(position, area, uv, hue, data);
            for (int i = 0; i < q.Vertices.Length; i++)
            {
                vl.Add(q.Vertices[i]);
            }
            return true;
        }

        private List<VertexPositionTextureDataColor> GetVLForThisEffectAndTexture(EffectState effect, Texture2D texture)
        {
            Dictionary<Texture2D, List<VertexPositionTextureDataColor>> vls;
            if (m_DrawCommands.ContainsKey(effect))
            {
                vls = m_DrawCommands[effect];
            }
            else
            {
                vls = new Dictionary<Texture2D, List<VertexPositionTextureDataColor>>();
                m_DrawCommands.Add(effect, vls);
            }

            List<VertexPositionTextureDataColor> vl;
            if (vls.ContainsKey(texture))
            {
                vl = vls[texture];
            }
            else
            {
                if (m_QueuedVertexLists.Count > 0)
                {
                    vl = m_QueuedVertexLists.Dequeue();
                    vl.Clear();
                }
                else
                {
                    vl = new List<VertexPositionTextureDataColor>(1024);
                }
                vls.Add(texture, vl);
            }
            return vl;
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