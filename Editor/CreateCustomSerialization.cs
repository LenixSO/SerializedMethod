using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class CreateCustomSerialization : EditorWindow
{

    [MenuItem("Assets/SerializedMethod/Create Custom SerializedParameter")]
    static void CreateScript()
    {
        string filePath = EditorUtility.SaveFilePanel("PickLocation", "Assets", "", "cs");
        string fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
        fileName = fileName.Remove(fileName.LastIndexOf('.'));

        Debug.Log($"{filePath}\n{fileName}");
        FileStream file = new FileStream(filePath, FileMode.Create);
        AddText(file, "using System;\nusing SerializableMethods;\nusing UnityEditor.UIElements;\nusing UnityEngine.UIElements;");
        AddText(file, "\n\n//A Custom Serialized Object must be in the Resources Folder to be detected");
        AddText(file, $"\npublic class {fileName} : ISerializedObject");
        AddText(file, "\n{");
        AddText(file, "\n   public Type[] usedTypes => new [] { typeof(object)/*change this to your desired type*/ };");
        AddText(file, "\n   public VisualElement GetElement(string label, object value, Type type, Action<object> onValueChanged)\n   {");
        AddText(file, "\n       return new VisualElement();");
        AddText(file, "\n   }");
        AddText(file, "\n}");
        file.Close();
        AssetDatabase.Refresh();
    }

    static void AddText(FileStream file,string text)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(text);
        file.Write(info, 0, info.Length);
    }
}