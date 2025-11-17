using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableMethods;
using System;

public class TestF : MonoBehaviour
{
    int deptht = 0;
    [SerializeMethod]
    private void TestListInt(List<bool> list)
    {
    }
    [SerializeMethod]
    private void TestArrayInt(int[] array)
    {
    }
    [SerializeMethod]
    private void TestStackInt(Queue<string> list)
    {
    }

    //private bool CheckForInterface(Type type, Type interfaceType)
    //{
    //    deptht++;
    //    bool hasInterface = type == interfaceType;
    //    Debug.Log($"({deptht}){type.Name} == {interfaceType.Name} | {hasInterface}");
    //    var interfaces = type.GetInterfaces();
    //    for (int i = 0; i < interfaces.Length; i++)
    //    {
    //        if (hasInterface) break;
    //        hasInterface |= CheckForInterface(interfaces[i], interfaceType);
    //    }
    //    deptht--;
    //    return hasInterface;
    //}
}
