using UnityEngine;
using Random = UnityEngine.Random;
using SerializableMethods;

public class TestA : MonoBehaviour
{
    [SerializeField] private string a;
    [SerializeField] protected string b;
    public string c;

    private void PrivateNoParam()
    {
        Debug.Log("invoking privateNoParam method");
    }

    [SerializeMethod]
    int WithReturnType()
    {
        //Debug.Log("invoking WithReturnType method");
        return Random.Range(0,10);
    }

    [SerializeMethod]
    protected int Protected()
    {
        //Debug.Log("invoking WithReturnType method");
        return Random.Range(0, 10);
    }

    [SerializeMethod]
    public void WithParams(string s, float f = 0, int i = 2, bool b = false)
    {
        Debug.Log($"invoking WithParams method.\n params: i={i} | n={f} | s=\"{s}\" | b={b}");
    }

    public GameObject TestReturnObject() => gameObject;

    protected virtual void TestOverride()
    {
        Debug.Log("virtual");
    }
}
