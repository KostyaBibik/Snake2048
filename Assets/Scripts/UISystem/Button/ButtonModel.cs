using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UIKit.Elements.Models
{
    public class ButtonModel
    {
        [CanBeNull] public Action ClickCallback { get; set; }
        [CanBeNull] public Action OnPointerEnter { get; set; }
        [CanBeNull] public Action OnPointerExit { get; set; }
        [CanBeNull] public ICustomizationModel CustomizationModel { get; set; }

        public ButtonModel()
        {
        }
        
        public ButtonModel(ICustomizationModel customizationModel)
        {
            CustomizationModel = customizationModel;
        }
    }
}