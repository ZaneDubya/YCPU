using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace YCPU.Platform
{
    public class Host : Game
    {
        GraphicsDeviceManager m_Graphics;
        Support.Settings m_Settings;
        Support.InputState m_Input;
        Support.FPS m_FPS;
        private Graphics.SpriteBatchExtended m_SBX;

        protected Graphics.SpriteBatchExtended SpriteBatch
        {
            get { return m_SBX; }
        }

        protected Support.Settings Settings
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

            m_SBX = new Graphics.SpriteBatchExtended(this);
            this.Components.Add(m_SBX);

            this.IsMouseVisible = true;

            Support.Common.Content = new ResourceContentManager(Services, ResContent.ResourceManager);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_Settings = new Support.Settings();
            Support.Common.Initialize(m_Settings, m_Graphics.GraphicsDevice, m_Input);
        }

        protected override void UnloadContent()
        {
            m_SBX.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            m_Input.Update(gameTime);

            if (m_Input.HandleKeyboardEvent(Input.KeyboardEvent.Press, Input.WinKeys.Escape, false, false, false))
                this.Exit();

            if (m_Settings.HasUpdates)
                handleUpdates();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (m_FPS.Update(gameTime))
                this.Window.Title = string.Format("YCPU Host [{0} fps]", m_FPS.CurrentFPS);
            base.Draw(gameTime);
        }

        private void handleUpdates()
        {
            Support.Settings.Setting s;
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
