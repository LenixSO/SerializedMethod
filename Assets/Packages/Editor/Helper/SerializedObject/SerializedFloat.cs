using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedFloat : ISerializedObject
    {
        public Type[] usedTypes => new[] { typeof(float), typeof(float?) };

        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            FloatField field = new FloatField(label);
            try { field.value = (float)Convert.ToDecimal(value); }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not convert value:\n{e}");
                field.value = default;
            }
            field.RegisterCallback<ChangeEvent<float>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}