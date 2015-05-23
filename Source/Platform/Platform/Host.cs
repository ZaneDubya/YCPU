using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Ypsilon.Platform.Support;

namespace Ypsilon.Platform
{
    public class Host : Game
    {
        GraphicsDeviceManager m_Graphics;
        Settings m_Settings;
        InputState m_Input;
        FPS m_FPS;
        private Graphics.ExtendedSpriteBatch m_SBX;

        protected Graphics.ExtendedSpriteBatch SpriteBatch
        {
            get { return m_SBX; }
        }

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

            m_Input = new Support.InputState();
            m_Input.Initialize(this.Window.Handle);

            m_FPS = new Support.FPS();

            m_SBX = new Graphics.ExtendedSpriteBatch(this);

            this.IsMouseVisible = true;

            Library.Content = new ResourceContentManager(Services, ResContent.ResourceManager);
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_SBX.Initialize();
        }

        protected override void LoadContent()
        {
            m_Settings = new Settings();
            Library.Initialize(m_Settings, m_Graphics.GraphicsDevice, m_Input);
        }

        protected override void UnloadContent()
        {
            m_SBX.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            m_Input.Update(gameTime);

            if (m_Settings.HasUpdates)
                handleUpdates();

            base.Update(gameTime);

            m_SBX.ResetGuiClipRect();
        }

        protected override void Draw(GameTime gameTime)
        {
            if (m_FPS.Update(gameTime))
                this.Window.Title = string.Format("YCPU Host [{0} fps]", m_FPS.CurrentFPS);
            m_SBX.Draw(gameTime);
            base.Draw(gameTime);
        }

        private void handleUpdates()
        {
            Settings.Setting s;
            while ((s = m_Settings.NextUpdate()) != Support.Settings.Setting.None)
            {
                switch (s)
                {
                    case Support.Settings.Setting.Resolution:
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
