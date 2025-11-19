using System;
using SerializableMethods;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SerializedDouble : ISerializedObject
{
   public SerializedDouble(){ }
   public Type[] usedTypes => new [] { typeof(double), typeof(double?) };
   public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
    {
        FloatField field = new FloatField(label);
        try { field.value = (float)Convert.ToDouble(value); }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not convert value:\n{e}");
            field.value = default;
        }
        field.RegisterCallback<ChangeEvent<float>>(evt => onValueChanged?.Invoke((double)evt.newValue));
        return field;
    }
}