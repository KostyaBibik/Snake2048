using System;
using UISystem;

namespace GameUtilities.UI
{
    public class UIElementInitializationException : System.Exception
    {
        public UIElementBase Element { get; }
        public UIElementInitializationException(UIElementBase element, Exception exception) : 
            base($"UI Element {element.name} initialization critical error.", exception)
        {
            Element = element;
        }
    }
}