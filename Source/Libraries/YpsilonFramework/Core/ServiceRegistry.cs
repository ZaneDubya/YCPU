﻿using System;
using System.Collections.Generic;

namespace Ypsilon.Core {
    public static class ServiceRegistry {
        private static readonly Dictionary<Type, object> m_Services = new Dictionary<Type, object>();

        public static T GetService<T>() {
            Type type = typeof(T);
            if (m_Services.ContainsKey(type)) {
                return (T)m_Services[type];
            }
            else {
                // Tracer.Critical(string.Format("Attempted to get service service of type {0}, but no service of this type is registered.", type.ToString()));
                return default(T);
            }
        }

        public static T Register<T>(T service) {
            Type type = typeof(T);
            if (m_Services.ContainsKey(type)) {
                // Tracer.Critical(string.Format("Attempted to register service of type {0} twice.", type.ToString()));
                m_Services.Remove(type);
            }
            m_Services.Add(type, service);
            return service;
        }

        public static bool ServiceExists<T>() {
            Type type = typeof(T);
            if (m_Services.ContainsKey(type)) {
                return true;
            }
            else {
                return false;
            }
        }

        public static void Unregister<T>() {
            Type type = typeof(T);
            if (m_Services.ContainsKey(type)) {
                m_Services.Remove(type);
            }
            else {
                // Tracer.Critical(string.Format("Attempted to unregister service of type {0}, but no service of this type (or type and equality) is registered.", type.ToString()));
            }
        }
    }
}