using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SerializableMethods
{
    public class SerializedUnityObject: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(Object) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            ObjectField field = new ObjectField(label);
            field.objectType = type;
            field.value = value == null ? default : (Object)value;
            field.RegisterCallback<ChangeEvent<Object>>((evt) => onValueChanged?.Invoke(evt.newValue));
            return field;
        }
    }
}