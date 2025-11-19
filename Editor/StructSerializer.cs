using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

public static class StructSerializer
{
    public static object[] SerializeStruct(object structure,Type structType)
    {
        List<object> data = new();
        data.Add(structType.AssemblyQualifiedName);
        //Debug.Log($"serializing: object of type {data[0]}");

        if(structure == null) return data.ToArray();

        FieldInfo[] fields = structType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        for (int i = 0; i < fields.Length; i++)
        {
            object field = fields[i].GetValue(structure);
            if (!fields[i].FieldType.IsSerializable)
            {
                field = SerializeStruct(field, fields[i].FieldType);
            }
            //Debug.Log($"{fields[i].Name} added with value: {field}");
            data.Add(field);
        }

        return data.ToArray();
    }
    
    public static object DeserializeStruct(object[] data)
    {
        Type valueType = Type.GetType(data[0].ToString());
        object returnValue = default;

        try
        {
            returnValue = Activator.CreateInstance(valueType);
        }
        catch
        {
            UnityEngine.Debug.LogWarning($"Could not create instance of ({valueType}), this type does not have a generic constructor");
            return default;
        }

        //UnityEngine.Debug.Log($"deserialized type is {valueType} - {returnValue}");
        
        FieldInfo[] fields = valueType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        for (int i = 0; i < fields.Length; i++)
        {
            object field = fields[i].GetValue(returnValue);
            if (!fields[i].FieldType.IsSerializable)
            {
                //Debug.Log($"{fields[i].Name}({fields[i].FieldType}) field also needs to be deserialized: {data[1+i].GetType()}");
                field = DeserializeStruct((object[])data[1 + i]);
            }
            else
            {
                field = data[1 + i];
                //Debug.Log($"{fields[i].Name}({fields[i]}) field value is {field}");
            }
            //Debug.Log($"field is {fields[i].FieldType} and value is {field.GetType()}");
            if(fields[i].FieldType == typeof(Single)) field = (float)Convert.ToDecimal(field);
            fields[i].SetValue(returnValue,field);
        }
        
        return returnValue;
    }
}
