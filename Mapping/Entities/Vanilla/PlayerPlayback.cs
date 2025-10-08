using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class PlayerPlayback : CSEntityData
    {
        public override string EntityName => "playbackTutorial";

        private static List<string> tutorials;

        public override void OnRegister()
        {
            base.OnRegister();

            // Read from disk instead.
            tutorials = [];
            PluginAsset directory = Path.Join(MainPlugin.CelesteDirectory, "Content", "Tutorials");
            foreach (string file in directory.GetFiles("*.bin"))
            {
                tutorials.Add(Path.GetFileNameWithoutExtension(file));
            }
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

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"tutorial", ""}
            };
        }

        public override JObject FieldInformation(string fieldName)
        {
            return new JObject()
            {
                {"items", JArray.FromObject(tutorials)},
                {"editable", true}
            };
        }
    }
}