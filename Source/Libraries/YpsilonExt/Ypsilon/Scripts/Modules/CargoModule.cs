using Ypsilon.Entities;

namespace Ypsilon.Scripts.Modules
{
    class CargoModule : AModule
    {
        public override int HoldSpace { get { return 10; } }

        public override string DefaultName { get { return "Small cargo hold"; } }
    }
}
