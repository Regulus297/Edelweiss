using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Interop.Forms
{
    /// <summary>
    /// Base class for objects that can be used to dynamically generate forms
    /// </summary>
    public abstract class FormObject : BindableList<FormField>
    {
        /// <summary>
        /// 
        /// </summary>
        public FormObject()
        {
            InitializeFields();
        }

        /// <summary>
        /// For Form Objects, this should never be set to. Use <see cref="CopyFrom"/> instead.
        /// </summary>
        public sealed override List<FormField> Value { 
            get => base.Value;
            set => base.Value = value; 
        }


        /// <summary>
        /// Copy the data from another form object. Ideally should be of the same type as this instance.
        /// </summary>
        public abstract void CopyFrom(FormObject other);

        /// <summary/>
        public abstract void InitializeFields();

        /// <summary>
        /// The root localization key for this form
        /// </summary>
        public virtual string LocalizationRoot => "Edelweiss.Forms";

        /// <summary>
        /// The values of fields that have been added dynamically
        /// </summary>
        public BindableDictionary<string, object> DynamicFields = [];

        /// <summary>
        /// Adds a field with the given name and auto-determined type
        /// </summary>
        protected void AddField(string name) => Add(new FormField(name, null));

        /// <summary>
        /// Adds a field with the given name and given type
        /// </summary>
        protected void AddField(string name, string type) => Add(new FormField(name, type));

        /// <summary>
        /// Adds a field with the given name, given type and given extra field information.
        /// Do not use this for enum types, use <see cref="AddField(string)"/> instead. The options will be auto-generated.
        /// </summary>
        protected void AddField(string name, string type, JObject info) => Add(new FormField(name, type, info));

        /// <summary>
        /// Creates a field that is a selection from multiple static options
        /// </summary>
        protected FormField CreateOptionsField(string name, string[] options)
        {
            return new FormField(name, "list", new JObject()
            {
                {"options", JArray.FromObject(options)}    
            });
        }

        /// <summary>
        /// Adds a field that is a selection from several options with different display values and internal values
        /// </summary>
        /// <param name="name" />
        /// <param name="options">Dictionary of the display name of the option to the internal value of the option</param>
        protected FormField CreateOptionsField(string name, Dictionary<string, string> options)
        {
            return new FormField(name, "list", new JObject()
            {
                {"options", JObject.FromObject(options)}    
            });
        }

        /// <summary>
        /// Adds a field that is a selection from several options that can be dynamically updated
        /// </summary>
        /// <param name="name" />
        /// <param name="options">The SyncableProperty for the options</param>
        protected FormField CreateOptionsField(string name, string options)
        {
            return new FormField(name, "list", new JObject()
            {
                {"bind", new JObject() {
                    {"options", options}
                }}    
            });
        }

        /// <summary>
        /// Makes the field with the given name a dynamic field
        /// </summary>
        protected void AddDynamicField(FormField field, object defaultValue = null)
        {
            field.Dynamic = true;
            DynamicFields[field.Name] = defaultValue;
            Add(field);
        }

        /// <summary>
        /// Gets the field with the given name
        /// </summary>
        protected FormField GetField(string name) => Value.Find(f => f.Name == name);

        /// <summary>
        /// Removes the field with the given name
        /// </summary>
        protected void RemoveField(string name)
        {
            Remove(GetField(name));
            DynamicFields.Remove(name);
        }
    }
}