using Microsoft.Xna.Framework;
using Ypsilon.Entities;
using Ypsilon.Modes.Space;
using Ypsilon.Modes.Space.Entities;
using Ypsilon.Scripts.Modules;

namespace Ypsilon.Persistence
{
    class Savegame
    {
        public static void Load()
        {
            World.Entities.Reset(true);

            // create player dude
            Ship player = (Ship)World.Entities.AddEntity(typeof(Ship));
            World.PlayerSerial = player.Serial;
            player.Name = "Player";
            player.SetComponent(new ShipSpaceComponent(player));
            player.Modules.TryAddModule(new CargoModule(), new Point(0, 0));
            player.Modules.TryAddModule(new LaserModule(), new Point(1, 0));
            
            // create other dudes.
            for (int i = 0; i < 100; i++)
            {
                Ship ship = (Ship)World.Entities.AddEntity(typeof(Ship));
                ship.Name = "Enemy ship";
                ShipSpaceComponent c = (ShipSpaceComponent)ship.SetComponent(new ShipSpaceComponent(ship));
                c.Position = new Position3D(
                    Utility.Random_GetNonpersistantDouble() * 1000 - 500,
                    Utility.Random_GetNonpersistantDouble() * 600 - 300,
                    0);
            }

            // create a planet.
            Spob planet = (Spob)World.Entities.AddEntity(typeof(Spob));
            planet.CanLandHere = true;
            planet.Name = "Gaea";
            planet.Description = "Gaea is just slightly smaller than Terra and has almost the same mass and gravity. The surface is about 60 percent water. ~Because of its distance from Beta Caeli, the surface temperature is slightly cooler than Terra's. The planet does not have as many ore deposits as its neighbor, Rhea. ~The planet has an oxygen-nitrogen-carbon dioxide atmosphere very similar to Terra's. The oxygen and the detectable traces of methane indicate that life is present on the planet. ~Distance from Star: 2.54.06x10(8) kilometers. Period of revolution (Terran): 793.98 days. Period of rotation (Terran): 22.32 hours. Equatorial diameter: 12,224 kilometers. Mass (Terra=1): 0.97. Density (water=1): 5.4. Atmosphere: Nitrogen, Oxygen. ~Mean temperature (visible surface; Celsius): 20 equator/-2 polar. Surface gravity (Terra=1): 0.96. Inclination of axis: 27.24 degrees. Inclination of orbit to ecliptic: 0.0 degrees. Eccentricity of orbit: 0.10410.";
            planet.Color = Color.CornflowerBlue;
            planet.Size = 60f;
            SpobSpaceComponent planetComponent = (SpobSpaceComponent)planet.SetComponent(new SpobSpaceComponent(planet));
            planetComponent.Position = new Position3D(0, 0, 0);

            Spob asteroid1 = (Spob)World.Entities.AddEntity(typeof(Spob));
            asteroid1.Name = "Class B Asteroid";
            asteroid1.Description = "The Alpha Asteroids are the remnants of a planet that  either never formed or was torn apart by the gravitational forces of Beta Caeli. Most of the lighter carbonaceous and stony asteroids have been ejected from the belt, leaving the heavier metallic asteroids behind. ~It is estimated from the combined mass of the metallic Alpha Asteroids that a planet two-thirds the size of Mercury could have once existed. Estimates put the combined mass of the Alpha Asteroids at 4.23x10(18) kilograms. ~The Alpha Asteroids are primarily found between 55 and 74 million kilometers from Beta Caeli.";
            asteroid1.Color = Color.LightGray;
            asteroid1.Size = 10f;
            asteroid1.RotationPeriod = 12f;
            asteroid1.VertexRandomizationFactor = 0.6f;
            SpobSpaceComponent asteroid1c = (SpobSpaceComponent)asteroid1.SetComponent(new SpobSpaceComponent(asteroid1));
            asteroid1c.Position = new Position3D(100, 100, 0);

            Spob asteroid2 = (Spob)World.Entities.AddEntity(typeof(Spob));
            asteroid2.Name = "Class D Asteroid";
            asteroid2.Color = Color.Gray;
            asteroid2.Size = 28f;
            asteroid2.RotationPeriod = 18f;
            asteroid2.VertexRandomizationFactor = 0.4f;
            SpobSpaceComponent asteroid2c = (SpobSpaceComponent)asteroid2.SetComponent(new SpobSpaceComponent(asteroid2));
            asteroid2c.Position = new Position3D(-100, 200, 0);
        }
    }
}
