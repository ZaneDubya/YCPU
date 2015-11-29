/***************************************************************************
 *   EntityList.cs
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
#endregion

namespace Ypsilon.Entities
{
    /// <summary>
    /// A collection of entities. Each 'place of interest' is its own world, and will maintain its own copy of these objects.
    /// </summary>
    public class EntityList
    {
        private Dictionary<int, AEntity> m_Entities = new Dictionary<int, AEntity>();
        private List<AEntity> m_Entities_Queued = new List<AEntity>();

        private bool m_EntitiesCollectionIsLocked = false;
        private List<int> m_SerialsToRemove = new List<int>();

        public Dictionary<int, AEntity> AllEntities
        {
            get
            {
                return m_Entities;
            }
        }

        public EntityList()
        {

        }

        public void Reset(bool clearPlayerEntity = false)
        {
            if (clearPlayerEntity)
            {
                m_Entities.Clear();
                foreach (AEntity entity in m_Entities.Values)
                {
                    entity.Dispose();
                }
            }
            else
            {
                foreach (AEntity entity in m_Entities.Values)
                {
                    if (!entity.IsPlayerEntity)
                        entity.Dispose();
                }
                AEntity player = GetPlayerEntity();
                m_Entities.Clear();
                if (player != null)
                    m_Entities.Add(player.Serial, player);
            }
        }

        public AEntity GetPlayerEntity()
        {
            // This could be cached to save time.
            if (m_Entities.ContainsKey(World.PlayerSerial))
                return (AEntity)m_Entities[World.PlayerSerial];
            else
                return null;
        }

        public void Update(float frameSeconds)
        {
            // redirect any new entities to a queue while we are enumerating the collection.
            m_EntitiesCollectionIsLocked = true;

            foreach (KeyValuePair<int, AEntity> entity in m_Entities)
            {
                if (!entity.Value.IsDisposed)
                {
                    entity.Value.Update(frameSeconds);
                }
                else
                {
                    m_SerialsToRemove.Add(entity.Key);
                }
            }

            // Remove disposed entities
            foreach (int i in m_SerialsToRemove)
            {
                m_Entities.Remove(i);
            }
            m_SerialsToRemove.Clear();

            // stop redirecting new entities to the queue and add any queued entities to the main entity collection.
            m_EntitiesCollectionIsLocked = false;
            foreach (AEntity e in m_Entities_Queued)
                m_Entities.Add(e.Serial, e);
            m_Entities_Queued.Clear();
        }

        public AEntity GetEntity(Serial serial)
        {
            // Check for existence in the collection.
            if (m_Entities.ContainsKey(serial))
            {
                // This object is in the m_entities collection. If it is being disposed, then we should complete disposal
                // of the object and then return a new object. If it is not being disposed, return the object in the collection.
                AEntity entity = (AEntity)m_Entities[serial];
                if (entity.IsDisposed)
                {
                    m_Entities.Remove(serial);
                    return null;
                }
                return entity;
            }

            // No object with this Serial is in the collection.
            return null;
        }

        public AEntity AddEntity(Type type, params object[] args)
        {
            AEntity entity = InternalAddEntity(type, args);
            return entity;
        }

        public void RemoveEntity(Serial serial)
        {
            if (m_Entities.ContainsKey(serial))
            {
                // dispose of the entity - it will be removed on next update.
                m_Entities[serial].Dispose();
            }
        }

        private AEntity InternalAddEntity(Type type, params object[] args)
        {
            AEntity entity = AEntity.CreateEntity(type, args);

            // If the entities collection is locked, add the new entity to the queue. Otherwise 
            // add it directly to the main entity collection.
            if (m_EntitiesCollectionIsLocked)
                m_Entities_Queued.Add(entity);
            else
                m_Entities.Add(entity.Serial, entity);

            return (AEntity)entity;
        }
    }
}

