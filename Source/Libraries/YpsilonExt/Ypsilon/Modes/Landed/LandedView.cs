using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Data;

namespace Ypsilon.Modes.Landed
{
    class LandedView : AView
    {
        protected new LandedModel Model
        {
            get { return (LandedModel)base.Model; }
        }

        public bool SelectSecond = false;

        private int m_SelectIndex, m_SelectScrollOffset;
        private int m_SelectIndex2, m_SelectScrollOffset2;

        public int SelectIndex
        {
            get
            {
                return SelectSecond ? m_SelectIndex2 : m_SelectIndex;
            }
            set
            {
                if (SelectSecond)
                    m_SelectIndex2 = value;
                else
                    m_SelectIndex = value;
            }
        }

        public int SelectScrollOffset
        {
            get
            {
                return SelectSecond ? m_SelectScrollOffset2 : m_SelectScrollOffset;
            }
            set
            {
                if (SelectSecond)
                    m_SelectScrollOffset2 = value;
                else
                    m_SelectScrollOffset = value;
            }
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
            switch (State)
            {
                case LandedState.Overview:
                    break;

                case LandedState.Exchange:
                    SelectSecond = false;
                    m_SelectIndex = 0;
                    m_SelectScrollOffset = 0;
                    m_SelectIndex2 = 0;
                    m_SelectScrollOffset2 = 0;
                    break;

                default:
                    break;
            }
        }

        public override void Draw(float frameSeconds)
        {
            int screenWidth = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = m_SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;

            m_SpriteBatch.GraphicsDevice.Clear(new Color(16, 0, 16, 255));

            m_Curses.Clear();

            switch (State)
            {
                case LandedState.Overview:
                    UpdateOverviewScreen();
                    break;

                case LandedState.Exchange:
                    UpdateExchangeScreen();
                    break;

                default:
                    m_Curses.WriteString(8, 6, "Error - LandedState is unknown value.");
                    break;
            }

            m_Curses.Render(m_SpriteBatch,
                new Vector2(
                    (screenWidth - m_Curses.DisplayWidth) / 2,
                    (screenHeight - m_Curses.DisplayHeight) / 2));
        }

        private void UpdateOverviewScreen()
        {
            m_Curses.WriteString(10, 4, Model.LandedOn.Name);
            m_Curses.WriteLines(10, 6, 60, Model.LandedOn.Description, '~');
            m_Curses.WriteString(80, 8, "[ 1 ] - Bar.");
            m_Curses.WriteString(80, 9, "[ 2 ] - Commodity Exchange.");
            m_Curses.WriteString(80, 10, "[ 3 ] - Ship outfitter.");
            m_Curses.WriteString(80, 11, "[ 4 ] - Shipyard.");
            m_Curses.WriteString(80, 12, "[Esc] - Lift off.");
        }

        private void UpdateExchangeScreen()
        {
            m_Curses.WriteString(10, 4, string.Format("{0} - Commodity Exchange", Model.LandedOn.Name));
            m_Curses.WriteLines(10, 6, 56, "The din of the exchange is the perfect background noise for the brokers haggling all around you.", '~');

            m_Curses.WriteBox(10, 11, 52, 13, Curses.CurseDecoration.Block);
            m_Curses.WriteString(14, 13, string.Format("{0,-30}{1,-10}{2}", "Commodities in Hold", "Amount", "Sell at"));

            m_Curses.WriteBox(65, 11, 52, 13, Curses.CurseDecoration.Block);
            m_Curses.WriteString(69, 13, string.Format("{0,-30}{1,-10}{2}", "Commodities for Sale", "Amount", "Buy at"));

            for (int i = 0; i < Commodities.List.Count; i++)
            {
                Commodity c = Commodities.List[i];
                m_Curses.WriteString(14, 15 + i, string.Format("{0,-30}{1,-10}{2}", c.Name, 500 / (i + 1), c.Cost));
            }

            m_Curses.WriteString(10, 27, "[ w ] - Scroll up.");
            m_Curses.WriteString(10, 28, "[ d ] - Scroll down.");
            m_Curses.WriteString(10, 29, "[Ent] - Sell.");
            m_Curses.WriteString(10, 30, "[Tab] - Switch to buying.");
            m_Curses.WriteString(10, 31, "[Esc] - Leave the exchange.");

            if (SelectSecond)
            {
                m_Curses.WriteString(67, SelectIndex + 15, "\x10");
            }
            else
            {
                int maxScrollOffset = Data.Commodities.List.Count - 8;
                if (SelectScrollOffset > 0)
                    m_Curses.WriteString(12, 14, "\x1E");
                if (SelectScrollOffset < maxScrollOffset)
                    m_Curses.WriteString(12, 23, "\x1F");
                m_Curses.WriteString(13, SelectIndex + 15, "\x10");
            }
        }
    }
}
