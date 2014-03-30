using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YCPU.Platform.Graphics
{
    public class SpriteBatchExtended : DrawableGameComponent
    {
        private Effect _effect;

        private Dictionary<Texture2D, List<VertexPositionTextureHueExtra>> _drawQueue;
        private Queue<List<VertexPositionTextureHueExtra>> _vertexListQueue;
        private short[] _indexBuffer;

        private Vector3 _zOffset = new Vector3();
        public float ZOffset { set { _zOffset = new Vector3(0, 0, value); } }

        public Texture2D PaletteTexture = null;

        public SpriteBatchExtended(Game game)
            : base(game)
        {
            if (Game.Services.GetService(this.GetType()) != null)
                throw new Exception("A SpriteBatchExtended service has already been added.");
            Game.Services.AddService(this.GetType(), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            _effect = Support.Common.Content.Load<Effect>("NES_PTHE");
            _drawQueue = new Dictionary<Texture2D, List<VertexPositionTextureHueExtra>>(256);
            _indexBuffer = createIndexBuffer(0x2000);
            _vertexListQueue = new Queue<List<VertexPositionTextureHueExtra>>(256);
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
            return new Texture2D(Game.GraphicsDevice, width, height);
        }

        public override void Draw(GameTime gameTime)
        {
            _effect.Parameters["ProjectionMatrix"].SetValue(Support.Common.ProjectionMatrixScreen);
            _effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            _effect.Parameters["Viewport"].SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            _effect.Parameters["PALETTE"].SetValue(PaletteTexture);

            _effect.GraphicsDevice.Clear(Color.Green);

            // http://blogs.msdn.com/b/shawnhar/archive/2009/02/18/depth-sorting-alpha-blended-objects.aspx
            // Pass 0: draw the solid part: alpha blending disabled, alpha test set to only accept the 100%
            // opaque areas, and depth buffer enabled
            // Pass 1: draw the fringes: alpha blending enabled, alpha test set to only accept pixels with
            // alpha < 1, depth buffer enabled, depth writes disabled

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Texture2D iTexture;
            List<VertexPositionTextureHueExtra> iVertexList;
            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionTextureHueExtra>>> iTexturesVertexes = _drawQueue.GetEnumerator();

            _effect.CurrentTechnique.Passes[0].Apply();

            while (iTexturesVertexes.MoveNext())
            {
                iTexture = iTexturesVertexes.Current.Key;
                iVertexList = iTexturesVertexes.Current.Value;
                GraphicsDevice.Textures[0] = iTexture;
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTextureHueExtra>(
                    PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, _indexBuffer, 0, iVertexList.Count / 2);
                iVertexList.Clear();
                _vertexListQueue.Enqueue(iVertexList);
            }

            _drawQueue.Clear();
            _zOffset.Z = 0;
        }

        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Color? hue = null, bool zInc = true, bool Palettized = false)
        {
            List<VertexPositionTextureHueExtra> vertexList;

            if (_drawQueue.ContainsKey(texture))
            {
                vertexList = _drawQueue[texture];
            }
            else
            {
                if (_vertexListQueue.Count > 0)
                {
                    vertexList = _vertexListQueue.Dequeue();
                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionTextureHueExtra>(1024);
                }

                _drawQueue.Add(texture, vertexList);
            }

            if (zInc)
            {
                position += _zOffset;
                _zOffset.Z++;
            }

            PreTransformedQuad q = new PreTransformedQuad(position, area, (hue == null ? Color.White : hue.Value), new Vector2(0, Palettized ? 0 : 1));

            for (int i = 0; i < q.Vertices.Length; i++)
            {
                vertexList.Add(q.Vertices[i]);
            }

            return true;
        }

        public bool DrawTriangleList(Texture2D texture, Vector3 position, VertexPositionTextureHueExtra[] list)
        {
            List<VertexPositionTextureHueExtra> vertexList;

            if (_drawQueue.ContainsKey(texture))
            {
                vertexList = _drawQueue[texture];
            }
            else
            {
                if (_vertexListQueue.Count > 0)
                {
                    vertexList = _vertexListQueue.Dequeue();
                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionTextureHueExtra>(1024);
                }

                _drawQueue.Add(texture, vertexList);
            }

            position += _zOffset;
            _zOffset.Z++;

            for (int i = 0; i < list.Length; i++)
            {
                VertexPositionTextureHueExtra v = list[i];
                v.Position += position;
                vertexList.Add(v);
            }

            return true;
        }

        public void DrawRectangle(Vector3 position, Vector2 area, Color hue)
        {
            position += _zOffset;
            _zOffset.Z++;

            // Upper edge
            DrawRectangleFilled(position, 
                new Vector2(area.X - 1, 1), hue, false);
            // Left edge
            DrawRectangleFilled(position,
                new Vector2(1, area.Y), hue, false);
            // Bottom edge
            DrawRectangleFilled(position + new Vector3(0, area.Y - 1, 0), 
                new Vector2(area.X - 1, 1), hue, false);
            // Right edge
            DrawRectangleFilled(position + new Vector3(area.X - 1, 0, 0), 
                new Vector2(1, area.Y), hue, false);
        }

        Texture2D _texture;
        public void DrawRectangleFilled(Vector3 position, Vector2 area, Color hue, bool zInc = true)
        {
            if (zInc)
            {
                position += _zOffset;
                _zOffset.Z++;
            }

            if (_texture == null)
            {
                _texture = new Texture2D(GraphicsDevice, 1, 1);
                _texture.SetData<Color>(new Color[] { Color.White });
            }

            // clipping
            if (position.X < m_GUIClipRect.X)
            {
                if (position.X + area.X < m_GUIClipRect.X)
                    return;
                float delta = m_GUIClipRect.X - position.X;
                position.X += delta;
            }
            if (position.Y < m_GUIClipRect.Y)
            {
                if (position.Y + area.Y < m_GUIClipRect.Y)
                    return;
                float delta = m_GUIClipRect.Y - position.Y;
                position.Y += delta;
                area.Y -= delta;
            }
            if (position.X + area.X > m_GUIClipRect.Z)
            {
                if (position.X > m_GUIClipRect.Z)
                    return;
                float delta = m_GUIClipRect.Z - (position.X + area.X);
                area.X += delta;
            }
            if (position.Y + area.Y > m_GUIClipRect.W)
            {
                if (position.Y > m_GUIClipRect.W)
                    return;
                float delta = m_GUIClipRect.W - (position.Y + area.Y);
                area.Y += delta;
            }

            DrawSprite(_texture, position, area, hue, false);
        }


        List<Vector4> m_GUIClipRect_Stack;
        Vector4 m_GUIClipRect = Vector4.Zero;
        TextRenderer _textRenderer;
        public Rectangle GUIClipRect
        {
            set
            {
                m_GUIClipRect = new Vector4(value.X, value.Y, value.Right, value.Bottom);
            }
        }

        public void GUIClipRect_Push(Rectangle value)
        {
            if (m_GUIClipRect_Stack == null)
                m_GUIClipRect_Stack = new List<Vector4>();
            m_GUIClipRect_Stack.Add(m_GUIClipRect);
            m_GUIClipRect = new Vector4(value.X, value.Y, value.Right, value.Bottom);
        }

        public void GUIClipRect_Pop()
        {
            if (m_GUIClipRect_Stack == null || m_GUIClipRect_Stack.Count == 0)
                return;
            m_GUIClipRect = m_GUIClipRect_Stack[m_GUIClipRect_Stack.Count - 1];
            m_GUIClipRect_Stack.RemoveAt(m_GUIClipRect_Stack.Count - 1);
        }

        public void GUIDrawSprite(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle = null, Color? color = null, SpriteEffects effects = SpriteEffects.None, bool Palettized = false, int Palette = 0)
        {
            if (texture == null)
                return;
            Vector4 dest = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Right, destinationRectangle.Bottom);
            Vector4 srcDelta = new Vector4();
            if (sourceRectangle == null)
                sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            // clipping
            if (dest.X < m_GUIClipRect.X)
            {
                if (dest.Z < m_GUIClipRect.X)
                    return;
                float delta = m_GUIClipRect.X - dest.X;
                dest.X += delta;
                srcDelta.X = delta / destinationRectangle.Width;
            }
            if (dest.Z > m_GUIClipRect.Z)
            {
                if (dest.X > m_GUIClipRect.Z)
                    return;
                float delta = m_GUIClipRect.Z - dest.Z;
                dest.Z += delta;
                srcDelta.Z = delta / destinationRectangle.Width;
            }
            if (dest.Y < m_GUIClipRect.Y)
            {
                if (dest.W < m_GUIClipRect.Y)
                    return;
                float delta = m_GUIClipRect.Y - dest.Y;
                dest.Y += delta;
                srcDelta.Y = delta / destinationRectangle.Width;
            }
            if (dest.W > m_GUIClipRect.W)
            {
                if (dest.Y > m_GUIClipRect.W)
                    return;
                float delta = m_GUIClipRect.W - dest.W;
                dest.W += delta;
                srcDelta.W = delta / destinationRectangle.Width;
            }

            Vector4 source = new Vector4(
                (float)sourceRectangle.Value.X / texture.Width, (float)sourceRectangle.Value.Y / texture.Height,
                (float)sourceRectangle.Value.Right / texture.Width, (float)sourceRectangle.Value.Bottom / texture.Height);

            source.X += srcDelta.X * ((float)sourceRectangle.Value.Width / texture.Width);
            source.Y += srcDelta.Y * ((float)sourceRectangle.Value.Width / texture.Height);
            source.Z += srcDelta.Z * ((float)sourceRectangle.Value.Width / texture.Width);
            source.W += srcDelta.W * ((float)sourceRectangle.Value.Width / texture.Height);

            if (effects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                float x = source.X;
                source.X = source.Z;
                source.Z = x;
            }
            if (effects.HasFlag(SpriteEffects.FlipVertically))
            {
                float y = source.Y;
                source.Y = source.W;
                source.W = y;
            }

            PreTransformedQuad q = new PreTransformedQuad(texture, dest, source, _zOffset.Z++, color == null ? Color.White : color.Value, new Vector2(Palette, Palettized ? 0 : 1));

            List<VertexPositionTextureHueExtra> vertexList;
            if (_drawQueue.ContainsKey(texture))
            {
                vertexList = _drawQueue[texture];
            }
            else
            {
                if (_vertexListQueue.Count > 0)
                {
                    vertexList = _vertexListQueue.Dequeue();
                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionTextureHueExtra>(1024);
                }

                _drawQueue.Add(texture, vertexList);
            }

            for (int i = 0; i < q.Vertices.Length; i++)
            {
                vertexList.Add(q.Vertices[i]);
            }
        }

        public void GUIDrawString(SpriteFont font, string text, Vector2 location, Color? color = null)
        {
            if (_textRenderer == null)
                _textRenderer = new TextRenderer(Game.GraphicsDevice, Support.Common.Content.Load<SpriteFont>("Arial12"));

            Texture2D texture = _textRenderer.RenderText(text);
            if (texture == null)
                return;

            GUIDrawSprite(texture, new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height), new Rectangle(0, 0, texture.Width, texture.Height), color == null ? Color.White : color.Value);
        }
    }
}
