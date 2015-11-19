using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.World.Entities;
using Ypsilon.World.Input;
using Ypsilon.World.Views;

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
        private Curses m_Curses;

        private Starfield m_Stars;

        public WorldView(WorldModel model)
            : base(model)
        {
            m_SpriteBatch = YpsilonGame.Registry.GetService<SpriteBatchExtended>();
            m_Vectors = YpsilonGame.Registry.GetService<VectorRenderer>();
            m_Curses = new Curses(m_SpriteBatch.GraphicsDevice, 128, 40, @"Content\BIOS8x16.png");

            m_Stars = new Starfield();
            m_Stars.CreateStars(100, new Rectangle(0, 0,
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight));
        }

        public override void Draw(float frameSeconds)
        {
            // update curses
            UpdateCurses();

            int screenWidth = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;

            WorldController controller = (WorldController)Model.GetController();
            MouseOverList mouseOver = controller.MouseOverList;
            mouseOver.WorldMousePosition = new Vector3(
                mouseOver.ScreenMousePosition.X - screenWidth / 2,
                (screenHeight - screenHeight / 2) - mouseOver.ScreenMousePosition.Y,
                0);
            mouseOver.Reset();

            Ship player = (Ship)Model.Entities.GetPlayerEntity();

            m_SpriteBatch.GraphicsDevice.Clear(new Color(16, 0, 16, 255));

            // draw backdrop
            m_Stars.Update(player.Velocity * frameSeconds);
            m_Stars.Draw(m_SpriteBatch);

            // get a list of all entities visible in the world. add them to a local list of visible entities.
            List<AEntity> visible = Model.Entities.GetVisibleEntities(player.Position, new Vector2(screenWidth, screenHeight));
            foreach (AEntity e in visible)
            {
                e.Draw(m_Vectors, player.Position, mouseOver);
            }

            // now render using sprite batches...
            m_Vectors.Render(
                GraphicsUtility.CreateProjectionMatrixScreenCentered(m_SpriteBatch.GraphicsDevice),
                Matrix.Identity,
                Matrix.Identity);

            // draw user interface
            DrawTitleSafeAreas();
            m_Curses.Render(m_SpriteBatch, new Vector2(64, 40));

            m_Vectors.Render(
                GraphicsUtility.CreateProjectionMatrixScreenOffset(m_SpriteBatch.GraphicsDevice),
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

            Color notActionSafeColor = new Color(127, 0, 0, 127); // Red, 50% opacity
            Color notTitleSafeColor = new Color(127, 127, 0, 127); // Yellow, 50% opacity

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

        private void UpdateCurses()
        {
            m_Curses.Clear();

            Ship player = (Ship)Model.Entities.GetPlayerEntity();
            m_Curses.WriteString(0, 0, string.Format("P <{0:F2} {1:F2}>", player.Position.X, player.Position.Y));
            m_Curses.WriteString(0, 1, string.Format("V <{0:F2} {1:F2}>", player.Velocity.X, player.Velocity.Y));

            m_Curses.WriteString(108, 0, string.Format("Hold:"));
            m_Curses.WriteString(108, 1, string.Format("Ore: {0:F2} kg", player.ResourceOre));

            //if (PlayerState.SelectedSerial != Serial.Null)
            //    m_Curses.WriteString(8, 2, "Hello world!");
        }
    }
}
