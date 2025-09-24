using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CoreToggle : CSEntityData
    {
        public override string EntityName => "coreModeToggle";

        public override List<string> PlacementNames()
        {
            return ["both", "fire", "ice"];
        }

        public override string Texture(RoomData room, Entity entity)
        {
            if ((bool)entity["onlyIce"])
                return "objects/coreFlipSwitch/switch13";

            if ((bool)entity["onlyFire"])
                return "objects/coreFlipSwitch/switch15";

            return "objects/coreFlipSwitch/switch01";
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"onlyIce", placement == "ice"},
                {"onlyFire", placement == "fire"},
                {"persistent", false}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            if ((bool)entity["onlyIce"])
            {
                entity["onlyIce"] = false;
                entity["onlyFire"] = true;
            }
            else if ((bool)entity["onlyFire"])
            {
                entity["onlyIce"] = false;
                entity["onlyFire"] = false;
            }
            else
            {
                entity["onlyIce"] = true;
                entity["onlyFire"] = false;
            }
            return true;
        }
    }
}