using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;

namespace SerializableMethods
{
    public class GenericElementField : ISerializedObject
    {
        public Type[] usedTypes => new Type[] { };

        public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)
        {
            Foldout foldout = new();
            foldout.text = label;
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            value ??= CreateInstance(type);

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                foldout.Add(SerializeMethodHelper.GetElementFieldByType(field.FieldType, field.Name, field.GetValue(value), fieldValue =>
                {
                    field.SetValue(value, fieldValue);
                    onValueChanged?.Invoke(value);
                }));
            }
            for (int i = 0; i < properties.Length; i++)
            {
                var field = properties[i];
                if (!field.CanWrite) continue;
                if (field.GetMethod.GetParameters().Length > 0 || field.SetMethod.GetParameters().Length > 1) continue;
                foldout.Add(SerializeMethodHelper.GetElementFieldByType(field.PropertyType, field.Name, field.GetValue(value), fieldValue =>
                {
                    field.SetValue(value, fieldValue);
                    onValueChanged?.Invoke(value);
                }));
            }

            //return new Label($"{type} is an unsupported type");
            return foldout;
        }

        private object CreateInstance(Type type)
        {
            var constructors = type.GetConstructors();
            ConstructorInfo constructor = null;
            ParameterInfo[] param = null;
            for (int i = 0; i < constructors.Length; i++)
            {
                var constructorParam = constructors[i].GetParameters();
                if (constructor != null && constructorParam.Length >= param?.Length) continue;
                constructor = constructors[i];
                param = constructorParam;
            }
            if (constructor == null || param is { Length: 0 }) return Activator.CreateInstance(type);

            object[] paramValues = new object[param.Length];
            for (int i = 0; i < param.Length; i++)
                paramValues[i] = param[i].ParameterType.IsValueType ? Activator.CreateInstance(param[i].ParameterType) : default;
            return constructor.Invoke(paramValues);
        }
    }
}