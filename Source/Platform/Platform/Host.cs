using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Ypsilon.Platform.Input;

namespace Ypsilon.Platform
{
    public class Host : Game
    {
        GraphicsDeviceManager m_Graphics;
        Settings m_Settings;
        InputState m_Input;
        FPS m_FPS;
        
        protected SpriteBatch m_SpriteBatch;

        protected InputState InputState
        {
            get { return m_Input; }
        }

        protected Settings Settings
        {
            get { return m_Settings; }
        }

        public Host()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.IsFullScreen = false;
            m_Graphics.PreferredBackBufferWidth = 640;
            m_Graphics.PreferredBackBufferHeight = 480;

            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_Settings = Library.Settings = new Settings();

            m_FPS = new FPS();

            m_Input = new Input.InputState();
            m_Input.Initialize(this.Window.Handle);

            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            m_SpriteBatch.Dispose();
        }

        protected object Create<T>()
        {
            switch (typeof(T).ToString())
            {
                case "Ypsilon.DeviceRenderer":
                    return new DeviceRenderer(m_SpriteBatch);
            }
            return null;
        }

        protected override void Update(GameTime gameTime)
        {
            m_Input.Update(gameTime);

            if (m_Settings.HasUpdates)
                handleUpdates();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (m_FPS.Update(gameTime))
                this.Window.Title = string.Format("YCPU Host [{0} fps]", m_FPS.CurrentFPS);

            GraphicsDevice.Clear(Color.Black);
            m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            OnDraw(gameTime);
            m_SpriteBatch.End();
            GraphicsDevice.Textures[0] = null;
            
        }

        protected virtual void OnDraw(GameTime gameTime)
        {

        }

        private void handleUpdates()
        {
            Settings.Setting s;
            while ((s = m_Settings.NextUpdate()) != Settings.Setting.None)
            {
                switch (s)
                {
                    case Settings.Setting.Resolution:
                        m_Graphics.PreferredBackBufferWidth = m_Settings.Resolution.X;
                        m_Graphics.PreferredBackBufferHeight = m_Settings.Resolution.Y;
                        m_Graphics.ApplyChanges();
                        break;
                    default:
                        throw new Exception("Setting not handled.");
                }
            }
        }
    }
}
