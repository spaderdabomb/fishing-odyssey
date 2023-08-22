using UnityEngine;
using UnityEngine.UIElements;

class CustomElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<CustomElement, UxmlTraits> { }

    // Add the two custom UXML attributes.
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_String =
            new UxmlStringAttributeDescription { name = "my-string", defaultValue = "default_value" };
        UxmlIntAttributeDescription m_Float =
            new UxmlIntAttributeDescription { name = "my-float", defaultValue = 2 };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as CustomElement;

            ate.myString = m_String.GetValueFromBag(bag, cc);
            ate.myFloat = m_Float.GetValueFromBag(bag, cc);
        }
    }

    // Must expose your element class to a { get; set; } property that has the same name 
    // as the name you set in your UXML attribute description with the camel case format
    public string myString { get; set; }

    // Property for our custom attribute
    public float myFloat
    {
        get
        {
            return resolvedStyle.width;
        }
        set
        {
            style.width = value;
        }
    }
}