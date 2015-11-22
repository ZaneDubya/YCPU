using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Core.Patterns.MVC;

namespace Ypsilon.Modes.Landed
{
    class LandedView : AView
    {
        private SpriteBatchExtended m_SpriteBatch;
        private VectorRenderer m_Vectors;
        private Curses m_Curses;

        public LandedView(LandedModel model)
            : base(model)
        {
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatchExtended>();
            m_Vectors = ServiceRegistry.GetService<VectorRenderer>();
            m_Curses = new Curses(m_SpriteBatch.GraphicsDevice, 128, 40, @"Content\BIOS8x16.png");
        }

        public override void Draw(float frameSeconds)
        {
            m_SpriteBatch.GraphicsDevice.Clear(new Color(64, 0, 16, 255));
        }
    }
}
