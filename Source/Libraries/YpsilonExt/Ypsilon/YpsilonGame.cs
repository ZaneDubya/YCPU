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
            Persistence.Savegame.Load();

            ServiceRegistry.Register<SpriteBatchExtended>(m_SpriteBatch = new SpriteBatchExtended(this));
            m_SpriteBatch.Initialize();

            ServiceRegistry.Register<VectorRenderer>(m_Vectors = new VectorRenderer(GraphicsDevice, Content));

            ServiceRegistry.Register<InputManager>(m_Input = new InputManager(Window.Handle));

            ServiceRegistry.Register<ModeManager>(m_Modes = new ModeManager());
            m_Modes.ActiveModel = new SpaceModel();

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
            World.Update(totalSeconds, frameSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float frameSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_Modes.Draw(frameSeconds);

            DrawTitleSafeAreas();
            m_Vectors.Render_ViewportSpace();

            m_SpriteBatch.Draw(gameTime);

            base.Draw(gameTime);
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
                new Vector3(dx, dy + height - 2 * dy, z) }, notActionSafeColor, 1f, Matrix.Identity, true);

            m_Vectors.DrawPolygon(new Vector3[4] { 
                new Vector3(dx * 2, dy * 2, z), 
                new Vector3(dx * 2 + width - 4 * dx, dy * 2, z),
                new Vector3(dx * 2 + width - 4 * dx, dy * 2 + height - 4 * dy, z),
                new Vector3(dx * 2, dy* 2 + height - 4 * dy, z) }, notTitleSafeColor, 1f, Matrix.Identity, true);
        }
    }
}
