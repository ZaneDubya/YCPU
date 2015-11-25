using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Entities;

namespace Ypsilon
{
    public static class World
    {
        public static float PlayerCredits = 10005;
        public static Serial PlayerSerial = Serial.Null;

        public static EntityManager Entities
        {
            get;
            private set;
        }

        public static void Update(float totalSeconds, float frameSeconds)
        {
            Entities.Update(frameSeconds);
        }

        static World()
        {
            Entities = new EntityManager();
        }
    }
}
