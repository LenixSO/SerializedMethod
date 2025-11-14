using UnityEngine;
using SerializableMethods;

public class TestD : MonoBehaviour
{
    [SerializeField] TestStruct testCustom;
    public int A;

    [SerializeMethod]
    void Method(TestGenericClass testClass)
    {

    }
    void StructTest(TestStruct testStruct)
    {

    }
    void Struct(Vector2 vector)
    {

    }
    void EnumTest(TestEnum testEnum)
    {
    }
    [SerializeMethod]
    void NullableTest(decimal? value)
    {
    }
    [SerializeMethod]
    void DecimalTest(decimal value)
    {
    }
    [SerializeMethod]
    void DoubleTest(decimal value)
    {
    }
}

[System.Serializable]
public struct TestStruct
{
    public float value;
    public bool toggle;
}

public enum TestEnum
{
    a, b, c
}