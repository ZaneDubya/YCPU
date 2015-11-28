using Microsoft.Xna.Framework;
using System;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Entities;
using Ypsilon.Scripts.Vendors;

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
            m_Curses.WriteString(80, 6, "[ 1 ] - Bar.");
            m_Curses.WriteString(80, 7, "[ 2 ] - Commodity Exchange.");
            m_Curses.WriteString(80, 8, "[ 3 ] - Ship outfitter.");
            m_Curses.WriteString(80, 9, "[ 4 ] - Shipyard.");
            m_Curses.WriteString(80, 10, "[Esc] - Lift off.");
        }

        private void UpdateExchangeScreen()
        {
            Ship player = (Ship)World.Entities.GetPlayerEntity();
            VendorInfo vendor = Model.LandedOn.Exchange;

            m_Curses.WriteString(10, 4, string.Format("{0} - Commodity Exchange", Model.LandedOn.Name));
            m_Curses.WriteLines(10, 6, 56, "The din of the exchange is the perfect background noise for the brokers haggling all around you.", '~');

            // ======================================================================
            // player hold - items for sale by player.
            // ======================================================================
            int saleIndex = 0;
            Point salePosition = new Point(10, 9);
            m_Curses.WriteBox(salePosition.X, salePosition.Y, 52, 13, Curses.CurseDecoration.Block);
            m_Curses.WriteString(salePosition.X + 4, salePosition.Y + 2, string.Format("{0,-30}{1,-10}{2}", "Commodities in Hold", "Amount", "Price"));
            
            for (int i = m_SelectScrollOffset; i < Model.BuyInfo.Types.Length; i++)
            {
                Type itemType = Model.BuyInfo.Types[i];
                AItem item;
                if (!player.Inventory.TryGetItem(itemType, out item)) // this will always succeed.
                    throw new Exception("LandedView.UpdateExchangeScreen() - could not retrieve item for sale.");

                int price = Model.BuyInfo.GetPurchasePrice(item);

                m_Curses.WriteString(salePosition.X + 4, salePosition.Y + 4 + saleIndex, string.Format("{0,-30}{1,-10}{2}",
                    item.Name, item.Amount, price));
                saleIndex++;
                if (saleIndex >= 8)
                    break;
            }

            // ======================================================================
            // spob vendor - items for purchase by player.
            // ======================================================================
            int purchaseIndex = 0;
            Point purPosition = new Point(10, 24);
            m_Curses.WriteBox(purPosition.X, purPosition.Y, 52, 13, Curses.CurseDecoration.Block);
            // string.Empty in the following line used to be "Amount". We aren't tracking amounts at the moment.
            m_Curses.WriteString(purPosition.X + 4, purPosition.Y + 2, string.Format("{0,-30}{1,-10}{2}", "Commodities for Sale", string.Empty, "Price"));
            
            for (int i = m_SelectScrollOffset2; i < vendor.Selling.Count; i++)
            {
                SellInfo info = vendor.Selling[i];
                // string.Empty in the following line used to be info.Amount. We aren't tracking amounts at the moment.
                m_Curses.WriteString(purPosition.X + 4, purPosition.Y + 4 + purchaseIndex, string.Format("{0,-30}{1,-10}{2}", info.Name, string.Empty, info.Price));
                purchaseIndex++;
                if (purchaseIndex >= 8)
                    break;
            }

            // ======================================================================
            // scrolling
            // ======================================================================
            if (SelectSecond)
            {
                int maxScrollOffset = vendor.Selling.Count - 8;
                if (SelectScrollOffset > 0)
                    m_Curses.WriteString(purPosition.X + 2, purPosition.Y + 3, "\x1E"); // up
                if (SelectScrollOffset < maxScrollOffset)
                    m_Curses.WriteString(purPosition.X + 2, purPosition.Y + 12, "\x1F"); // dn
                m_Curses.WriteString(purPosition.X + 3, purPosition.Y + 4 + SelectIndex, "\x10"); // right
            }
            else
            {
                int maxScrollOffset = Model.BuyInfo.Types.Length - 8;
                if (SelectScrollOffset > 0)
                    m_Curses.WriteString(salePosition.X + 2, salePosition.Y + 3, "\x1E"); // up
                if (SelectScrollOffset < maxScrollOffset)
                    m_Curses.WriteString(salePosition.X + 2, salePosition.Y + 12, "\x1F"); // dn
                m_Curses.WriteString(salePosition.X + 3, salePosition.Y + 4 + SelectIndex, "\x10"); // right
            }

            // ======================================================================
            // RIGHT - money money money
            // ======================================================================
            m_Curses.WriteString(80, 4, string.Format("Credits: {0}", World.PlayerCredits));

            // ======================================================================
            // RIGHT - explanation of controls.
            // ======================================================================
            m_Curses.WriteString(80, 6, "[ w ] - Scroll up.");
            m_Curses.WriteString(80, 7, "[ d ] - Scroll down.");
            m_Curses.WriteString(80, 8, string.Format("[Ent] - {0}.", SelectSecond ? "Purchase" : "Sell"));
            m_Curses.WriteString(80, 9, string.Format("[Tab] - Switch to {0}.", SelectSecond ? "selling" : "purchasing"));
            m_Curses.WriteString(80, 10, "[Esc] - Leave the exchange.");
        }
    }
}
