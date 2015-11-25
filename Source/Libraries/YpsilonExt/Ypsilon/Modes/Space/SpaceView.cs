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
            m_Curses = new Curses(m_SpriteBatch.GraphicsDevice, 128, 40, @"Content\BIOS8x16.png", false);

            m_Stars = new Starfield();
            m_Stars.CreateStars(100, new Rectangle(0, 0,
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight));
        }

        public override void Draw(float frameSeconds)
        {
            // update curses
            UpdateCurses(frameSeconds);

            int screenWidth = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;

            SpaceController controller = (SpaceController)Model.GetController();
            MouseOverList mouseOver = controller.MouseOverList;
            mouseOver.WorldMousePosition = new Vector3(
                mouseOver.ScreenMousePosition.X - screenWidth / 2,
                (screenHeight - screenHeight / 2) - mouseOver.ScreenMousePosition.Y,
                0);
            mouseOver.Reset();

            Ship player = (Ship)World.Entities.GetPlayerEntity();
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
            m_Curses.Render(m_SpriteBatch, 
                new Vector2(
                    (screenWidth - m_Curses.DisplayWidth) / 2, 
                    (screenHeight - m_Curses.DisplayHeight) / 2));

            m_Vectors.Render(
                GraphicsUtility.CreateProjectionMatrixScreenOffset(m_SpriteBatch.GraphicsDevice),
                Matrix.Identity,
                Matrix.Identity);
        }

        private bool m_DisplayingMessage = false;
        private string m_Message;
        private float m_MessageTime;

        private void UpdateCurses(float frameSeconds)
        {
            UpdateDisplayMessage(frameSeconds);

            m_Curses.Clear();

            Ship player = (Ship)World.Entities.GetPlayerEntity();
            ShipSpaceComponent playerShip = player.GetComponent<ShipSpaceComponent>();

            m_Curses.WriteString(0, 0, string.Format("P <{0:F2} {1:F2}>", playerShip.Position.X, playerShip.Position.Y));
            m_Curses.WriteString(0, 1, string.Format("V <{0:F2} {1:F2}>", playerShip.Velocity.X, playerShip.Velocity.Y));
            // m_Curses.WriteString(0, 0, "Player ship");
            m_Curses.WriteString(0, 38, "Sh: \xDB\xDB\xDB\xDB\xDB\xDB\xDB\xDB");
            m_Curses.WriteString(0, 39, "Ar: \xDB\xDB\xDB\xDB\xDB\xDB\xDB\xDB");

            m_Curses.WriteString(0, 4, string.Format("Hold:"));
            m_Curses.WriteString(0, 5, string.Format("Ore: {0:F2} kg", player.ResourceOre));

            AEntity selected = World.Entities.GetEntity<AEntity>(Model.SelectedSerial);
            if (selected != null)
            {
                string name = selected.Name;
                m_Curses.WriteString(64 - name.Length / 2, 2, name);
                // m_Curses.WriteString(8, 2, "Hello world!");
            }

            //if (PlayerState.SelectedSerial != Serial.Null)
            //    

            if (m_DisplayingMessage)
            {
                m_Curses.WriteString(64 - m_Message.Length / 2, 32, m_Message);
            }
        }

        private void UpdateDisplayMessage(float frameSeconds)
        {
            MessageType msgType;
            string msg;
            if (Messages.Get(out msgType, out msg))
            {
                m_DisplayingMessage = true;
                m_Message = msg;
                m_MessageTime = 2f + m_Message.Length * .1f + frameSeconds;
            }

            if (m_DisplayingMessage)
            {
                m_MessageTime -= frameSeconds;
                if (m_MessageTime <= 0)
                {
                    m_DisplayingMessage = false;
                }
            }
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

            foreach (KeyValuePair<int, AEntity> entity in World.Entities.AllEntities)
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
