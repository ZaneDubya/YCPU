// RoundLine.cs
// By Michael D. Anderson
// Version 4.00, Feb 8 2011
// A class to efficiently draw thick lines with rounded ends.
// Microsoft Public License (Ms-PL)

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ypsilon.Graphics;
using Ypsilon.Entities;
using System;
using System.Collections.Generic;
#endregion

namespace Ypsilon
{
    public class NewGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatchExtended m_SpriteBatch;
        private List<Ship> m_Entities;
        private Ship m_Player;

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

            Random r = new Random();
            for (int i = 0; i < 0; i++)
            {
                Ship ship = new Ship();
                ship.Position = new Position3D(r.NextDouble() * 100 - 50, r.NextDouble() * 100 - 50, r.NextDouble() * 40 - 20);
                m_Entities.Add(ship);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            double frameSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            m_Player.Update(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            double frameSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            GraphicsDevice.Clear(new Color(16, 0, 16, 255));
            m_SpriteBatch.DrawTitleSafeAreas();
            foreach (Ship ship in m_Entities)
            {
                ship.Draw(frameSeconds, m_SpriteBatch.Vectors, m_Player.Position);
            }
            
            m_SpriteBatch.Vectors.Render_WorldSpace(Vector2.Zero, 1.0f);
            base.Draw(gameTime);
        }
    }
}
