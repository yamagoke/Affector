using System;

namespace Affector
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false, Inherited =true)]
    public class AffectableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        public int Index { get; private set; }
        public KeyAttribute(int index)
        {
            Index = index;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValueAttribute : Attribute
    {
        public string[] Names { get; private set; }
        public ValueAttribute() : this(new string[0]) { }
        public ValueAttribute(params string[] names)
        {
            Names = names;
        }
    }
}
