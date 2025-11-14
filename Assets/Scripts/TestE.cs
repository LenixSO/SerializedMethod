using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestE : TestA
{
    public string text;

    protected override void TestOverride()
    {
        base.TestOverride();
        Debug.Log("override");
    }

    public void CallCoroutine()
    {
        Debug.Log("call coroutine");
        StartCoroutine(TestCoroutine(2));
    }

    IEnumerator TestCoroutine(int a)
    {
        Debug.Log("coroutine called");
        yield return new WaitForSeconds(a);
        Debug.Log("coroutine awaited");
    }
}