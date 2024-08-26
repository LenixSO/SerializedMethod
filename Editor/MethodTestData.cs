using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SerializableMethods
{
    public class MethodTestData
    {
        private string path;
        private string filePath;
        private Dictionary<string, object> methodData;
        public Dictionary<string, object> methodParameters
        {
            get
            {
                if (methodData == null) methodData = Load();
                return methodData;
            }
        }

        public MethodTestData()
        {
            path = Directory.GetCurrentDirectory() + "\\Temp\\";
            filePath = path + "methodTestData.json";
            //Debug.Log(Directory.GetFiles(path).Length);
        }

        Dictionary<string, object> Load()
        {
            Dictionary<string, object> data = new();
            if (File.Exists(filePath))
            {
                DataContractJsonSerializer son = new DataContractJsonSerializer(typeof(Dictionary<string, object>));
                FileStream file = new FileStream(filePath, FileMode.Open);
                file.Position = 0;
                data = (Dictionary<string, object>)son.ReadObject(file);

                file.Close();
                data = DeserializeObjects(data);
                return data;
            }

            Debug.LogWarning("Saved data not found");
            return data;
        }

        private Dictionary<string, object> DeserializeObjects(Dictionary<string, object> data)
        {
            Dictionary<string, object> returnData = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value != null)
                {
                    try
                    {
                        object value = JsonUtility.FromJson<ObjectID>((string)item.Value).ToObject();
                        returnData.Add(item.Key, value);
                    }
                    catch
                    {
                        object value = item.Value;
                        if (item.Value.GetType() == typeof(object[]))
                        {
                            //Debug.Log("deserialized object");
                            object[] deserializedStruct = (object[])item.Value;
                            value = StructSerializer.DeserializeStruct(deserializedStruct);
                        }
                        if (value is SerializedEnum)
                        {
                            value = ((SerializedEnum)value).ToEnum();
                        }
                        returnData.Add(item.Key, value);
                        //Debug.Log($"L - {item.Key}({item.Value.GetType()}) value: {returnData[item.Key]}");
                    }
                }
                else returnData.Add(item.Key, null);

            }

            return returnData;
        }

        public void Save()
        {
            List<Type> knownTypes = new()
            {
                typeof(List<object>)
            };
            DataContractJsonSerializer son = new DataContractJsonSerializer(typeof(Dictionary<string, object>), knownTypes);
            //Debug.Log(filePath);
            FileStream file = new FileStream(filePath, FileMode.Create);
            son.WriteObject(file, SerializableDictionary());
            file.Close();
        }

        private Dictionary<string, object> SerializableDictionary()
        {
            Dictionary<string, object> returnType = new();

            foreach (KeyValuePair<string, object> item in methodData)
            {
                try
                {
                    if (item.Value != null)
                        returnType.Add(item.Key, JsonUtility.ToJson(new ObjectID((UnityEngine.Object)item.Value)));
                    else returnType.Add(item.Key, item.Value);
                }
                catch
                {
                    //Debug.Log($"is {item.Key}({item.Value.GetType()}) serializable? {item.Value.GetType().IsSerializable}");
                    object value = item.Value;
                    if (value != null)
                    {
                        if (value.GetType().IsEnum)
                        {
                            value = new SerializedEnum((Enum)value);
                        }
                        if (!value.GetType().IsSerializable)
                        {
                            value = StructSerializer.SerializeStruct(value, value.GetType());
                        }
                    }
                    returnType.Add(item.Key, value);
                    //Debug.Log($"S - {item.Key} value: {returnType[item.Key]}");
                }
            }
            return returnType;
        }

        public void DeleteSave()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
    [Serializable]
    public class ObjectID
    {
        public int id;
        public string name;
        public ObjectID(Object obj)
        {
            id = obj.GetInstanceID();
            name = obj.name;
        }

        public Object ToObject() => EditorUtility.InstanceIDToObject(id);
    }

    public struct SerializedEnum
    {
        public string _value;
        public string typeName;

        public SerializedEnum(Enum value)
        {
            Type type = value.GetType();
            _value = Enum.ToObject(type,value).ToString();
            typeName = type.AssemblyQualifiedName;
        }
        
        public Enum ToEnum()
        {
            Type type = Type.GetType(typeName);
            object value = Enum.Parse(type, _value);

            return (Enum)value;
        }
    }
}