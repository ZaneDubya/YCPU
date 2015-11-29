using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Ypsilon.Data
{
    /// <summary>
    /// Ship definition keeps all essential ship variables organized.
    /// 1. The base values of the variables in the definition are loaded from a file.
    /// 2. Eventually, the ship definition will query a ship's contents to get the current values of these variables.
    /// 3. Right now, everything is hardcoded.
    /// </summary>
    public class AShipDefinition
    {
        /// <summary>
        /// A list of places where modules can be installed.
        /// </summary>
        public List<Point> Hardpoints = new List<Point>() {
            new Point(0, 0), new Point(1, 0),
            new Point(0, 1), new Point(1, 1),
            new Point(0, 2), new Point(1, 2),
            new Point(0, 3), new Point(1, 3) };

        /// <summary>
        /// Space available to add additional items and upgrades. 
        /// </summary>
        public int DefaultExpansionSpace = 100;

        /// <summary>
        /// Shield strength, in standard shield units.
        /// </summary>
        public int DefaultShield = 100;

        /// <summary>
        /// Shield recharge speed, in amount per second.
        /// </summary>
        public int DefaultShieldRechargeRate = 10;

        /// <summary>
        /// Armor strength, in standard armor units.
        /// </summary>
        public int DefaultArmor = 100;

        /// <summary>
        /// Units per second added to or subtracted from max speed.
        /// </summary>
        public int DefaultAcceleration = 100;

        /// <summary>
        /// Units per second, max speed. 10 units is 1 pixel.
        /// </summary>
        public int DefaultSpeed = 1000;

        /// <summary>
        /// Degrees per second rotation.
        /// </summary>
        public int DefaultRotation = 60;

        /// <summary>
        /// Fuel capacity. 100 = 1 jump.
        /// </summary>
        public int DefaultEnergyCapacity = 500;


        public float Size = 7.5f;
    }
}
