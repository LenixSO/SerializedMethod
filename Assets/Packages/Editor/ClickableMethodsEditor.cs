using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    [CustomEditor(typeof(MonoBehaviour),true)]
    public class ClickableMethodsEditor : Editor
    {
        private Dictionary<string, object> methodParameters = new();

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            ObjectField classField = new ObjectField("Script");
            classField.value = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
            classField.SetEnabled(false);
            root.Add(classField);

            //Load fields
            FieldInfo[] fields = GetAllFields(target.GetType());

            //Debug.Log($"{target.GetType().Name} - {fields.Length}");
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                {
                    PropertyField property = new PropertyField(new SerializedObject(target).FindProperty(field.Name));
                    root.Add(property);
                }
            }
            
            //Place methods
            MethodInfo[] methods = SerializeMethodHelper.GetMethods(target.GetType());
            foreach (MethodInfo method in methods)
            {
                if (method.GetCustomAttribute<SerializeMethod>() != null)
                {
                    MonoBehaviour mono = (MonoBehaviour)target;
                    SerializeMethodHelper.ShowMethod(mono.gameObject, method, root);
                }
            }

            return root;
        }

        private static FieldInfo[] GetAllFields(Type type)
        {
            List<FieldInfo> fieldList = new();
            Stack<Type> typeStack = SerializeMethodHelper.GetInheritedTypes(type, typeof(MonoBehaviour));
            int count = typeStack.Count;
            for (int i = 0; i < count; i++)
            {
                type = typeStack.Pop();
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
                for (int u = 0; u < fields.Length; u++)
                {
                    fieldList.Add(fields[u]);
                }
            }
            return fieldList.ToArray();
        }
    }
}