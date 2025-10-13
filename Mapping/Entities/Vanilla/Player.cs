using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Player : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "player";

        public override List<string> PlacementNames()
        {
            return ["player"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/player/sitDown00";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("isDefaultSpawn", false);
        }

        // For some reason the save files for vanilla store width for this entity fucking up selection
        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [GetDefaultRectangle(room, entity, -1)];
        }
    }
}