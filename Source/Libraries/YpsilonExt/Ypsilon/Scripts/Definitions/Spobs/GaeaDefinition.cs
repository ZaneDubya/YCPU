using Microsoft.Xna.Framework;
using Ypsilon.Data;

namespace Ypsilon.Scripts.Definitions.Spobs
{
    class GaeaDefinition : ASpobDefinition
    {
        public override string Name { get { return "Gaea"; } }
        public override string Description { get { return "Gaea is just slightly smaller than Terra and has almost the same mass and gravity. The surface is about 60 percent water. ~Because of its distance from Beta Caeli, the surface temperature is slightly cooler than Terra's. The planet does not have as many ore deposits as its neighbor, Rhea. ~The planet has an oxygen-nitrogen-carbon dioxide atmosphere very similar to Terra's. The oxygen and the detectable traces of methane indicate that life is present on the planet. ~Distance from Star: 2.54.06x10(8) kilometers. Period of revolution (Terran): 793.98 days. Period of rotation (Terran): 22.32 hours. Equatorial diameter: 12,224 kilometers. Mass (Terra=1): 0.97. Density (water=1): 5.4. Atmosphere: Nitrogen, Oxygen. ~Mean temperature (visible surface; Celsius): 20 equator/-2 polar. Surface gravity (Terra=1): 0.96. Inclination of axis: 27.24 degrees. Inclination of orbit to ecliptic: 0.0 degrees. Eccentricity of orbit: 0.10410."; } }
        public override Color Color { get { return Color.CornflowerBlue; } }
        public override float Size { get { return 60f; } }
    }
}
