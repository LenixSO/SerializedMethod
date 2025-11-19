using System;
using SerializableMethods;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//A Custom Serialized Object must be in the Resources Folder to be detected
public class SerializedEnum : ISerializedObject
{
   public SerializedEnum(){ }
   public Type[] usedTypes => new [] { typeof(Enum)/*change this to your desired type*/ };
   public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
   {
       EnumField field = new EnumField(label);
       field.Init((Enum)Enum.GetValues(type).GetValue(0));
       if (value != null) field.value = (Enum)value;
       field.RegisterCallback<ChangeEvent<Enum>>((evt) => onValueChanged?.Invoke(evt.newValue));
       return field;
   }
}