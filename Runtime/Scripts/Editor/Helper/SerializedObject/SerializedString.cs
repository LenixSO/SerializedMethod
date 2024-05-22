using System;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedString : ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(string) };

        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            value = value == null ? string.Empty : value;
            TextField field = new TextField(label);
            field.value = value.ToString();
            //methodParameters[key] = field.value;
            field.RegisterCallback<ChangeEvent<string>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}