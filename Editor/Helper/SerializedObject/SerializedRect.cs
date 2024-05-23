using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedRect: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(Rect) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Rect rect = (value as Rect?).HasValue ? (Rect)(value as Rect?) : Rect.zero;
            RectField field = new RectField();
            field.value = rect;
            field.RegisterCallback<ChangeEvent<Rect>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}