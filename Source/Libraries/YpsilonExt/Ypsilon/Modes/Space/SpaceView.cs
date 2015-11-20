using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ypsilon.Core;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Entities;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Views;

namespace Ypsilon.Modes.Space
{
    /// <summary>
    /// Need a better name for this business.
    /// </summary>
    class SpaceView : AView
    {
        protected new SpaceModel Model
        {
            get { return (SpaceModel)base.Model; }
        }

        private SpriteBatchExtended m_SpriteBatch;
        private VectorRenderer m_Vectors;
        private Curses m_Curses;

        private Starfield m_Stars;

        public SpaceView(SpaceModel model)
            : base(model)
        {
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatchExtended>();
            m_Vectors = ServiceRegistry.GetService<VectorRenderer>();
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

            SpaceController controller = (SpaceController)Model.GetController();
            MouseOverList mouseOver = controller.MouseOverList;
            mouseOver.WorldMousePosition = new Vector3(
                mouseOver.ScreenMousePosition.X - screenWidth / 2,
                (screenHeight - screenHeight / 2) - mouseOver.ScreenMousePosition.Y,
                0);
            mouseOver.Reset();

            Ship player = (Ship)Model.Entities.GetPlayerEntity();
            ShipSpaceComponent playerShip = player.GetComponent<ShipSpaceComponent>();

            m_SpriteBatch.GraphicsDevice.Clear(new Color(16, 0, 16, 255));

            // draw backdrop
            m_Stars.Update(playerShip.Velocity * frameSeconds);
            m_Stars.Draw(m_SpriteBatch);

            // get a list of all entities visible in the world. add them to a local list of visible entities.
            List<AEntity> visible = GetVisibleEntities(playerShip.Position, new Vector2(screenWidth, screenHeight));
            foreach (AEntity e in visible)
            {
                AEntitySpaceComponent c = e.GetComponent<AEntitySpaceComponent>();
                c.Draw(m_Vectors, playerShip.Position, mouseOver);
                if (e.Serial == Model.SelectedSerial)
                    c.DrawSelection(m_Vectors);
            }

            // now render using sprite batches...
            m_Vectors.Render(
                GraphicsUtility.CreateProjectionMatrixScreenCentered(m_SpriteBatch.GraphicsDevice),
                Matrix.Identity,
                Matrix.Identity);

            // draw user interface
            // DrawTitleSafeAreas();
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
            ShipSpaceComponent playerShip = player.GetComponent<ShipSpaceComponent>();

            m_Curses.WriteString(0, 0, string.Format("P <{0:F2} {1:F2}>", playerShip.Position.X, playerShip.Position.Y));
            m_Curses.WriteString(0, 1, string.Format("V <{0:F2} {1:F2}>", playerShip.Velocity.X, playerShip.Velocity.Y));

            m_Curses.WriteString(108, 0, string.Format("Hold:"));
            m_Curses.WriteString(108, 1, string.Format("Ore: {0:F2} kg", player.ResourceOre));

            AEntity selected = Model.Entities.GetEntity<AEntity>(Model.SelectedSerial, false);
            if (selected != null)
            {
                string name = selected.Name;
                m_Curses.WriteString(64 - name.Length / 2, 2, name);
                // m_Curses.WriteString(8, 2, "Hello world!");
            }

            //if (PlayerState.SelectedSerial != Serial.Null)
            //    
        }

        // ======================================================================
        // Drawable entities 
        // ======================================================================

        private List<AEntity> m_EntitiesOnScreen;

        public List<AEntity> GetVisibleEntities(Position3D center, Vector2 dimensions)
        {
            RectangleF bounds = new RectangleF(
                (float)center.X - dimensions.X / 2f,
                (float)center.Y - dimensions.Y / 2f,
                dimensions.X,
                dimensions.Y);
            if (m_EntitiesOnScreen == null)
                m_EntitiesOnScreen = new List<AEntity>();
            m_EntitiesOnScreen.Clear();

            foreach (KeyValuePair<int, AEntity> entity in Model.Entities.AllEntities)
            {
                AEntity e = entity.Value;
                AEntitySpaceComponent c = e.GetComponent<AEntitySpaceComponent>();

                if (!e.IsDisposed && c.IsInitialized && c.IsVisible && c.Position.Intersects(bounds, c.ViewSize))
                {
                    m_EntitiesOnScreen.Add(e);
                }
            }
            return m_EntitiesOnScreen;
        }
    }
}
