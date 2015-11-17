using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon.World.Entities;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.World.Views;
using System.Collections.Generic;
using Ypsilon.Core.Graphics;

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
        private VectorRenderer m_Vectors;
        private List<AEntity> m_MouseOverEntities;

        private Starfield m_Stars;

        public WorldView(WorldModel model)
            : base(model)
        {
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatchExtended>();
            m_Vectors = ServiceRegistry.GetService<VectorRenderer>();
            m_MouseOverEntities = new List<AEntity>();

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

            // get a list of all entities visible in the world. add them to a local list of visible entities.
            List<AEntity> visible = Model.Entities.GetVisibleEntities(player.Position, new Vector2(
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight));
            foreach (AEntity e in visible)
            {
                e.Draw(m_Vectors, player.Position);
            }

            // now render using sprite batches...
            m_Vectors.Render(
                Utility.CreateProjectionMatrixScreenCentered(m_SpriteBatch.GraphicsDevice),
                Matrix.Identity,
                Matrix.Identity);

            // draw user interface
            DrawTitleSafeAreas();

            m_Vectors.Render(
                Utility.CreateProjectionMatrixScreenOffset(m_SpriteBatch.GraphicsDevice),
                Matrix.Identity,
                Matrix.Identity);
        }

        private void DrawTitleSafeAreas()
        {
            int width = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;
            int dx = (int)(width * 0.05);
            int dy = (int)(height * 0.05);
            int z = 1999;

            Color notActionSafeColor = new Color(255, 0, 0, 127); // Red, 50% opacity
            Color notTitleSafeColor = new Color(255, 255, 0, 127); // Yellow, 50% opacity

            m_Vectors.DrawPolygon(new Vector3[4] { 
                new Vector3(dx, dy, z), 
                new Vector3(dx + width - 2 * dx, dy, z),
                new Vector3(dx + width - 2 * dx, dy + height - 2 * dy, z),
                new Vector3(dx, dy + height - 2 * dy, z) }, true, notActionSafeColor, false);

            m_Vectors.DrawPolygon(new Vector3[4] { 
                new Vector3(dx * 2, dy * 2, z), 
                new Vector3(dx * 2 + width - 4 * dx, dy * 2, z),
                new Vector3(dx * 2 + width - 4 * dx, dy * 2 + height - 4 * dy, z),
                new Vector3(dx * 2, dy* 2 + height - 4 * dy, z) }, true, notTitleSafeColor, false);
        }
    }
}
