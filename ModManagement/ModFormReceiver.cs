using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Edelweiss.Mapping.PacketReceivers
{
    internal class ModFormReceiver : PacketReceiver
    {
        public override long HandledCode => Netcode.FORM_SUBMITTED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            string id = data.Value<string>("id");
            JObject extraData = data.Value<JObject>("extraData");
            if (id == "ModCreationForm")
                CreateMod(extraData);
                
        }

        private void CreateMod(JObject data)
        {
            string modName = data.Value<string>("name").Replace(' ', '_');
            string mapperName = data.Value<string>("mapperName").Replace(' ', '_');
            string modDirectory = Path.Join(MainPlugin.CelesteDirectory, "Mods", modName);
            Directory.CreateDirectory(modDirectory);

            // YAML
            var yamlData = new
            {
                Name = modName,
                Version = "0.0.1"
            };
            var yamlList = new[] { yamlData };
            ISerializer serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            File.WriteAllText(Path.Join(modDirectory, "everest.yaml"), serializer.Serialize(yamlList));

            // Edelweiss Meta
            JObject meta = new JObject()
            {
                {"mapper", mapperName}
            };
            File.WriteAllText(Path.Join(modDirectory, "edelweiss.meta.json"), meta.ToString());
        }
    }
}