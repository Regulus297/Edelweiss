using System;

namespace Edelweiss.Interop.Forms
{
    /// <summary>
    /// Designates that a particular field of a FormObject should not be included in automatic field generation
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FormIgnoreAttribute : Attribute
    {
        
    }
}