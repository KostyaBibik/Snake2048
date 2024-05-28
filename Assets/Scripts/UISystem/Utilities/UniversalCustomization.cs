using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UniversalCustomization : ICustomization
{
    private readonly List<Image> _images = new();
    private readonly List<TextMeshProUGUI> _texts = new();

    public UniversalCustomization(GameObject container)
    {
        foreach (Transform child in container.transform)
        {
            foreach (var graphic in child.GetComponentsInChildren<Graphic>())
            {
                AddTarget(graphic);
            }
        }

        if (_images.Count == 0 && container.TryGetComponent(out Image image))
        {
            AddTarget(image);
        }
    }

    public void AddTarget(Graphic target)
    {
        if (target is Image image)
        {
            _images.Add(image);
        }

        if (target is TextMeshProUGUI text)
        {
            _texts.Add(text);
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
                if (_texts.Count != 0)
                {
                    foreach (var textContainer in _texts)
                    {
                        textContainer.text = text;
                    }
                }
            }
            else if (content is Sprite sprite)
            {
                if (_images.Count != 0)
                {
                    foreach (var imageContainer in _images)
                    {
                        imageContainer.sprite = sprite;
                    }
                }
            }
            else if (content is Tuple<Sprite, string> customSprite)
            {
                SetImageByName(customSprite.Item2, customSprite.Item1);
            }
            else if (content is CustomColor color)
            {
                switch (color.destiny)
                {
                    case EColorDestiny.Text:
                    {
                        if (_texts.Count != 0)
                        {
                            foreach (var textContainer in _texts)
                            {
                                textContainer.color = color.Value;
                            }
                        }
                    }
                        break;
                    default:
                        return;
                }
            }
            else if (content is Tuple<CustomColor, string> customColor)
            {
                switch (customColor.Item1.destiny)
                {
                    case EColorDestiny.Image:
                        SetImageColor(customColor.Item2, customColor.Item1.Value);
                        break;

                    case EColorDestiny.Text:
                        SetTextColor(customColor.Item2, customColor.Item1.Value);
                        break;
                }
            }
        }
    }
    
    private void SetImageByName(string name, Sprite sprite)
    {
        if (_images.Count != 0)
        {
            foreach (var imageContainer in _images)
            {
                if (imageContainer.name == string.Concat("$", name))
                {
                    imageContainer.sprite = sprite;
                }
            }
        }
    }
    
    private void SetImageColor(string name, Color color)
    {
        if (_images.Count != 0)
        {
            foreach (var imageContainer in _images)
            {
                if (imageContainer.name == string.Concat("$", name))
                {
                    imageContainer.color = color;
                }
            }
        }
    }
    
    private void SetTextColor(string name, Color color)
    {
        if (_texts.Count != 0)
        {
            foreach (var textContainer in _texts)
            {
                if (textContainer.name == string.Concat("$", name))
                {
                    textContainer.color = color;
                }
            }
        }
    }
}