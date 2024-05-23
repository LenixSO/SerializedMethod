using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Reflection;

namespace SerializableMethods
{
    public class MethodTestWindow : EditorWindow
    {
        //const fields
        private const string TargetObjectLabel = "Target Object";
        private const string PickClassLabel = "Pick class:";
        private const string PickClassDropdown = "Pick a class:";
        private const string MethodAreaName = "Method area";

        //saved values
        MethodTestData data = new();
        private GameObject target = null;
        private int classChoice = -1;

        [MenuItem("Tools/MethodTesting")]
        public static void ShowWindow()
        {
            MethodTestWindow window = GetWindow<MethodTestWindow>();
            window.titleContent = new GUIContent("MethodTesting");
        }

        public void CreateGUI()
        {
            //data.DeleteSave();
            //methodParameters.Clear();
            VisualElement root = rootVisualElement;
            int padding = 5;
            root.style.paddingTop = padding;
            root.style.paddingBottom = padding;
            root.style.paddingLeft = padding;
            root.style.paddingRight = padding;

            VisualElement Header = new VisualElement();
            Header.style.flexDirection = FlexDirection.Row;
            //Create Elements
            string label = TargetObjectLabel;
            ObjectField targetObjectField = new ObjectField(label);
            targetObjectField.name = label;
            targetObjectField.style.flexGrow = .2f;
            targetObjectField.objectType = typeof(GameObject);
            targetObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(LoadClassOptions);

            VisualElement pickClassArea = new VisualElement();
            pickClassArea.name = PickClassLabel;
            pickClassArea.style.flexGrow = 1;

            VisualElement methodsArea = new Image();
            methodsArea.name = MethodAreaName;
            methodsArea.style.flexDirection = FlexDirection.Row;
            methodsArea.style.flexWrap = Wrap.Wrap;

            //add elements
            Header.Add(targetObjectField);
            Header.Add(pickClassArea);
            root.Add(Header);
            root.Add(methodsArea);

            targetObjectField.value = target;
        }

        private void LoadClassOptions(ChangeEvent<UnityEngine.Object> evt)
        {
            VisualElement area = rootVisualElement.Q(PickClassLabel);
            area.Clear();
            if (evt.newValue.GetType() != typeof(GameObject) || evt.newValue == null)
            {
                target = null;
                return;
            }

            target = (GameObject)evt.newValue;

            //dropDown For picking class
            DropdownField dropdownField = new DropdownField(PickClassDropdown);
            dropdownField.name = PickClassDropdown;
            List<string> options = new();
            Component[] components = target.GetComponents<Component>();
            for (int i = 1; i < components.Length; i++) //starts at 1 to ignore the transform component
                options.Add(components[i].GetType().Name);
            dropdownField.choices = options;

            dropdownField.RegisterCallback<ChangeEvent<string>>(GetClass);

            area.Add(dropdownField);
            if (classChoice >= 0 && classChoice < dropdownField.choices.Count)
                dropdownField.value = dropdownField.choices[classChoice];
        }

        private void GetClass(ChangeEvent<string> evt)
        {
            DropdownField dropdown = rootVisualElement.Q<DropdownField>(PickClassDropdown);
            if (!dropdown.choices.Contains(evt.newValue)) return;

            classChoice = dropdown.choices.IndexOf(evt.newValue);

            LoadMethods(target.GetComponents<Component>()[classChoice + 1].GetType()); //still ignoring transform component
        }

        private void LoadMethods(Type targetClass)
        {
            MethodInfo[] methods = SerializeMethodHelper.GetMethods(targetClass);
            VisualElement methodsArea = rootVisualElement.Q(MethodAreaName);
            methodsArea.Clear();
            foreach (MethodInfo method in methods)
            {
                SerializeMethodHelper.ShowMethod(target, method, methodsArea);
            }

            SerializeMethodHelper.SaveData();
        }

    }
}