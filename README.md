# Serialized Method
A package for serializing class methods in unity editor.
# SerializeMethod Attribute
By adding the **SerializedMethod** attribute to a method in a MonoBehaviour class, a button to call that method will appear in the inspector of that class.

![image](https://github.com/user-attachments/assets/3662b3de-2baf-46ef-9a67-ddcd87e85085)

The method can also have parameters and return values

![image](https://github.com/user-attachments/assets/01edf9bf-58b0-4ce2-999d-b7449a7e936d)

You can also add the **SerializeClassMethods** attribute to your classes to serialize all public and/or not public methods in that class

<img width="730" height="49" alt="image" src="https://github.com/user-attachments/assets/02c0ed89-a75c-4fd6-b0e4-e3c358b6cd5e" />


The package supports most of C# and Unity's basic types when serializing parameters for the functions, and it can also serialize some types of strucs and classes, but in case you want to serialize a function with a unsupported parameter type you can create your own serialization.
## Creating your own SerializedObject
The parameters are serialized using [Unity's VisualElement](https://docs.unity3d.com/Manual/UIE-uxml-element-VisualElement.html), and you can create a template of your serialization from the creation menu:

![image](https://github.com/user-attachments/assets/bac18d84-20a9-4b79-bcc4-26d39417cc45)

This will create a template script for your serialization, with all the basics you need to make it work. The template also comes with an example of how a custom serialization would look like for an integer parameter (which already has its serialization).

***The script must be on the Resources folder to be detected!***

![image](https://github.com/user-attachments/assets/1f6aec21-8f10-4699-a5ce-3c9e3bae43e2)

**usedTypes:** The type(s) this script will be used to serialize. this usually will contain only one type, but in some ocasions it might be possible to use the same script for multiple types.

![image](https://github.com/user-attachments/assets/170e8db9-00a3-4e90-b822-44ff75b45b63)

**GetElement:** The method that will return the [VisualElement](https://docs.unity3d.com/Manual/UIE-VisualTree.html) that represents the serialization of the **usedTypes**. It's parameters are:
- *label:* The name of the parameter being serialized.
- *value:* The initial value of the parameter.
- *type:* The parameter type.
- *onValueChanged:* Invoke this action when the parameter value changes in the inspector so it can be internally saved by the package.

# MethodTesting Window
If you don't want to add the ***SerializeMethod*** attribute to all methods in a class, you can also see all non-static methods of a MonoBehaviour class using the **MethodTest** Window.

![image](https://github.com/user-attachments/assets/9f272bae-9adb-4d80-b5d9-c5665b21427e)

Just pick the *Target object(GameObject)* and the *Class(Component)* and all all non-static methods will appear in the window the same way they would using the attribute.

![image](https://github.com/user-attachments/assets/d43ae0b7-ddfc-4c65-a10e-94250310f3ee)
