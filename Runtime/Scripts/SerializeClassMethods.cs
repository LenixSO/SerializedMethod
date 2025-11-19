using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SerializableMethods
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SerializeClassMethods : Attribute
    {
        public bool includeInherited {  get; set; }
        public MethodType MethodType { get; set; }
        public SerializeClassMethods(MethodType methodType = MethodType.Public | MethodType.NotPublic, bool includeInherited = false)
        {
            MethodType = methodType;
            this.includeInherited = includeInherited;
        }
        public BindingFlags GetBindingFlags() => BindingFlags.DeclaredOnly | BindingFlags.Instance | (BindingFlags)((int)MethodType);
    }
    [Flags]
    public enum MethodType
    {
        Public = 16,
        NotPublic = 32,
    }
}