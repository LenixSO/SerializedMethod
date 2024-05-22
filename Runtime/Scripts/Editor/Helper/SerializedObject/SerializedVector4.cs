using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedVector4: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(Vector4) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Vector4 vector = (value as Vector4?).HasValue ? (Vector4)(value as Vector4?) : Vector4.zero;
            Vector4Field field = new Vector4Field();
            field.value = vector;
            field.RegisterCallback<ChangeEvent<Vector4>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}