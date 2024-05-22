using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    public class SerializedFloat: ISerializedObject
    {
        public Type[] usedTypes => new [] { typeof(float), typeof(decimal), typeof(double) };
        
        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            FloatField field = new FloatField(label);
            field.value = (float)Convert.ToDecimal(value);
            field.RegisterCallback<ChangeEvent<float>>(evt => onValueChanged?.Invoke(evt.newValue));
            return field;
            //returnObject = EditorGUILayout.FloatField(label, (float)returnObject, width);
        }
    }
}