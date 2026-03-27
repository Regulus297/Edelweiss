using System;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Interop.Forms
{
    /// <summary>
    /// Class representing a field of a form object
    /// </summary>
    public class FormField(string name, string type, JObject fieldInfo = null)
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public virtual string Name { get; set; } = name;

        /// <summary>
        /// 
        /// </summary>
        public virtual string Type { get; set; } = type;

        /// <summary>
        /// Gets any addition field info for the field.
        /// </summary>
        public virtual JObject FieldInfo { get; set; } = fieldInfo;

        /// <summary>
        /// Whether this field is dynamic. If true, the value set will be at Object.DynamicFields.@Name instead of Object.Name
        /// </summary>
        public bool Dynamic { get; set; } = false;
    }
}