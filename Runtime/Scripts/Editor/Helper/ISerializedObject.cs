using System;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public interface ISerializedObject
    {
        public Type[] usedTypes { get; }
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged);
    }
}