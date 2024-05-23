using System;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedBool: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(bool) };
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Toggle field = new Toggle(label);
            field.value = (bool)value;
            field.RegisterCallback<ChangeEvent<bool>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
            //returnObject = EditorGUILayout.Toggle(label, (bool)returnObject, width);
        }
    }
}