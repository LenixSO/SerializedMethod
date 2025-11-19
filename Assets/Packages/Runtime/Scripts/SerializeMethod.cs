using System;

namespace SerializableMethods
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class SerializeMethod : Attribute
    {
    }
}