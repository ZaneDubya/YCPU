using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ypsilon.World.Entities;

namespace Ypsilon.World.Input
{
    class MouseOverList
    {
        private List<AEntity> m_Entities;

        public Vector3 MousePosition
        {
            get;
            set;
        }
        public bool HasEntities
        {
            get
            {
                return m_Entities.Count > 0;
            }
        }

        public MouseOverList()
        {
            m_Entities = new List<AEntity>();
        }

        public void Reset()
        {
            m_Entities.Clear();
        }

        public void Reset(Point mousePosition)
        {
            m_Entities.Clear();
            MousePosition = new Vector3(mousePosition.X, mousePosition.Y, 0);
        }

        public void AddEntityIfMouseIsOver(AEntity entity, Vector3 center)
        {
            BoundingSphere bounds = new BoundingSphere(center, entity.ViewSize);
            if (bounds.Contains(MousePosition) == ContainmentType.Contains)
            {
                m_Entities.Add(entity);
            }
        }
    }
}
