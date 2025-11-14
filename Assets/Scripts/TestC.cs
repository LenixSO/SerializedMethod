using System;
using UnityEngine;

public class TestC : MonoBehaviour,ITestGenericInterface
{

    void Method(Action action)
    {

    }
    void Method(TestA testA)
    {

    }
    void Method(TestGenericClass testClass)
    {

    }
    void Method(ITestGenericInterface testInterface)
    {

    }
}