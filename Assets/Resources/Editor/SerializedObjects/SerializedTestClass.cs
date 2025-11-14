using System;
using SerializableMethods;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class SerializedTestClass : ISerializedObject
{
    public Type[] usedTypes => new [] { typeof(TestGenericClass) };
    public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
    {
        TestGenericClass data = value == null ? new TestGenericClass() : value as TestGenericClass;
        VisualElement field = new VisualElement();
        IntegerField n = new IntegerField(nameof(TestGenericClass.number));
        n.value = data.number;
        n.RegisterCallback<ChangeEvent<int>>((evt) =>
        {
            data.number = evt.newValue;
            onValueChanged?.Invoke(data);
        });

        TextField t = new TextField(nameof(TestGenericClass.text));
        t.value = data.text;
        t.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            data.text = evt.newValue;
            onValueChanged?.Invoke(data);
        });

        field.Add(n);
        field.Add(t);
        return field;
    }
}