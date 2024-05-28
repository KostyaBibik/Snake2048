using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICustomizationModel
{
    IEnumerable<object> GetContent();
}

public struct CustomColor
{
    public Color Value;
    public EColorDestiny destiny;
}

public enum EColorDestiny
{
    None,
    Text,
    Image
}

public class UniversalCustomizationModel : ICustomizationModel
{
    private object[] _values;

    public UniversalCustomizationModel(params object[] values)
    {
        _values = values;
    }

    public IEnumerable<object> GetContent()
    {
        return _values;
    }
}

public class Customization : ICustomization
{
    private Image _image;
    private TextMeshProUGUI _text;

    public Customization(GameObject container)
    {
        foreach (Transform child in container.transform)
        {
            foreach (var graphic in child.GetComponentsInChildren<Graphic>())
            {
                AddTarget(graphic);
            }  
        }

        if (_image == null)
        {
            _image = container.GetComponent<Image>();
        }
    }
    
    public void AddTarget(Graphic target)
    {
        if (target is Image image)
        {
            _image = image;
        }
        
        if (target is TextMeshProUGUI text)
        {
            _text = text;
        }
    }
    
    public void SetContent(ICustomizationModel customizationModel)
    {
        if (customizationModel == null)
        {
            return;
        }

        foreach (var content in customizationModel.GetContent())
        {
            if (content is string text)
            {
                if (_text != null)
                {
                    _text.text = text;
                }
            }
            else if (content is Sprite sprite)
            {
                if (_image != null)
                {
                    _image.sprite = sprite;
                }
            }
            else if (content is CustomColor customColor)
            {
                switch (customColor.destiny)
                {
                    case EColorDestiny.Image:
                    {
                        if (_image != null)
                        {
                            _image.color = customColor.Value;
                        }

                        break;
                    }
                    case EColorDestiny.Text:
                    {
                        if (_text != null)
                        {
                            _text.color = customColor.Value;
                        }

                        break;
                    }
                }
            }
        }
    }
}
