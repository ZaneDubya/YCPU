using Ypsilon.Data;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items
{
    class Food : AItem
    {
        public override string DefaultName { get { return "Food"; } }

        static Food()
        {
            Prices.AddPrice(typeof(Food), 150);
        }
    }
}
