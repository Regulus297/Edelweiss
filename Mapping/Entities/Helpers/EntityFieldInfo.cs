using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Helpers
{
    /// <summary>
    /// Data structure containing field info for entities
    /// </summary>
    public class EntityFieldInfo
    {
        private JObject info = [];

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> fields = [];

        private string cyclableField = null;
        private string rotateField = null;
        private string hFlipField = null;
        private string vFlipField = null;
        private enum CycleType
        {
            None,
            Options,
            Boolean
        }
        private CycleType cycleType = CycleType.None;
        private CycleType rotateType = CycleType.None;
        private CycleType hFlipType = CycleType.None;
        private CycleType vFlipType = CycleType.None;

        private Dictionary<string, object[]> optionLookup = [];

        /// <summary>
        /// Adds a field to the info
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public EntityFieldInfo AddField(string name, object value) {
            fields.Add(name, value);
            return this;
        }

        /// <summary>
        /// Adds a field to the data which contains an integer value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public EntityFieldInfo AddIntField(string name, int value = 0, int? min = null, int? max = null)
        {
            fields[name] = value;
            JObject fieldInfo = [];
            bool add = false;
            if (min != null)
            {
                fieldInfo["min"] = min;
                add = true;
            }
            if (max != null)
            {
                fieldInfo["max"] = max;
                add = true;
            }
            if (add)
            {
                info[name] = fieldInfo;
            }
            return this;
        }

        /// <summary>
        /// Adds a field to the data which contains a float value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public EntityFieldInfo AddFloatField(string name, float value = 0, float? min = null, float? max = null)
        {
            fields[name] = value;
            JObject fieldInfo = [];
            bool add = false;
            if (min != null)
            {
                fieldInfo["min"] = min;
                add = true;
            }
            if (max != null)
            {
                fieldInfo["max"] = max;
                add = true;
            }
            if (add)
            {
                info[name] = fieldInfo;
            }
            return this;
        }

        /// <summary>
        /// Adds a field to the data which contains a string with options that is not editable
        /// </summary>
        public EntityFieldInfo AddOptionsField(string name, string value, params string[] options) => AddOptionsField(name, value, false, null, options);
        /// <summary>
        /// Adds a field to the data which contains a string with options
        /// </summary>
        public EntityFieldInfo AddOptionsField(string name, object value, bool editable, params object[] options) => AddOptionsField(name, value, editable, null, options);
        /// <summary>
        /// Adds a field to the data which contains an object of the given type with options that is not editable
        /// </summary>
        public EntityFieldInfo AddOptionsField(string name, object value, string type, params object[] options) => AddOptionsField(name, value, false, type, options);
        /// <summary>
        /// Adds a field to the data which contains an object of the given type with options.
        /// </summary>
        public EntityFieldInfo AddOptionsField(string name, object value, bool editable, string type, params object[] options)
        {
            fields[name] = value;
            JObject fieldInfo = [];
            if (type != null)
                fieldInfo["fieldType"] = type;
            if (editable)
                fieldInfo["editable"] = editable;
            fieldInfo["items"] = JArray.FromObject(options);
            optionLookup[name] = options;
            info[name] = fieldInfo;
            return this;
        }

        /// <summary>
        /// Adds a field to the data which contains an object of the given type with options that are key-value pairs
        /// </summary>
        public EntityFieldInfo AddOptionsField(string name, object value, params (string, string)[] options)
        {
            fields[name] = value;
            JObject fieldInfo = [];
            fieldInfo["items"] = JObject.FromObject(options.Select(t => new KeyValuePair<object, object>(t.Item1, t.Item2)).ToDictionary());
            optionLookup[name] = options.Select(t => t.Item2).ToArray();
            info[name] = fieldInfo;
            return this;
        }

        /// <summary>
        /// Makes the entity resizable. By default, makes it 8*8 and resizable on both axes
        /// </summary>
        /// <param name="width">The starting width of the entity. Null if it cannot be resized horizontally</param>
        /// <param name="height">The starting height of the entity. Null if it cannot be resized vertically</param>
        /// <returns></returns>
        public EntityFieldInfo AddResizability(int? width = 8, int? height = 8)
        {
            if (width != null)
                AddField("width", width);
            if (height != null)
                AddField("height", height);
            return this;
        }

        /// <summary>
        /// Adds a list of nodes to the entity's placement
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public EntityFieldInfo AddNodes(params Point[] nodes)
        {
            fields["nodes"] = nodes;
            return this;
        }

        /// <summary>
        /// This field will be changed when the entity's state is cycled
        /// </summary>
        public EntityFieldInfo SetCyclableField(string name) => SetOperationField(name, ref cyclableField, ref cycleType);

        /// <summary>
        /// This field will be changed when the entity is rotated
        /// </summary>
        public EntityFieldInfo SetRotateField(string name) => SetOperationField(name, ref rotateField, ref rotateType);

        /// <summary>
        /// This field will be changed when the entity is flipped horizontally
        /// </summary>
        public EntityFieldInfo SetHorizontalFlipField(string name) => SetOperationField(name, ref hFlipField, ref hFlipType);
        /// <summary>
        /// This field will be changed when the entity is flipped vertically
        /// </summary>
        public EntityFieldInfo SetVerticalFlipField(string name) => SetOperationField(name, ref vFlipField, ref vFlipType);

        private EntityFieldInfo SetOperationField(string name, ref string field, ref CycleType cycleType)
        {
            if (name == null)
            {
                field = null;
                cycleType = CycleType.None;
            }
            if (!fields.TryGetValue(name, out object value))
                return this;
            if (value is bool)
            {
                field = name;
                cycleType = CycleType.Boolean;
            }
            else if (fields.ContainsKey(name))
            {
                field = name;
                cycleType = CycleType.Options;
            }
            else
            {
                field = null;
                cycleType = CycleType.None;
            }
            return this;
        }

        /// <summary>
        /// Cycles the entity based on the cyclable field set
        /// </summary>
        public bool Cycle(Entity entity, int amount) => OperateField(entity, amount, cyclableField, cycleType);

        /// <summary>
        /// Rotates the entity based on the rotate field set
        /// </summary>
        public bool Rotate(Entity entity, int direction) => OperateField(entity, direction, rotateField, rotateType);

        /// <summary>
        /// Flips the entity based on the flip fields set
        /// </summary>
        public bool Flip(Entity entity, bool horizontal, bool vertical)
        {
            bool h = false, v = false;
            if (horizontal)
                h = OperateField(entity, 1, hFlipField, hFlipType);
            if (vertical)
                v = OperateField(entity, 1, vFlipField, vFlipType);
            return h || v;
        }

        
        private bool OperateField(Entity entity, int amount, string field, CycleType type)
        {
            if (type == CycleType.Boolean && amount % 2 != 0)
            {
                entity[field] = !entity.Get<bool>(field);
                return true;
            }
            else if (type == CycleType.Options)
            {
                object[] options = optionLookup[field];
                entity[field] = options.Cycle(entity.Get<object>(field), amount);
                return true;
            }
            return false;
            
        }

        /// <summary>
        /// Gets the field info for a specific field
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JObject GetFieldInfo(string name) => (JObject)info.Value<JObject>(name)?.DeepClone();
    }
}