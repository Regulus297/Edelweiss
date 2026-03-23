using System;
using System.IO;
using Edelweiss.Interop;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// An object containing metadata for a map
    /// </summary>
    public class MapMeta
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
        public BindableVariable<Inventory> inventory = Inventory.Default;

        /// <summary>
        /// The way the player enters the map
        /// </summary>
        public BindableVariable<IntroType> introType = IntroType.WakeUp;

        /// <summary>
        /// 
        /// </summary>
        public BindableVariable<string> path;

        /// <summary>
        /// 
        /// </summary>
        public BindableVariable<string> poemID;

        /// <summary>
        /// The room the player spawns into on entering the map
        /// </summary>
        public BindableVariable<string> startLevel = "";
        /// <summary>
        /// True if collecting a crystal heart ends the map
        /// </summary>
        public BindableVariable<bool> heartIsEnd = true;
        /// <summary>
        /// Whether Seekers can slow down time while attacking the player
        /// </summary>
        public BindableVariable<bool> seekerSlowdown = false;
        /// <summary>
        /// Whether Theo (and other holdables) can be held while in a booster
        /// </summary>
        public BindableVariable<bool> theoInBubble = false;
        
        /// <summary>
        /// Whether the player starts the chapter dreaming.
        /// </summary>
        public BindableVariable<bool> dreaming = false;

        /// <summary>
        /// Whether the map is an interlude chapter: no B-Side and deaths aren't counted
        /// </summary>
        public BindableVariable<bool> interlude = false;

        /// <summary>
        /// If false, the meta for the A-Side of this map will take priority over the map's metadata
        /// </summary>
        public BindableVariable<bool> overrideASideMeta = false;

        /// <summary>
        /// The wipe that plays upon death
        /// </summary>
        public BindableVariable<string> wipe = "";

        /// <summary>
        ///
        /// </summary>
        public BindableVariable<string> colourGrade = "";

        /// <summary>
        /// The base bloom level
        /// </summary>
        public BindableVariable<float> bloomBase = 0f;

        /// <summary>
        /// The bloom strength
        /// </summary>
        public BindableVariable<float> bloomStrength = 1.0f;

        /// <summary>
        /// The alpha used for darkness
        /// </summary>
        public BindableVariable<float> darknessAlpha = 0.05f;
    }
}