using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Data;
using Ypsilon.Entities;
using Ypsilon.Modes.Space;
using Ypsilon.Modes.Space.Entities;

namespace Ypsilon.Persistence
{
    class Savegame
    {
        public static AModel Load()
        {
            SpaceModel model = new SpaceModel();

            // create player dude
            Ship player = model.Entities.GetEntity<Ship>(Serial.GetNext(), true);
            PlayerState.Vars.PlayerSerial = player.Serial;
            player.Name = "Player";
            player.SetComponent(new ShipSpaceComponent(player));
            // create other dudes.
            for (int i = 0; i < 100; i++)
            {
                Ship ship = model.Entities.GetEntity<Ship>(Serial.GetNext(), true);
                ship.Name = "Enemy ship";
                ShipSpaceComponent c = (ShipSpaceComponent)ship.SetComponent(new ShipSpaceComponent(ship));
                c.Position = new Position3D(
                    Utility.Random_GetNonpersistantDouble() * 1000 - 500,
                    Utility.Random_GetNonpersistantDouble() * 600 - 300,
                    0);
            }

            // create a planet.
            Spob planet = model.Entities.GetEntity<Spob>(Serial.GetNext(), true);
            planet.Definition = Definitions.GetSpob("gaea");
            planet.CanLandHere = true;
            SpobSpaceComponent planetComponent = (SpobSpaceComponent)planet.SetComponent(new SpobSpaceComponent(planet));
            planetComponent.Position = new Position3D(0, 0, 0);

            Spob asteroid1 = model.Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid1.Definition = Definitions.GetSpob("asteroid small");
            SpobSpaceComponent asteroid1c = (SpobSpaceComponent)asteroid1.SetComponent(new SpobSpaceComponent(asteroid1));
            asteroid1c.Position = new Position3D(100, 100, 0);

            Spob asteroid2 = model.Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid2.Definition = Definitions.GetSpob("asteroid large");
            SpobSpaceComponent asteroid2c = (SpobSpaceComponent)asteroid2.SetComponent(new SpobSpaceComponent(asteroid2));
            asteroid2c.Position = new Position3D(-100, 200, 0);

            return model;
        }
    }
}
