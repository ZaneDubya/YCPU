using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ypsilon.Core.Graphics
{
    public class SpriteBatchExtended : DrawableGameComponent
    {
        private Effect _effect;

        private Dictionary<Texture2D, List<VertexPositionColorTexture>> _drawQueue;
        private Queue<List<VertexPositionColorTexture>> _vertexListQueue;
        private short[] _indexBuffer;

        private Vector3 _zOffset = new Vector3();
        public float ZOffset { set { _zOffset = new Vector3(0, 0, value); } }

        public SpriteBatchExtended(Game game)
            : base(game)
        {
            if (Game.Services.GetService(this.GetType()) != null)
                throw new Exception("A SpriteBatchLegacy service has already been added.");
            Game.Services.AddService(this.GetType(), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            _effect = Game.Content.Load<Effect>("Basic");
            _drawQueue = new Dictionary<Texture2D, List<VertexPositionColorTexture>>(256);
            _indexBuffer = createIndexBuffer(0x2000);
            _vertexListQueue = new Queue<List<VertexPositionColorTexture>>(256);
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
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            Texture2D iTexture;
            List<VertexPositionColorTexture> iVertexList;

            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionColorTexture>>> keyValuePairs = _drawQueue.GetEnumerator();

            _effect.Parameters["ProjectionMatrix"].SetValue(GraphicsUtility.ProjectionMatrixScreen);
            _effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            _effect.Parameters["Viewport"].SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));

            _effect.CurrentTechnique.Passes[0].Apply();

            while (keyValuePairs.MoveNext())
            {
                iTexture = keyValuePairs.Current.Key;
                iVertexList = keyValuePairs.Current.Value;
                GraphicsDevice.Textures[0] = iTexture;
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, _indexBuffer, 0, iVertexList.Count / 2);
                iVertexList.Clear();
                _vertexListQueue.Enqueue(iVertexList);
            }
            _drawQueue.Clear();
        }

        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Color hue)
        {
            List<VertexPositionColorTexture> vertexList;

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
                    vertexList = new List<VertexPositionColorTexture>(1024);
                }

                _drawQueue.Add(texture, vertexList);
            }

            position += _zOffset;

            PreTransformedQuad q = new PreTransformedQuad(position, area, hue);

            for (int i = 0; i < q.Vertices.Length; i++)
            {
                vertexList.Add(q.Vertices[i]);
            }

            return true;
        }

        public bool DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Vector4 uv, Color hue)
        {
            List<VertexPositionColorTexture> vertexList;

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
                    vertexList = new List<VertexPositionColorTexture>(1024);
                }

                _drawQueue.Add(texture, vertexList);
            }

            position += _zOffset;

            PreTransformedQuad q = new PreTransformedQuad(position, area, uv, hue);

            for (int i = 0; i < q.Vertices.Length; i++)
            {
                vertexList.Add(q.Vertices[i]);
            }

            return true;
        }

        public void DrawRectangle(Vector3 position, Vector2 area, Color hue)
        {
            position += _zOffset;

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

        Texture2D _texture;
        public void DrawFilledRectangle(Vector3 position, Vector2 area, Color hue)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(GraphicsDevice, 1, 1);
                _texture.SetData<Color>(new Color[] { Color.White });
            }
            DrawSprite(_texture, position, area, hue);
        }
    }

    struct TextToDraw
    {
        public string Text;
        public Vector2 Position;
        public Color Hue;
        public bool IsOriginCentered;
        public float Z;
        public int FontIndex;

        public TextToDraw(string text, FontEnum font, Vector3 position, Color hue, bool isOriginCentered)
        {
            Text = text;
            FontIndex = (int)font;
            Position = new Vector2(position.X, position.Y);
            Hue = hue;
            IsOriginCentered = isOriginCentered;
            Z = position.Z;
        }
    }

    public enum FontEnum
    {
        Arial12 = 0
    }
}
