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
#endregion

namespace Ypsilon
{
    public class NewGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatchExtended m_SpriteBatch;
        private Entities.Entity m_Entity;

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
            m_Entity = new Entities.Entity();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            m_Entity.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(16, 0, 16, 255));
            m_SpriteBatch.DrawTitleSafeAreas();
            m_SpriteBatch.Vectors.DrawPolygon(m_Entity.WorldVertices(Entities.Position3D.Zero), true, Color.White, false);
            m_SpriteBatch.Vectors.Render_WorldSpace(Vector2.Zero, 1.0f);
            base.Draw(gameTime);
        }
    }
}
