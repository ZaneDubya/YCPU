using Ypsilon.Core.Patterns.MVC;
using Ypsilon.Entities;
using System.Collections.Generic;
using System;

namespace Ypsilon.Modes.Space
{
    class SpaceModel : AModel
    {
        public Serial SelectedSerial
        {
            get;
            set;
        }

        public World World
        {
            get;
            private set;
        }

        public SpaceModel(World world)
        {
            SelectedSerial = Serial.Null;
            World = world;
        }

        public override void Initialize()
        {

        }

        public override void Dispose()
        {

        }

        public override void Update(float totalSeconds, float frameSeconds)
        {
            List<Tuple<AEntity, AEntity>> collisions = new List<Tuple<AEntity, AEntity>>();
            World.GetProjectileCollisions(collisions);
            for (int i = 0; i < collisions.Count; i++)
            {
                collisions[i].Item1.Dispose();
                collisions[i].Item2.Dispose();
            }
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
