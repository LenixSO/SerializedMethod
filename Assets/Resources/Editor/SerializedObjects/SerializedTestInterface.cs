using UnityEngine;
using SerializableMethods;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

public class SerializedTestInterface : ISerializedObject
{
    public Type[] usedTypes => new[] { typeof(ITestGenericInterface) };

    public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
    {
        ObjectField field = new ObjectField(label);
        field.objectType = typeof(ITestGenericInterface);
        field.value = (Object)value;
        field.RegisterCallback<ChangeEvent<Object>>((evt) => onValueChanged?.Invoke(evt.newValue));
        return field;
    }
}
