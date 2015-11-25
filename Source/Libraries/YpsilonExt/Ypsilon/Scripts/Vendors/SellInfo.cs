using System;
using Ypsilon.Entities;

namespace Ypsilon.Scripts.Vendors
{
    /// <summary>
    /// Information about an item for sale from a vendor.
    /// </summary>
    class SellInfo
    {
        private int m_Price;

        public Type Type
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public float PriceDifferential
        {
            get;
            set;
        }

        public int Price
        {
            get
            {
                return (int)(m_Price * PriceDifferential);
            }
        }

        public int MaxAmount
        {
            get;
            set;
        }

        public int Amount
        {
            get;
            set;
        }

        public object[] Args
        {
            get;
            set;
        }

        public SellInfo(string name, float priceDiff, int maxAmount, Type type, object[] args)
        {
            Name = name;
            PriceDifferential = priceDiff;
            Amount = MaxAmount = maxAmount;
            Type = type;
            Args = args;

            // this is a horrible hackity hack hack hack please forgive me.
            AItem item = (AItem)Activator.CreateInstance(type);
            m_Price = item.Price;
        }

        /// <summary>
        /// get a new instance of an object (we just bought it)
        /// </summary>
        public virtual AEntity GetEntity()
        {
            if (Args == null || Args.Length == 0)
                return (AEntity)Activator.CreateInstance(Type);
            else
                return (AEntity)Activator.CreateInstance(Type, Args);
        }

        public void Restock()
        {
            Amount = MaxAmount;
        }
    }
}
