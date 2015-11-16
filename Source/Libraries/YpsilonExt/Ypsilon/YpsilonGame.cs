#region Using Statements
using Microsoft.Xna.Framework;
using Ypsilon.World.Data;
using Ypsilon.World.Entities;
using Ypsilon.Graphics;
using Ypsilon.World.View;
#endregion

namespace Ypsilon
{
    public class YpsilonGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatchExtended m_SpriteBatch;
        private World.WorldModel m_Model;

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
            ServiceRegistry.Register<SpriteBatchExtended>(m_SpriteBatch = new SpriteBatchExtended(this));
            m_SpriteBatch.Initialize();

            World.Data.Textures.Initialize(GraphicsDevice);
            m_Model = new World.WorldModel();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_Model.Update(totalSeconds, frameSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_Model.GetView().Draw(frameSeconds);

            m_SpriteBatch.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
