using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Patterns.MVC;

namespace Ypsilon.Modes.Landed
{
    class LandedView : AView
    {
        protected new LandedModel Model
        {
            get { return (LandedModel)base.Model; }
        }


        private LandedState m_State = LandedState.None;
        public LandedState State
        {
            get
            {
                return m_State;
            }
            set
            {
                if (m_State != value)
                {
                    m_State = value;
                    OnLandedStateChanged();
                }
            }
        }

        private SpriteBatchExtended m_SpriteBatch;
        private VectorRenderer m_Vectors;
        private Curses m_Curses;

        public LandedView(LandedModel model)
            : base(model)
        {
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatchExtended>();
            m_Vectors = ServiceRegistry.GetService<VectorRenderer>();
            m_Curses = new Curses(m_SpriteBatch.GraphicsDevice, 128, 40, @"Content\BIOS8x16.png", true);

            State = LandedState.Overview;
        }

        private void OnLandedStateChanged()
        {
            m_Curses.Clear();
            m_Curses.WriteString(8, 4, Model.LandedOn.Name);

            switch (State)
            {
                case LandedState.Overview:
                    m_Curses.WriteLines(8, 6, 60, Model.LandedOn.Description, '~');

                    m_Curses.WriteString(80, 8, "[1] - Bar.");
                    m_Curses.WriteString(80, 9, "[2] - Commodity Exchange.");
                    m_Curses.WriteString(80, 10, "[3] - Ship outfitter.");
                    m_Curses.WriteString(80, 11, "[4] - Shipyard.");
                    m_Curses.WriteString(80, 12, "[5] - Lift off.");
                    break;

                case LandedState.Exchange:
                    m_Curses.WriteString(8, 6, "Outfitter");
                    break;

                default:
                    m_Curses.WriteString(8, 6, "Error - LandedState is unknown value.");
                    break;
            }
            

            
        }

        public override void Draw(float frameSeconds)
        {
            int screenWidth = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;

            m_SpriteBatch.GraphicsDevice.Clear(new Color(16, 0, 16, 255));

            m_Curses.Render(m_SpriteBatch,
                new Vector2(
                    (screenWidth - m_Curses.DisplayWidth) / 2,
                    (screenHeight - m_Curses.DisplayHeight) / 2));
        }
    }
}
