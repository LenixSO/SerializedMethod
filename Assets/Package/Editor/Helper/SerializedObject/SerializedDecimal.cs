using System;
using UnityEngine;
using SerializableMethods;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//A Custom Serialized Object must be in the Resources Folder to be detected
public class SerializedDecimal : ISerializedObject
{
   public SerializedDecimal(){ }
   public Type[] usedTypes => new [] { typeof(decimal), typeof(decimal?) };
   public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
   {
        FloatField field = new FloatField(label);
        try { field.value = (float)Convert.ToDecimal(value); }
        catch (Exception e) 
        {
            Debug.LogWarning($"Could not convert value:\n{e}");
            field.value = default; 
        }
        field.RegisterCallback<ChangeEvent<float>>(evt => onValueChanged?.Invoke((decimal)evt.newValue));
        return field;
    }
}