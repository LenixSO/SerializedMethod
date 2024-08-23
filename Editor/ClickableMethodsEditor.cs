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

        public override void OnInspectorGUI()
        {
            Debug.Log("?");
            base.OnInspectorGUI();
        }

        public override VisualElement CreateInspectorGUI()
        {
            Debug.Log("AA");
            VisualElement root = new();
            
            //Load fields
            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                            BindingFlags.Public | BindingFlags.NonPublic);
            //Debug.Log(fields.Length);
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
    }
}