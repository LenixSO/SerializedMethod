# Serialized Method
A package for serializing class methods in unity editor.
# SerializeMethod Attribute
By adding the **SerializedMethod** attribute to a method in a MonoBehaviour class, a button to call that method will appear in the inspector of that class.

![example](https://cdn.discordapp.com/attachments/709946849817133136/1252299370465071246/attribute.png?ex=6671b62c&is=667064ac&hm=4bc48df9073943b7e541b043d1f8bbff41669a6f62769754718331727c63e701&)

The method can also have parameters and return values

![param and return](https://cdn.discordapp.com/attachments/709946849817133136/1252307068246032405/image.png?ex=6671bd57&is=66706bd7&hm=562da2c8d3a791ba1a3cb50ad5e9070e566933489965d5c8f05ef5bc9ea1605d&)

The package supports most of C# and Unity's basic types when serializing parameters for the functions, but in case you want to serialize a function with a unsupported parameter type you can create your own serialization.
## Creating your own SerializedObject
The parameters are serialized using [Unity's VisualElement](https://docs.unity3d.com/Manual/UIE-uxml-element-VisualElement.html), and you can create a template of your serialization from the creation menu:

![template](https://cdn.discordapp.com/attachments/709946849817133136/1252310249625489489/image.png?ex=6671c04e&is=66706ece&hm=c7281c981dafc04468aa799b4a046f52bd3cb47ebb7d35a4bf6b2ae0e7aed7d0&)

This will create a template script for your serialization, with all the basics you need to make it work. The template also comes with an example of how a custom serialization would look like for an integer parameter (which already has its serialization).

***The script must be on the Resources folder to be detected!***

![types](https://cdn.discordapp.com/attachments/709946849817133136/1252325589365293098/image.png?ex=6671ce97&is=66707d17&hm=ed0e7b8ee6a2ead1af37f98e58a184c7e09de22f9e14110425a2fd9ef2c52fbd&)

**usedTypes:** The type(s) this script will be used to serialize. this usually will contain only one type, but in some ocasions it might be possible to use the same script for multiple types.

![method](https://cdn.discordapp.com/attachments/709946849817133136/1252346086216044714/image.png?ex=6671e1ae&is=6670902e&hm=7a2ea4e57205eaa675784c715ba911baadf90d35c04087d56fb27f623b4cbfce&)

**GetElement:** The method that will return the [VisualElement](https://docs.unity3d.com/Manual/UIE-VisualTree.html) that represents the serialization of the **usedTypes**. It's parameters are:
- *label:* The name of the parameter being serialized.
- *value:* The initial value of the parameter.
- *type:* The parameter type.
- *onValueChanged:* Invoke this action when the parameter value changes in the inspector so it can be internally saved by the package.

# MethodTesting Window
If you don't want to add the ***SerializeMethod*** attribute to all methods in a class, you can also see all non-static methods of a MonoBehaviour class using the **MethodTest** Window.
![tool](https://cdn.discordapp.com/attachments/709946849817133136/1252359710838100098/image.png?ex=6671ee5e&is=66709cde&hm=ccd822e132ae8906f20c81e51d33bbe2c115936c4244d4167f048cf121f8f930&)
Just pick the *Target object(GameObject)* and the *Class(Component)* and all all non-static methods will appear in the window the same way they would using the attribute.
![window](https://cdn.discordapp.com/attachments/709946849817133136/1252362361457868901/image.png?ex=6671f0d6&is=66709f56&hm=0301d8271ab4ad840e266e896c2965abe095c0a1dae98bb70a2b5a9bbd8e6e6f&)
