using System;

namespace SerializableMethods
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SerializeMethod : Attribute
    {
    }
}