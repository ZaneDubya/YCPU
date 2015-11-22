using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Data;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Entities;

namespace Ypsilon.Modes.Space
{
    class SpaceModel : AModel
    {
        public Serial SelectedSerial
        {
            get;
            set;
        }

        internal EntityManager Entities
        {
            get;
            private set;
        }

        public SpaceModel()
        {
            Entities = new EntityManager();
            SelectedSerial = Serial.Null;

            // create player dude
            Ship player = Entities.GetEntity<Ship>(Serial.GetNext(), true);
            PlayerState.Vars.PlayerSerial = player.Serial;
            player.Name = "Player";
            player.SetComponent(new ShipSpaceComponent(player));
            // create other dudes.
            for (int i = 0; i < 100; i++)
            {
                Ship ship = Entities.GetEntity<Ship>(Serial.GetNext(), true);
                ship.Name = "Enemy ship";
                ShipSpaceComponent c = (ShipSpaceComponent)ship.SetComponent(new ShipSpaceComponent(ship));
                c.Position = new Position3D(
                    Utility.Random_GetNonpersistantDouble() * 1000 - 500,
                    Utility.Random_GetNonpersistantDouble() * 600 - 300,
                    0);
            }

            // create a planet.
            Spob planet = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            planet.Definition = Definitions.GetSpob("planet");
            planet.Name = "Earth";
            planet.CanLandHere = true;
            SpobSpaceComponent planetComponent = (SpobSpaceComponent)planet.SetComponent(new SpobSpaceComponent(planet));
            planetComponent.Position = new Position3D(0, 0, 0);

            Spob asteroid1 = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid1.Definition = Definitions.GetSpob("asteroid small");
            asteroid1.Name = "Class B Asteroid";
            SpobSpaceComponent asteroid1c = (SpobSpaceComponent)asteroid1.SetComponent(new SpobSpaceComponent(asteroid1));
            asteroid1c.Position = new Position3D(100, 100, 0);

            Spob asteroid2 = Entities.GetEntity<Spob>(Serial.GetNext(), true);
            asteroid2.Definition = Definitions.GetSpob("asteroid large");
            asteroid2.Name = "Class D Asteroid";
            SpobSpaceComponent asteroid2c = (SpobSpaceComponent)asteroid2.SetComponent(new SpobSpaceComponent(asteroid2));
            asteroid2c.Position = new Position3D(-100, 200, 0);
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
            return new SpaceController(this);
        }

        protected override AView CreateView()
        {
            return new SpaceView(this);
        }
    }
}
