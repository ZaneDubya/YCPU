using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace YCPU.Platform.Graphics
{
    public class TextureBaker : SpriteBatch, IDisposable
    {
        public enum RenderState { Tile, Fill, Stencil }

        private readonly RenderTarget2D _renderTarget;

        public TextureBaker(GraphicsDevice graphicsDevice, int width, int height, RenderState state)
            : base(graphicsDevice)
        {
            _renderTarget = new RenderTarget2D(GraphicsDevice, width, height);
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            SetState(state);
        }

        #region State

        public void SetState(RenderState state)
        {

            switch (state)
            {
                case (RenderState.Tile):

                    GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;

                    break;
                case (RenderState.Fill):

                    GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;

                    break;
                case (RenderState.Stencil):

                    GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                    GraphicsDevice.BlendState = new BlendState
                    {

                        ColorWriteChannels = ColorWriteChannels.All,

                        ColorSourceBlend = Blend.Zero,
                        AlphaSourceBlend = Blend.Zero,

                        ColorDestinationBlend = Blend.SourceAlpha,
                        AlphaDestinationBlend = Blend.SourceAlpha,

                    };

                    break;
            }
        }

        #endregion

        #region Mask baking

        public void Mask(Texture2D mask, Rectangle destination, Color color)
        {

            var oldState = GraphicsDevice.BlendState;

            GraphicsDevice.BlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One
            };

            Draw(mask, destination, color);

            GraphicsDevice.BlendState = oldState;
        }

        public void Mask(Texture2D mask, Rectangle destination, Rectangle source, Color color)
        {

            var oldState = GraphicsDevice.BlendState;

            GraphicsDevice.BlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One
            };

            Draw(mask, destination, source, color);

            GraphicsDevice.BlendState = oldState;
        }

        public void Mask(Texture2D mask, Rectangle destination, Rectangle source, Color color,
            float rotation, Vector2 origin, SpriteEffects effects, Single layer)
        {

            var oldState = GraphicsDevice.BlendState;

            GraphicsDevice.BlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One
            };

            Draw(mask, destination, source, color, rotation, origin, effects, layer);

            GraphicsDevice.BlendState = oldState;
        }

        public void Mask(Texture2D mask, Vector2 position, Color color)
        {

            var oldState = GraphicsDevice.BlendState;

            GraphicsDevice.BlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One
            };

            Draw(mask, position, color);

            GraphicsDevice.BlendState = oldState;
        }

        public void Mask(Texture2D mask, Vector2 position, Rectangle source, Color color)
        {

            var oldState = GraphicsDevice.BlendState;

            GraphicsDevice.BlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One
            };

            Draw(mask, position, source, color);

            GraphicsDevice.BlendState = oldState;
        }

        public void Mask(Texture2D mask, Vector2 position, Rectangle source, Color color,
            float rotation, Vector2 origin, float scale, SpriteEffects effects, Single layer)
        {

            var oldState = GraphicsDevice.BlendState;

            GraphicsDevice.BlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One
            };

            Draw(mask, position, source, color, rotation, origin, scale, effects, layer);

            GraphicsDevice.BlendState = oldState;
        }

        # endregion

        #region SpriteFont baking

        public void BakeTextCentered(SpriteFont font, string text, Vector2 location, Color textColor)
        {
            var shifted = new Vector2
            {
                X = (float)Math.Round(location.X - font.MeasureString(text).X / 2),
                Y = (float)Math.Round(location.Y - font.MeasureString(text).Y / 2)
            };

            DrawString(font, text, shifted, textColor);
        }

        #endregion

        public Texture2D GetTexture()
        {

            End();
            GraphicsDevice.SetRenderTarget(null);

            return _renderTarget;
        }
    }
}
