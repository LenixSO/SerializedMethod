using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedUnityObject: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(UnityEngine.Object) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            ObjectField field = new ObjectField(label);
            field.objectType = type;
            field.value = (UnityEngine.Object)value;
            field.RegisterCallback<ChangeEvent<Object>>((evt) => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}