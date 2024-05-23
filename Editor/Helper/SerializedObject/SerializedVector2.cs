using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedVector2: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(Vector2) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Vector2 vector = (value as Vector2?).HasValue ? (Vector2)(value as Vector2?) : Vector2.zero;
            Vector2Field field = new Vector2Field();
            field.value = vector;
            field.RegisterCallback<ChangeEvent<Vector2>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}