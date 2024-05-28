using System;
using UIKit.Elements.Models;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIKit.Elements
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : UIElementView<ButtonModel>, IPointerEnterHandler, IPointerExitHandler
    {
        private Button _button;
        private ICustomization _customization;
        private UIElementBase _rootView;
        private Action _clickCallback;
        private Action _pointerEnterCallback;
        private Action _pointerExitCallback;

        public void Customize(ICustomizationModel customization)
        {
            if (_customization == null)
            {
                _customization = new Customization(gameObject);
            }
            
            _customization.SetContent(customization);
        }
        
        public override void Initialization()
        {
            _button = GetComponent<Button>();
        }

        protected override void UpdateView(ButtonModel model)
        {
            if (_rootView == null)
            {
                _rootView = transform.parent.GetComponentInParent<UIElementBase>(true);
            }
            
            if (model == null)
            {
                return;
            }

            var activeButton = model.ClickCallback != null;
            
            Customize(model.CustomizationModel);
            
            _button.interactable = activeButton;
            
            _pointerEnterCallback = null;
            _pointerExitCallback = null;
            
            if (activeButton)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(() =>
                {
                    if (_rootView == null || _rootView.IsShow)
                    {
                        model.ClickCallback.Invoke();
                    }
                });
                _pointerEnterCallback += model.OnPointerEnter;
                _pointerExitCallback += model.OnPointerExit;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _pointerEnterCallback?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerExitCallback?.Invoke();
        }
    }
}