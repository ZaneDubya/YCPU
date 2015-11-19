using Ypsilon.Core.Patterns.MVC;
using Ypsilon.World.Data;
using Ypsilon.World.Entities;
using Ypsilon.PlayerState;

namespace Ypsilon.World
{
    class WorldModel : AModel
    {
        internal EntityManager Entities
        {
            get;
            private set;
        }

        public static Serial PlayerSerial = Serial.Null;
        public static Serial SelectedSerial = Serial.Null;

        public WorldModel()
        {
            Entities = new EntityManager();

            // create player
            Ship player = Entities.GetEntity<Ship>(Serial.GetNext(), true);
            WorldModel.PlayerSerial = player.Serial;
            // create other dudes.
            for (int i = 0; i < 100; i++)
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

        public override void Initialize()
        {

        }

        public override void Dispose()
        {

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
