using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SerializableMethods
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ClickableMethodsEditor : Editor
    {
        private Dictionary<string, object> methodParameters = new();

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            root.Add(new IMGUIContainer(OnInspectorGUI));
            
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