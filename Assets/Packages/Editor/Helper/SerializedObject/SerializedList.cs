using SerializableMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//A Custom Serialized Object must be in the Resources Folder to be detected
public class SerializedList : ISerializedObject
{
    public SerializedList() { }
    public Type[] usedTypes => new[] { typeof(IEnumerable), typeof(IEnumerator) };
    public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
    {
        if (type.IsArray) return ArrayElement(label, value, type, onValueChanged);

        return new Label("list implementation in progress");
    }

    private VisualElement CreateListField(Type listFieldType, Type valueType, string label, Action<object> onValueChanged)
    {
        //create instance
        var listField = listFieldType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { label }) as VisualElement;

        var methods = listFieldType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        MethodInfo baseMethod = null;
        foreach (var method in methods)
        {
            if (method.Name != nameof(VisualElement.RegisterCallback) || method.GetParameters().Length != 2) continue;
            baseMethod = method;
            break;
        }

        //add value change callback
        var eventType = typeof(CollectionChangeEvent<>).MakeGenericType(new Type[] { valueType });
        var getValuesMethod = listFieldType.GetMethod(nameof(ListField<BaseBoolField, bool>.GetValues));
        var callbackMethod = baseMethod.MakeGenericMethod(new Type[] { eventType });
        //EventCallback<object> callback = _ => Debug.Log("value changed");
        EventCallback<object> callback = _ =>
        {
            var raw = getValuesMethod.Invoke(listField, new object[] { });
            onValueChanged?.Invoke(raw);
        };
        callbackMethod.Invoke(listField, new object[] { callback, TrickleDown.NoTrickleDown });

        return listField;
    }

    private IEnumerator GetEnumerator(object value, Type type)
    {
        if(value == null)
        {
            //create enumerator instance
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(int) });
            if (constructor == null) return null;

            //lists
            if (constructor.IsGenericMethod)
            {
                Debug.Log("generic constructor");
                return null;
            }
            //arrays
            var parameters = constructor.GetParameters();
            if(parameters.Length != 1 || parameters[0].ParameterType != typeof(int))
                return null;
            value = constructor.Invoke(new object[] { 1 });
        }
        IEnumerator enumerator = value as IEnumerator;
        if(enumerator == null && value is IEnumerable) 
            enumerator = (value as IEnumerable).GetEnumerator();

        return enumerator;
    }

    private VisualElement ArrayElement(string label, object value, Type type, Action<object> onValueChanged)
    {
        //type.GetElementType() => array elements type
        //type.GetArrayRank() => dimentions of array (must be 1 for this to work)
        if (type.GetArrayRank() != 1) return new Label("Must be a 1 dimensional array");

        var elementType = type.GetElementType();
        var baseElement = SerializeMethodHelper.GetElementFieldByType(elementType, label, default, _ => Debug.Log("Changed"));
        Debug.Log($"{baseElement.GetType().Name} | {type.Name}");
        var listFieldType = typeof(ListField<,>).MakeGenericType(baseElement.GetType(), elementType);
        var addMethod = listFieldType.GetMethod(nameof(ListField<BaseBoolField, bool>.AddElement));

        //create instance
        var listField = CreateListField(listFieldType,elementType, label, onValueChanged);

        //load values
        var enumerator = GetEnumerator(value, type);
        enumerator.Reset();
        while (enumerator.MoveNext())
            addMethod.Invoke(listField, new object[] { enumerator.Current });

        return listField;
    }
}