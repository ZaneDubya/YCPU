#region Using Statements
using Microsoft.Xna.Framework;
using Ypsilon.Data;
using Ypsilon.Entities;
using Ypsilon.Graphics;
using Ypsilon.Views;
#endregion

namespace Ypsilon
{
    public class YpsilonGame : Game
    {
        private GraphicsDeviceManager m_Graphics;

        internal SpriteBatchExtended SpriteBatch
        {
            get;
            private set;
        }

        internal EntityManager Entities
        {
            get;
            private set;
        }

        internal TheView View
        {
            get;
            private set;
        }

        public YpsilonGame()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            m_Graphics.PreferredBackBufferWidth = 1280;
            m_Graphics.PreferredBackBufferHeight = 720;
            m_Graphics.IsFullScreen = false;
            m_Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            Components.Add(View = new TheView(this));
            Components.Add(SpriteBatch = new SpriteBatchExtended(this));

            Entities = new EntityManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create player
            Ship player = Entities.GetEntity<Ship>(Serial.GetNext(), true);
            State.Vars.PlayerSerial = player.Serial;
            // create other dudes.
            for (int i = 0; i < 30; i++)
            {
                Ship ship = Entities.GetEntity<Ship>(Serial.GetNext(), true);
                ship.Position = new Position3D(
                    Utility.Random_GetNonpersistantDouble() * 100 - 50,
                    Utility.Random_GetNonpersistantDouble() * 100 - 50,
                    Utility.Random_GetNonpersistantDouble() * 10 - 5);
            }

            // create a planet.
            Spob planet = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            planet.Definition = Definitions.GetSpob("planet");

            Spob asteroid1 = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid1.Definition = Definitions.GetSpob("asteroid small");
            asteroid1.Position = new Position3D(10, 10, 0);

            Spob asteroid2 = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid2.Definition = Definitions.GetSpob("asteroid large");
            asteroid2.Position = new Position3D(-10, 20, 0);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Entities.Update(frameSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
