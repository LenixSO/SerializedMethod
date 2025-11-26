using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace SerializableMethods
{
    public class SerializeMethodData
    {
        private string path;
        private string filePath;
        private Dictionary<string, object> methodData;
        public Dictionary<string, object> methodParameters
        {
            get
            {
                if (methodData == null)
                {
                    try { methodData = Load(); }
                    catch { methodData = new(); }
                }
                return methodData;
            }
        }


        private readonly List<Type> knownTypes = 
            new()
            {
                typeof(List<object>),
                typeof(SerializedEnum),
                typeof(SerializedCollection),
                typeof(ObjectID),
            };

        public SerializeMethodData()
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
                DataContractJsonSerializer son = new DataContractJsonSerializer(typeof(Dictionary<string, object>), knownTypes);
                FileStream file = new FileStream(filePath, FileMode.Open);
                try
                {
                    file.Position = 0;
                    data = (Dictionary<string, object>)son.ReadObject(file);

                    file.Close();
                    data = DeserializeObjects(data);
                    return data;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading data:\n\n({ex.GetType().Name}){ex.Message}");
                    file.Close();
                    DeleteSave();
                }
                finally { file.Close(); }
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
                        if (value is SerializedCollection)
                        {
                            value = ((SerializedCollection)value).GetCollection();
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
            DataContractJsonSerializer son = new DataContractJsonSerializer(typeof(Dictionary<string, object>), knownTypes);
            //Debug.Log(filePath);
            FileStream file = new FileStream(filePath, FileMode.Create);
            try { son.WriteObject(file, SerializableDictionary()); }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving data:\n\n{ex.Message}");
                file.Close();
                DeleteSave();
            }
            finally { file.Close(); }
        }

        private Dictionary<string, object> SerializableDictionary()
        {
            Dictionary<string, object> returnType = new();

            foreach (KeyValuePair<string, object> item in methodParameters)
            {
                object value = item.Value;
                if(value != null)
                {
                    if (value.GetType().IsSubclassOf(typeof(UnityEngine.Object))) returnType.Add(item.Key, JsonUtility.ToJson(new ObjectID((UnityEngine.Object)item.Value)));
                    else
                    {
                        Type type = value.GetType();
                        if (type.IsEnum)
                        {
                            value = new SerializedEnum((Enum)value);
                        }
                        if (!type.IsSerializable)
                        {
                            value = StructSerializer.SerializeStruct(value, value.GetType());
                        }
                        if (typeof(IEnumerable).IsAssignableFrom(type))
                        {
                            value = new SerializedCollection(value as IEnumerable);
                        }

                        returnType.Add(item.Key, value);
                    }
                }
                else returnType.Add(item.Key, value);
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
    public struct SerializedCollection
    {
        public string typeName;
        public List<object> elements;
        public SerializedCollection(IEnumerator enumerator)
        {
            typeName = enumerator.GetType().AssemblyQualifiedName;
            elements = new();
            FillList(enumerator);
        }
        public SerializedCollection(IEnumerable enumerable)
        {
            typeName = enumerable.GetType().AssemblyQualifiedName;
            elements = new();
            FillList(enumerable.GetEnumerator());
        }

        private void FillList(IEnumerator enumerator)
        {
            enumerator.Reset();
            while (enumerator.MoveNext())
                elements.Add(enumerator.Current);
        }

        public object GetCollection()
        {
            Type collectionType = Type.GetType(typeName);
            if (collectionType.IsArray) return ParseArray(collectionType);
            return ParseCollection(collectionType);
        }

        private object ParseArray(Type type)
        {
            var elementType = type.GetElementType();
            var parseMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(elementType);
            var array = parseMethod.Invoke(null, new object[] { elements.ToArray() });
            return (array as IEnumerable).GetEnumerator();
        }

        private object ParseCollection(Type type)
        {
            var elementType = type.GenericTypeArguments[0];
            var parseMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(elementType);
            var collectionConstructor = type.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
            if (collectionConstructor == null)
            {
                Debug.LogWarning("Couldn't find an appropriate constructor to parse value");
                return null;
            }
            
            var collection = collectionConstructor.Invoke(new[] { parseMethod.Invoke(null, new object[] { elements }) });
            return collection;
        }
    }
}