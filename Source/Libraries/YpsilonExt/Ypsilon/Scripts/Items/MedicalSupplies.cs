using Ypsilon.Data;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Items
{
    class MedicalSupplies : AItem
    {
        public override string DefaultName { get { return "Medical Supplies"; } }

        static MedicalSupplies()
        {
            Prices.AddPrice(typeof(MedicalSupplies), 150);
        }
    }
}
