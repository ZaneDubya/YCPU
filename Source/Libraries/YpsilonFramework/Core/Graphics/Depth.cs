using Microsoft.Xna.Framework;

namespace Ypsilon.Core.Graphics {
    public static class Depth {
        private static Vector3 s_ZOffset = Vector3.Zero;

        public static Vector3 NextZ {
            get {
                s_ZOffset.Z++;
                return s_ZOffset;
            }
        }

        public static void IncrementBy1000() {
            s_ZOffset.Z += 1000;
        }

        public static void ResetZ() {
            s_ZOffset = Vector3.Zero;
        }
    }
}