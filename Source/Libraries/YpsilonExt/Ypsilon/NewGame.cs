#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Ypsilon.Entities;
using Ypsilon.Graphics;
using Ypsilon.Views;
#endregion

namespace Ypsilon
{
    public class NewGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatchExtended m_SpriteBatch;
        private List<Ship> m_Entities;
        private Ship m_Player;
        private Stars m_Stars;

        public NewGame()
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
            Components.Add(m_SpriteBatch = new Graphics.SpriteBatchExtended(this));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_Entities = new List<Ship>();
            m_Entities.Add(m_Player = new Ship());
            m_Player.IsPlayerEntity = true;

            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                Ship ship = new Ship();
                ship.Position = new Position3D(r.NextDouble() * 100 - 50, r.NextDouble() * 100 - 50, r.NextDouble() * 40 - 20);
                m_Entities.Add(ship);
            }

            m_Stars = new Stars(GraphicsDevice);
            m_Stars.CreateStars(100, new Rectangle(0, 0,
                m_Graphics.PreferredBackBufferWidth, m_Graphics.PreferredBackBufferHeight));
        }

        protected override void Update(GameTime gameTime)
        {
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Ship ship in m_Entities)
            {
                ship.Update(frameSeconds);
            }

            m_Stars.Update(m_Player.Velocity * frameSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Position3D playerPosition = m_Player.Position;

            GraphicsDevice.Clear(new Color(16, 0, 16, 255));
            m_SpriteBatch.DrawTitleSafeAreas();
            m_Stars.Draw(m_SpriteBatch);

            foreach (Ship ship in m_Entities)
            {
                ship.Draw(m_SpriteBatch.Vectors, playerPosition);
            }
            
            m_SpriteBatch.Vectors.Render_WorldSpace(Vector2.Zero, 1.0f);
            base.Draw(gameTime);
        }
    }
}
