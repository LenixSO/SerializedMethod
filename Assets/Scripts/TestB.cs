using UnityEngine;
using SerializableMethods;

public class TestB : MonoBehaviour
{
    [SerializeMethod]void reference(GameObject obj){}
    [SerializeMethod]void color(Color color){}
    void vector(Vector2 vector2){}
    void vector(Vector3 vector3){}
    void vector(Vector4 vector4){}
    [SerializeMethod]void rect(Rect rect){}
    void bounds(Bounds bounds){}
}
