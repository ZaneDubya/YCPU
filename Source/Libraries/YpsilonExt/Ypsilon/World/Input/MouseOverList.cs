using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ypsilon.World.Entities;

namespace Ypsilon.World.Input
{
    class MouseOverList
    {
        private List<AEntity> m_Entities;
        private Vector3 m_MousePosition;

        public Vector3 ScreenMousePosition
        {
            get
            {
                return m_MousePosition;
            }
            set
            {
                m_MousePosition = value;
                Reset();
            }
        }

        public Vector3 WorldMousePosition
        {
            get; set;
        }

        public bool HasEntities
        {
            get
            {
                return m_Entities.Count > 0;
            }
        }

        public AEntity GetFirstEntity()
        {
            if (m_Entities.Count == 0)
                return null;
            return m_Entities[0];
        }

        public MouseOverList()
        {
            m_Entities = new List<AEntity>();
        }

        public void Reset()
        {
            m_Entities.Clear();
        }

        public void AddEntityIfMouseIsOver(AEntity entity, Vector3 center)
        {
            if (m_Entities.Contains(entity))
                return;

            BoundingSphere bounds = new BoundingSphere(center, entity.ViewSize);
            if (bounds.Contains(WorldMousePosition) == ContainmentType.Contains)
            {
                m_Entities.Add(entity);
            }
        }

        
    }
}
