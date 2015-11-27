using Ypsilon.Data;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items
{
    class Chocolate : AItem
    {
        public override string DefaultName { get { return "Chocolate"; } }

        static Chocolate()
        {
            Prices.AddPrice(typeof(Chocolate), 150);
        }
    }
}
