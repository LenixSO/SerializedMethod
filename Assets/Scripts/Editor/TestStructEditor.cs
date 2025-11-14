using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(TestStruct))]
public class TestStructEditor : PropertyDrawer
{
    /*public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return new Label("it works");
    }*/

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        EditorGUI.LabelField(position,"B");
    }
}
