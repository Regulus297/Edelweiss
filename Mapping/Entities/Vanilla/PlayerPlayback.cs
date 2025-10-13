using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class PlayerPlayback : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "playbackTutorial";

        private static List<string> tutorials;

        public override void OnRegister()
        {
            // Read from disk instead.
            tutorials = [];
            PluginAsset directory = Path.Join(MainPlugin.CelesteDirectory, "Content", "Tutorials");
            foreach (string file in directory.GetFiles("*.bin"))
            {
                tutorials.Add(Path.GetFileNameWithoutExtension(file));
            }
            base.OnRegister();
        }

        public override List<string> PlacementNames()
        {
            return ["playback"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/player/sitDown00";
        public override string Color(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.8f, 0.2f, 0.2f, 0.75f);
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 2];

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("tutorial", "", true, [.. tutorials]);
        }
    }
}