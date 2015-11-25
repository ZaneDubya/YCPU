using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Data;
using Ypsilon.Entities;
using Ypsilon.Modes.Space;
using Ypsilon.Modes.Space.Entities;

namespace Ypsilon.Persistence
{
    class Savegame
    {
        public static void Load()
        {
            World.Entities.Reset(true);

            // create player dude
            Ship player = World.Entities.GetEntity<Ship>(Serial.Next, true);
            PlayerState.Vars.PlayerSerial = player.Serial;
            player.Name = "Player";
            player.SetComponent(new ShipSpaceComponent(player));
            // create other dudes.
            for (int i = 0; i < 100; i++)
            {
                Ship ship = World.Entities.GetEntity<Ship>(Serial.Next, true);
                ship.Name = "Enemy ship";
                ShipSpaceComponent c = (ShipSpaceComponent)ship.SetComponent(new ShipSpaceComponent(ship));
                c.Position = new Position3D(
                    Utility.Random_GetNonpersistantDouble() * 1000 - 500,
                    Utility.Random_GetNonpersistantDouble() * 600 - 300,
                    0);
            }

            // create a planet.
            Spob planet = World.Entities.GetEntity<Spob>(Serial.Next, true);
            planet.Definition = Definitions.GetSpob("gaea");
            planet.CanLandHere = true;
            SpobSpaceComponent planetComponent = (SpobSpaceComponent)planet.SetComponent(new SpobSpaceComponent(planet));
            planetComponent.Position = new Position3D(0, 0, 0);

            Spob asteroid1 = World.Entities.GetEntity<Spob>(Serial.Next, true);
            asteroid1.Definition = Definitions.GetSpob("asteroid small");
            SpobSpaceComponent asteroid1c = (SpobSpaceComponent)asteroid1.SetComponent(new SpobSpaceComponent(asteroid1));
            asteroid1c.Position = new Position3D(100, 100, 0);

            Spob asteroid2 = World.Entities.GetEntity<Spob>(Serial.Next, true);
            asteroid2.Definition = Definitions.GetSpob("asteroid large");
            SpobSpaceComponent asteroid2c = (SpobSpaceComponent)asteroid2.SetComponent(new SpobSpaceComponent(asteroid2));
            asteroid2c.Position = new Position3D(-100, 200, 0);
        }
    }
}
