using SerializableMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        else if(type.GenericTypeArguments.Length == 1) return  CollectionElement(label, value, type, onValueChanged);
        return new Label($"Collection of type {type} not supported");
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
            onValueChanged?.Invoke(getValuesMethod.Invoke(listField, new object[] { }));
        callbackMethod.Invoke(listField, new object[] { callback, TrickleDown.NoTrickleDown });

        return listField;
    }

    private void AddListValues(Type listFieldType, object listField, object value, Type type)
    {
        var addMethod = listFieldType.GetMethod(nameof(ListField<BaseBoolField, bool>.AddElement));

        //load values
        var enumerator = GetEnumerator(value, type);
        while (enumerator.MoveNext())
            addMethod.Invoke(listField, new object[] { enumerator.Current });
    }

    private IEnumerator GetEnumerator(object value, Type type)
    {
        if(value == null)
        {
            //create enumerator instance
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(int) });
            if (constructor == null) return null;
            var parameters = constructor.GetParameters();
            if(parameters.Length != 1 || parameters[0].ParameterType != typeof(int))
                return null;
            value = constructor.Invoke(new object[] { 1 });
        }
        IEnumerator enumerator = value as IEnumerator;
        if(enumerator == null && value is IEnumerable enumerable) 
            enumerator = enumerable.GetEnumerator();

        return enumerator;
    }

    private VisualElement ArrayElement(string label, object value, Type type, Action<object> onValueChanged)
    {
        //type.GetElementType() => array elements type
        //type.GetArrayRank() => dimentions of array (must be 1 for this to work)
        if (type.GetArrayRank() != 1) return new Label("Must be a 1 dimensional array");

        var elementType = type.GetElementType();
        var baseElement = SerializeMethodHelper.GetElementFieldByType(elementType, label, default, _ => Debug.Log("Changed"));
        var listFieldType = typeof(ListField<,>).MakeGenericType(baseElement.GetType(), elementType);

        //create instance
        var listField = CreateListField(listFieldType,elementType, label, ParseToArray);
        //load values
        AddListValues(listFieldType, listField, value, type);

        return listField;

        void ParseToArray(object value) => onValueChanged?.Invoke(value);
    }

    private VisualElement CollectionElement(string label, object value, Type type, Action<object> onValueChanged)
    {
        var elementType = type.GenericTypeArguments[0];
        var baseElement = SerializeMethodHelper.GetElementFieldByType(elementType, label, default, _ => Debug.Log("Changed"));
        if (!(typeof(ListField<BaseBoolField, bool>).GenericTypeArguments[0].BaseType.
            GetGenericTypeDefinition().MakeGenericType(elementType)).IsAssignableFrom(baseElement.GetType()))
            return new Label($"{baseElement.GetType()} invalid field element type, element must inherit from BaseField");
        var listFieldType = typeof(ListField<,>).MakeGenericType(baseElement.GetType(), elementType);
        var collectionConstructor = type.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
        var parseMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(elementType);

        //create instance
        var listField = CreateListField(listFieldType, elementType, label, ParseToCollection);
        //load values
        AddListValues(listFieldType, listField, value, type);

        return listField;
        void ParseToCollection(object newValue)
        {
            if (collectionConstructor == null)
            {
                Debug.LogWarning("Couldn't find an appropriate constructor to parse value");
                return;
            }
            var enumerable = newValue as IEnumerable;
            onValueChanged?.Invoke(
                collectionConstructor.Invoke(new[] { parseMethod.Invoke(null, new object[] { enumerable }) }));
        }
    }
}