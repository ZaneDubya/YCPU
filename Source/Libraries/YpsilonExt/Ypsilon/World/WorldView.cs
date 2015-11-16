using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon.World.Entities;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.World.Views;
using Ypsilon.Graphics;

namespace Ypsilon.World
{
    /// <summary>
    /// Need a better name for this business.
    /// </summary>
    class WorldView : AView
    {
        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        private SpriteBatchExtended m_SpriteBatch;
        private Starfield m_Stars;

        public WorldView(WorldModel model)
            : base(model)
        {
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatchExtended>();

            m_Stars = new Starfield();
            m_Stars.CreateStars(100, new Rectangle(0, 0,
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight));
        }

        public override void Draw(float frameSeconds)
        {
            Ship player = (Ship)Model.Entities.GetPlayerEntity();

            m_SpriteBatch.GraphicsDevice.Clear(new Color(16, 0, 16, 255));

            // draw backdrop
            m_Stars.Update(player.Velocity * frameSeconds);
            m_Stars.Draw(m_SpriteBatch);

            // draw world
            Model.Entities.Draw(m_SpriteBatch.Vectors, player.Position);
            m_SpriteBatch.Vectors.Render_WorldSpace(Vector2.Zero, 1.0f);

            // draw user interface
            m_SpriteBatch.DrawTitleSafeAreas();
        }
    }
}
