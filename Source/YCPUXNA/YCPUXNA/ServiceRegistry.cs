﻿using System;
using System.Collections.Generic;

namespace YCPUXNA
{
    public class ServiceRegistry
    {
        private Dictionary<Type, object> m_Services = new Dictionary<Type, object>();

        public T Register<T>(T service)
        {
            Type type = typeof(T);

            if (m_Services.ContainsKey(type))
            {
                // Tracer.Critical(string.Format("Attempted to register service of type {0} twice", type.ToString()));
                m_Services.Remove(type);
            }

            m_Services.Add(type, service);
            return service;
        }

        public void Unregister<T>()
        {
            Type type = typeof(T);

            if (m_Services.ContainsKey(type))
            {
                m_Services.Remove(type);
            }
        }

        public bool ServiceExists<T>()
        {
            Type type = typeof(T);

            if (m_Services.ContainsKey(type))
            {
                return true;
            }
            return false;
        }

        public T GetService<T>()
        {
            Type type = typeof(T);

            if (m_Services.ContainsKey(type))
            {
                return (T)m_Services[type];
            }
            // Tracer.Critical(string.Format("Attempted to get service service of type {0}, but no service of this type is registered", type.ToString()));
            return default(T);
        }
    }
}
