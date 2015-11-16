using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Core.Patterns.MVC;
using Ypsilon.World.Entities;
using Ypsilon.World.Data;

namespace Ypsilon.World
{
    class WorldModel : AModel
    {
        internal EntityManager Entities
        {
            get;
            private set;
        }

        public WorldModel()
        {
            Entities = new EntityManager();

            // create player
            Ship player = Entities.GetEntity<Ship>(Serial.GetNext(), true);
            World.State.Vars.PlayerSerial = player.Serial;
            // create other dudes.
            for (int i = 0; i < 30; i++)
            {
                Ship ship = Entities.GetEntity<Ship>(Serial.GetNext(), true);
                ship.Position = new Position3D(
                    Utility.Random_GetNonpersistantDouble() * 600 - 300,
                    Utility.Random_GetNonpersistantDouble() * 600 - 300,
                    0);
            }

            // create a planet.
            Spob planet = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            planet.Definition = Definitions.GetSpob("planet");

            Spob asteroid1 = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid1.Definition = Definitions.GetSpob("asteroid small");
            asteroid1.Position = new Position3D(100, 100, 0);

            Spob asteroid2 = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid2.Definition = Definitions.GetSpob("asteroid large");
            asteroid2.Position = new Position3D(-100, 200, 0);
        }

        public override void Update(float totalSeconds, float frameSeconds)
        {
            Entities.Update(frameSeconds);
        }

        protected override AController CreateController()
        {
            return new WorldController(this);
        }

        protected override AView CreateView()
        {
            return new WorldView(this);
        }
    }
}
