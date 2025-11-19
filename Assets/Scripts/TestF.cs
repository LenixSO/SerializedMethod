using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableMethods;
using System;

[SerializeClassMethods(MethodType.NotPublic)]
public class TestF : MonoBehaviour
{
    int deptht = 0;
    private void TestListInt(List<bool> list)
    {
    }
    private void TestArrayInt(int[] array)
    {
    }
    private void TestStackInt(Queue<string> list)
    {
    }

    public void TestPublic()
    {

    }
    private void TestPrivate()
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
