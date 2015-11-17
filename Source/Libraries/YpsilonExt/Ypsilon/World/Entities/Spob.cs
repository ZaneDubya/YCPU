using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.World.Crafting;
using Ypsilon.World.Entities.Movement;
using Ypsilon.World.Input;

namespace Ypsilon.World.Entities
{
    /// <summary>
    /// A space object.
    /// </summary>
    class Spob : AEntity
    {
        public ASpobDefinition Definition = new ASpobDefinition();

        private PlanetRotator m_Rotator;
        private Vector3[] m_ModelVertices;

        public override float ViewSize
        {
            get
            {
                return Definition.Size;
            }
        }

        public Spob(EntityManager manager, Serial serial)
            : base(manager, serial)
        {
            
        }

        protected override void OnInitialize()
        {
            m_ModelVertices = Vertices.GetSpobVertices(Definition);
            m_Rotator = new PlanetRotator(Definition);
        }

        public override void Update(float frameSeconds)
        {
            m_Rotator.Rotation = m_Rotator.Rotation + frameSeconds / Definition.RotationPeriod;
        }

        public override void Draw(VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            Vector3 translation = (Position - worldSpaceCenter).ToVector3();
            DrawMatrix = CreateWorldMatrix(translation);
            DrawVertices = m_ModelVertices;
            DrawColor = Definition.Color;

            base.Draw(renderer, worldSpaceCenter, mouseOverList);
        }

        private Matrix CreateWorldMatrix(Vector3 translation)
        {
            Matrix rotMatrix = m_Rotator.RotationMatrix;

            Vector3 forward = rotMatrix.Forward;
            forward.Normalize();

            Vector3 up = rotMatrix.Up;
            up.Normalize();

            return Matrix.CreateWorld(translation, forward, up);
        }

        public float ExtractOre(float frameSeconds)
        {
            float amount = (ResourceOre > frameSeconds) ? frameSeconds : ResourceOre;
            ResourceOre -= amount;
            return amount;
        }

        public float ResourceOre = 100f;
    }
}
