using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class PicoConsole : CSEntityData
    {
        public override string EntityName => "picoconsole";

        public override List<string> PlacementNames()
        {
            return ["pico_console"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override int Depth(RoomData room, Entity entity) => 1000;
        public override string Texture(RoomData room, Entity entity) => "objects/pico8Console";
    }
}