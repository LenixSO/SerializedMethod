using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class CreateCustomSerialization : EditorWindow
{
    private const string className = "$1";
    private static string template;

    private static bool editing = false;
    private static bool added = false;

    private static string ScriptTemplate
    {
        get
        {
            if (string.IsNullOrEmpty(template))
            {
                template = "using System;\n";
                template += "using SerializableMethods;\n";
                template += "using UnityEditor.UIElements;\n";
                template += "using UnityEngine.UIElements;";
                template += "\n\n//A Custom Serialized Object must be in the Resources Folder to be detected";
                template += $"\npublic class {className} : ISerializedObject";
                template += "\n{";
                template += $"\n   public {className}(){{ }}";
                template += "\n   public Type[] usedTypes => new [] { typeof(object)/*change this to your desired type*/ };";
                template += "\n   public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)\n   {";
                template += "\n       //A simple example of an integer custom serialization:";
                template += "\n       /*";
                template += "\n       IntegerField field = new IntegerField(label);";
                template += "\n       field.value = value == null ? default : (int)value;";
                template += "\n       field.RegisterCallback<ChangeEvent<int>>(evt => onValueChanged?.Invoke(evt.newValue));";
                template += "\n       return field;";
                template += "\n       */";
                template += "\n       ";
                template += "\n       return new VisualElement();";
                template += "\n   }";
                template += "\n}";
            }

            return template;
        }
    }

    [MenuItem("Assets/Create/SerializedMethod/Custom SerializedParameter")]
    static void CreateScript()
    {
        ProjectWindowUtil.CreateAssetWithContent("newSerializedObject.cs", ScriptTemplate);
        editing = true;
        if (!added)
        {
            Selection.selectionChanged += SelectionHook;
            added = true;
        }
    }

    private static void SelectionHook()
    {
        editing = !editing;
        if (editing && Selection.activeObject != null)
        {
            Selection.selectionChanged -= SelectionHook;
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            string scriptName = Path.GetFileNameWithoutExtension(assetPath);
            File.WriteAllText(assetPath, ScriptTemplate.Replace(className, scriptName));
            AssetDatabase.Refresh();// <-- not strictly necessary
            editing = false;
        }
    }
}