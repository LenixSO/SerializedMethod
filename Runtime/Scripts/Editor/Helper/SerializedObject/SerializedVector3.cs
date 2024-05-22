using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedVector3: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(Vector3) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Vector3 vector = (value as Vector3?).HasValue ? (Vector3)(value as Vector3?) : Vector3.zero;
            Vector3Field field = new Vector3Field();
            field.value = vector;
            field.RegisterCallback<ChangeEvent<Vector3>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}