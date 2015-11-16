using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Entities;

namespace Ypsilon.Views
{
    /// <summary>
    /// Need a better name for this business.
    /// </summary>
    class TheView : DrawableGameComponent
    {
        private YpsilonGame m_Game;

        private Stars m_Stars;

        public TheView(YpsilonGame game)
            : base(game)
        {
            m_Game = game;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_Stars = new Stars(GraphicsDevice);
            m_Stars.CreateStars(100, new Rectangle(0, 0,
                GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Ship player = (Ship)m_Game.Entities.GetPlayerEntity();

            m_Stars.Update(player.Velocity * frameSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            Position3D playerPosition = m_Game.Entities.GetPlayerEntity().Position;

            GraphicsDevice.Clear(new Color(16, 0, 16, 255));

            // draw backdrop
            m_Stars.Draw(m_Game.SpriteBatch);

            // draw world
            m_Game.Entities.Draw(m_Game.SpriteBatch.Vectors, playerPosition);
            m_Game.SpriteBatch.Vectors.Render_WorldSpace(Vector2.Zero, 1.0f);

            // draw user interface
            m_Game.SpriteBatch.DrawTitleSafeAreas();

            base.Draw(gameTime);
        }
    }
}
