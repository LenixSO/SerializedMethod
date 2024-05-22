using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedColor: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(Color) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Color color = (value as Color?).HasValue ? (Color)(value as Color?) : default;
            ColorField field = new ColorField();
            field.value = color;
            field.RegisterCallback<ChangeEvent<Color>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}