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
        Debug.Log($"list: {list.Count}");
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log($"{i} => {list[i]}");
        }
    }
    private void TestArrayInt(int[] array)
    {
        Debug.Log($"array: {array.Length}");
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log($"{i} => {array[i]}");
        }
    }
    
    private void TestStackInt(Stack<string> stack)
    {
        Debug.Log($"stack: {stack.Count}");
        int size = stack.Count;
        for (int i = 0; i < size; i++)
        {
            Debug.Log($"{i} => {stack.Pop()}");
        }
    }
    
    private void TestQueueInt(Queue<string> stack)
    {
        Debug.Log($"queue: {stack.Count}");
        int size = stack.Count;
        for (int i = 0; i < size; i++)
        {
            Debug.Log($"{i} => {stack.Dequeue()}");
        }
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
