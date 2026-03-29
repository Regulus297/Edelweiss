using System;
using System.Collections.Generic;
using System.IO;
using Edelweiss.Interop;
using Edelweiss.Interop.Forms;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// An object containing metadata for a map
    /// </summary>
    public class MapMeta() : FormObject()
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

        private static readonly Dictionary<string, string> WipeOptions = new Dictionary<string, string>()
        {
            {"", ""},
            {"Angled", "Celeste.AngledWipe"},
            {"Curtain", "Celeste.CurtainWipe"},
            {"Dream", "Celeste.DreamWipe"},
            {"Drop", "Celeste.DropWipe"},
            {"Fade", "Celeste.FadeWipe"},
            {"Fall", "Celeste.FallWipe"},
            {"Heart", "Celeste.HeartWipe"},
            {"KeyDoor", "Celeste.KeyDoorWipe"},
            {"Mountain", "Celeste.MountainWipe"},
            {"Spotlight", "Celeste.SpotlightWipe"},
            {"Starfield", "Celeste.StarfieldWipe"},
            {"Wind", "Celeste.WindWipe"}
        };

        private static readonly string[] ColourGradeOptions = ["", "oldsite", "reflection", "cold", "credits", "feelingdown", "golden", "hot", "none", "panicattack", "templevoid"];

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
        [FormIgnore]
        public BindableVariable<string> path = "";

        /// <summary>
        /// 
        /// </summary>
        [FormIgnore]
        public BindableVariable<string> poemID = "";

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
        [FormIgnore]
        public BindableVariable<string> wipe = "";

        /// <summary>
        ///
        /// </summary>
        [FormIgnore]
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

        /// <inheritdoc/>
        public override void CopyFrom(FormObject other)
        {
            if(other is MapMeta m)
            {
                inventory.Value = m.inventory.Value;
                introType.Value = m.introType.Value;
                path.Value = m.path.Value;
                poemID.Value = m.poemID.Value;
                startLevel.Value = m.startLevel.Value;
                heartIsEnd.Value = m.heartIsEnd.Value;
                seekerSlowdown.Value = m.seekerSlowdown.Value;
                theoInBubble.Value = m.theoInBubble.Value;
                dreaming.Value = m.dreaming.Value;
                interlude.Value = m.interlude.Value;
                overrideASideMeta.Value = m.overrideASideMeta.Value;
                wipe.Value = m.wipe.Value;
                colourGrade.Value = m.colourGrade.Value;
                bloomBase.Value = m.bloomBase.Value;
                bloomStrength.Value = m.bloomStrength.Value;
                darknessAlpha.Value = m.darknessAlpha.Value;
                
                Value = other.Value;
                DynamicFields.Value = other.DynamicFields.Value;
            }
        }

        /// <inheritdoc/>
        public override string LocalizationRoot => "Edelweiss.Mapping.MapMeta";

        /// <inheritdoc/>
        public override void InitializeFields()
        {
            base.InitializeFields();
            Add(CreateOptionsField(nameof(wipe), WipeOptions, true));
            Add(CreateOptionsField(nameof(colourGrade), ColourGradeOptions, true));
        }
    }
}