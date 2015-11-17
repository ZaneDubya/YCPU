#region Using Statements
using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.World;
using Ypsilon.Core.Input;
#endregion

namespace Ypsilon
{
    public class YpsilonGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatchExtended m_SpriteBatch;
        private VectorRenderer m_Vectors;
        private InputManager m_Input;

        private WorldModel m_Model;

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

            ServiceRegistry.Register<VectorRenderer>(m_Vectors = new VectorRenderer(GraphicsDevice, Content));

            ServiceRegistry.Register<InputManager>(m_Input = new InputManager(Window.Handle));

            World.Crafting.Textures.Initialize(GraphicsDevice);
            m_Model = new World.WorldModel();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_Input.Update(totalSeconds, frameSeconds);

            m_Model.Update(totalSeconds, frameSeconds);
            m_Model.GetController().Update(frameSeconds, m_Input);

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
