using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedBounds: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(Bounds) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Bounds bounds = (value as Bounds?).HasValue ? (Bounds)(value as Bounds?) : default;
            BoundsField field = new BoundsField();
            field.value = bounds;
            field.RegisterCallback<ChangeEvent<Bounds>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}