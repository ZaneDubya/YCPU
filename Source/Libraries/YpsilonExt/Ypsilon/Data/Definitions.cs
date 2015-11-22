using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Ypsilon.Data
{
    static class Definitions
    {
        private static Dictionary<string, ASpobDefinition> m_Spobs = new Dictionary<string, ASpobDefinition>();

        public static ASpobDefinition GetSpob(string key)
        {
            key = key.ToLower();

            ASpobDefinition spob;
            if (m_Spobs.TryGetValue(key, out spob))
                return spob;

            return null;
        }

        public static void AddSpob(string key, ASpobDefinition spob)
        {
            key = key.ToLower();

            if (m_Spobs.ContainsKey(key))
                throw new Exception("Cannot add spob with duplicate key '" + key + "'.");

            m_Spobs.Add(key, spob);
        }

        static Definitions()
        {
            AddSpob("Gaea", new ASpobDefinition()
            {
                Color = Color.CornflowerBlue,
                Size = 60f,
                VertexRandomizationFactor = 0f,
                Name = "Gaea",
                Description = "Gaea is just slightly smaller than Terra and has almost the same mass and gravity. The surface is about 60 percent water. ~Because of its distance from Beta Caeli, the surface temperature is slightly cooler than Terra's. The planet does not have as many ore deposits as its neighbor, Rhea. ~The planet has an oxygen-nitrogen-carbon dioxide atmosphere very similar to Terra's. The oxygen and the detectable traces of methane indicate that life is present on the planet. ~Distance from Star: 2.54.06x10(8) kilometers. Period of revolution (Terran): 793.98 days. Period of rotation (Terran): 22.32 hours. Equatorial diameter: 12,224 kilometers. Mass (Terra=1): 0.97. Density (water=1): 5.4. Atmosphere: Nitrogen, Oxygen. ~Mean temperature (visible surface; Celsius): 20 equator/-2 polar. Surface gravity (Terra=1): 0.96. Inclination of axis: 27.24 degrees. Inclination of orbit to ecliptic: 0.0 degrees. Eccentricity of orbit: 0.10410."
            });

            AddSpob("Asteroid Small", new ASpobDefinition()
            {
                Color = Color.LightGray,
                Size = 10f,
                RotationPeriod = 10f,
                VertexRandomizationFactor = 0.6f,
                Name = "Class B Asteroid",
                Description = "The Alpha Asteroids are the remnants of a planet that  either never formed or was torn apart by the gravitational forces of Beta Caeli. Most of the lighter carbonaceous and stony asteroids have been ejected from the belt, leaving the heavier metallic asteroids behind. ~It is estimated from the combined mass of the metallic Alpha Asteroids that a planet two-thirds the size of Mercury could have once existed. Estimates put the combined mass of the Alpha Asteroids at 4.23x10(18) kilograms. ~The Alpha Asteroids are primarily found between 55 and 74 million kilometers from Beta Caeli."
            });

            AddSpob("Asteroid Large", new ASpobDefinition()
            {
                Color = Color.Gray,
                Size = 28f,
                RotationPeriod = 20f,
                VertexRandomizationFactor = 0.4f,
                Name = "Class D Asteroid"
            });
        }
    }
}
