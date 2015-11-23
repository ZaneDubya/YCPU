#region Using Statements
using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Input;
using Ypsilon.Modes;
using Ypsilon.Modes.Space;
#endregion

namespace Ypsilon
{
    public class YpsilonGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatchExtended m_SpriteBatch;
        private VectorRenderer m_Vectors;
        private InputManager m_Input;

        private ModeManager m_Modes;

        public YpsilonGame()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            m_Graphics.PreferredBackBufferWidth = 1280;
            m_Graphics.PreferredBackBufferHeight = 720;
            m_Graphics.IsFullScreen = false;
            m_Graphics.ApplyChanges();
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ServiceRegistry.Register<SpriteBatchExtended>(m_SpriteBatch = new SpriteBatchExtended(this));
            m_SpriteBatch.Initialize();

            ServiceRegistry.Register<VectorRenderer>(m_Vectors = new VectorRenderer(GraphicsDevice, Content));

            ServiceRegistry.Register<InputManager>(m_Input = new InputManager(Window.Handle));

            ServiceRegistry.Register<ModeManager>(m_Modes = new ModeManager());
            m_Modes.ActiveModel = Persistence.Savegame.Load();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Modes.Space.Resources.Textures.Initialize(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_Input.Update(totalSeconds, frameSeconds);
            m_Modes.Update(totalSeconds, frameSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_Modes.Draw(frameSeconds);

            m_SpriteBatch.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
