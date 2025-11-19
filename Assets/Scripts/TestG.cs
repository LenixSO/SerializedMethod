using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableMethods;

[SerializeClassMethods]
public class TestG : MonoBehaviour
{
    public void TestClass(UnknownClass c)
    {

    }

    public void TestStruct(UnknownStruct s)
    {
    }
}

public class UnknownClass
{
    public string name;
    public int age;
    public bool alive {  get; set; }
    public bool gene { get; }
}
public struct UnknownStruct
{
    public string text;
    public int value;
    public bool flag {  get; set; }
    public bool getOnly { get; }
}