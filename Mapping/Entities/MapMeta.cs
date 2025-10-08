using System.IO;
using Edelweiss.Mapping.SaveLoad;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// An object containing metadata for a map
    /// </summary>
    public class MapMeta : IMapSaveable
    {
        /// <summary>
        /// Options for player inventory (moves the player has access to)
        /// </summary>
        public enum Inventory
        {
            /// <summary>
            /// 1 dash, dream blocks enabled, wearing a backpack, ground refills enabled
            /// </summary>
            Default,
            /// <summary>
            /// 2 dashes, dream blocks enabled, wearing a backpack, ground refills enabled
            /// </summary>
            CH6End,
            /// <summary>
            /// 2 dashes, dream blocks enabled, wearing a backpack, ground refills disabled
            /// </summary>
            Core,
            /// <summary>
            /// 1 dash, dream blocks disabled, wearing a backpack, ground refills enabled
            /// </summary>
            OldSite,
            /// <summary>
            /// 0 dashes, dream blocks disabled, wearing a backpack, ground refills enabled
            /// </summary>
            Prologue,
            /// <summary>
            /// 2 dashes, dream blocks enabled, no backpack, ground refills enabled
            /// </summary>
            TheSummit,
            /// <summary>
            /// 
            /// </summary>
            Farewell
        }

        /// <summary>
        /// The way the player enters the map
        /// </summary>
        public enum IntroType
        {
            /// <summary>
            /// 
            /// </summary>
            WakeUp,
            /// <summary>
            /// 
            /// </summary>
            Respawn,
            /// <summary>
            /// 
            /// </summary>
            WalkInRight,
            /// <summary>
            /// 
            /// </summary>
            WalkInLeft,
            /// <summary>
            /// 
            /// </summary>
            Jump,
            /// <summary>
            /// 
            /// </summary>
            Fall,
            /// <summary>
            /// 
            /// </summary>
            TempleMirrorVoid,
            /// <summary>
            /// 
            /// </summary>
            ThinkForABit,
            /// <summary>
            /// 
            /// </summary>
            None
        }

        /// <summary>
        /// The inventory the player has at the start of the map
        /// </summary>
        public Inventory inventory;

        /// <summary>
        /// The way the player enters the map
        /// </summary>
        public IntroType introType;

        /// <summary>
        /// 
        /// </summary>
        public string path;

        /// <summary>
        /// 
        /// </summary>
        public string poemID;

        /// <summary>
        /// The room the player spawns into on entering the map
        /// </summary>
        public string startLevel;
        /// <summary>
        /// True if collecting a crystal heart ends the map
        /// </summary>
        public bool heartIsEnd = true;
        /// <summary>
        /// Whether Seekers can slow down time while attacking the player
        /// </summary>
        public bool seekerSlowdown;
        /// <summary>
        /// Whether Theo (and other holdables) can be held while in a booster
        /// </summary>
        public bool theoInBubble;
        
        /// <summary>
        /// Whether the player starts the chapter dreaming.
        /// </summary>
        public bool dreaming;

        /// <summary>
        /// Whether the map is an interlude chapter: no B-Side and deaths aren't counted
        /// </summary>
        public bool interlude;

        /// <summary>
        /// If false, the meta for the A-Side of this map will take priority over the map's metadata
        /// </summary>
        public bool overrideASideMeta;

        /// <summary>
        /// The wipe that plays upon death
        /// </summary>
        public string wipe = "";

        /// <summary>
        ///
        /// </summary>
        public string colourGrade = "";

        /// <summary>
        /// The base bloom level
        /// </summary>
        public float bloomBase = 0f;

        /// <summary>
        /// The bloom strength
        /// </summary>
        public float bloomStrength = 1.0f;

        /// <summary>
        /// The alpha used for darkness
        /// </summary>
        public float darknessAlpha = 0.05f;


        /// <inheritdoc/>
        public void AddToLookup(StringLookup lookup)
        {
            lookup.Add("meta", "mode", "Inventory", "Path", "PoemID", "StartLevel", "HeartIsEnd", "SeekerSlowdown", "TheoInBubble", "Dreaming", "Interlude", "Wipe", "ColorGrade", "BloomBase", "BloomStrength", "DarknessAlpha", "OverrideASideMeta", "IntroType");
            lookup.Add(inventory.ToString(), wipe.ToString(), colourGrade.ToString(), introType.ToString());
            if (!string.IsNullOrEmpty(startLevel))
                lookup.Add(startLevel);
            if (!string.IsNullOrEmpty(poemID))
                lookup.Add(poemID);
            if (!string.IsNullOrEmpty(path))
                lookup.Add(path);
        }

        /// <inheritdoc/>
        public void Encode(BinaryWriter writer)
        {
            // meta child
            writer.WriteLookupString("meta");
            writer.Write((byte)9);
            writer.WriteAttribute("Dreaming", dreaming);
            writer.WriteAttribute("Interlude", interlude);
            writer.WriteAttribute("OverrideASideMeta", overrideASideMeta);
            writer.WriteAttribute("Wipe", wipe);
            writer.WriteAttribute("ColorGrade", colourGrade);
            writer.WriteAttribute("BloomBase", bloomBase);
            writer.WriteAttribute("BloomStrength", bloomStrength);
            writer.WriteAttribute("DarknessAlpha", darknessAlpha);
            writer.WriteAttribute("IntroType", introType.ToString());

            writer.Write((short)1);

            writer.WriteLookupString("mode");
            byte attrCount = 4;
            if (!string.IsNullOrEmpty(startLevel))
                attrCount++;
            if (!string.IsNullOrEmpty(path))
                attrCount++;
            if (!string.IsNullOrEmpty(poemID))
                attrCount++;

            writer.Write(attrCount);
            writer.WriteAttribute("Inventory", inventory.ToString());
            writer.WriteAttribute("HeartIsEnd", heartIsEnd);
            writer.WriteAttribute("SeekerSlowdown", seekerSlowdown);
            writer.WriteAttribute("TheoInBubble", theoInBubble);
            if (!string.IsNullOrEmpty(startLevel))
                writer.WriteAttribute("StartLevel", startLevel);
            if (!string.IsNullOrEmpty(path))
                writer.WriteAttribute("Path", path);
            if (!string.IsNullOrEmpty(poemID))
                writer.WriteAttribute("TheoInBubble", theoInBubble);

            writer.Write((short)0);
        }

        /// <summary>
        /// Converts this map meta object to a JSON object
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject()
            {
                {nameof(inventory), inventory.ToString()},
                {nameof(heartIsEnd), heartIsEnd},
                {nameof(seekerSlowdown), seekerSlowdown},
                {nameof(theoInBubble), theoInBubble},
                {nameof(dreaming), dreaming},
                {nameof(interlude), interlude},
                {nameof(overrideASideMeta), overrideASideMeta},
                {nameof(wipe), wipe},
                {nameof(colourGrade), colourGrade},
                {nameof(bloomBase), bloomBase},
                {nameof(bloomStrength), bloomStrength},
                {nameof(darknessAlpha), darknessAlpha},
                {nameof(introType), introType.ToString()}
            };
        }
    }
}