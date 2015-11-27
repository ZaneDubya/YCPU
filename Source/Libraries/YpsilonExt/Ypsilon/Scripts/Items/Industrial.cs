using Ypsilon.Data;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items
{
    class Industrial : AItem
    {
        public override string DefaultName { get { return "Industrial"; } }

        static Industrial()
        {
            Prices.AddPrice(typeof(Industrial), 150);
        }
    }
}
