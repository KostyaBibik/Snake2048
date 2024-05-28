using System;
using System.Collections.Generic;
using System.Linq;
using GameUtilities.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UISystem
{
    public class UIManager : StaticBehaviour<UIManager>
    {
        private readonly List<UIElementBase> _cachedUIElements = new();

        public static GameObject InstantiateElement(GameObject element)
        {
            var instance = Instantiate(element, element.transform.parent);
            var elements = instance.GetComponentsInChildren<UIElementBase>();
            
            foreach (var childView in elements.OrderByDescending(x => x.gameObject.GetLayer()))
            {
                Instance.InitializeElement(childView);
            }

            return instance;
        }
        
        public static T InstantiateElement<T>(T element) where T : UIElementBase
        {
            var instance = Instantiate(element, element.transform.parent);
            var elements = instance.GetComponentsInChildren<UIElementBase>()
                .Concat(new []{ instance });
            
            foreach (var childView in elements.OrderByDescending(x => x.gameObject.GetLayer()))
            {
                Instance.InitializeElement(childView);
            }

            return instance;
        }

        public int GetUIElementsCount(UIElementOption option = UIElementOption.None)
        {
            var count = _cachedUIElements.Count(x => IsValid(x, option));

            return count;
        }

        public List<UIElementBase> GetUIElements(UIElementOption option = UIElementOption.None)
        {
            var cachedElements = _cachedUIElements.FindAll(x => IsValid(x, option));

            return cachedElements;
        }

        public T GetUIElement<T>(UIElementOption option = UIElementOption.None) where T : UIElementBase
        {
            var cachedElements = _cachedUIElements.FindAll(x => x is T && IsValid(x, option));

            if (cachedElements.Count == 0)
                return null;

            if (cachedElements.Count > 1)
                throw new Exception($"More than one UIElement({typeof(T)}) was found.");

            return (T)cachedElements[0];
        }

        protected override void Initialization()
        {
            _cachedUIElements.AddRange(GetComponentsInChildren<UIElementBase>(true));
            
            foreach (var element in _cachedUIElements.OrderByDescending(x => x.gameObject.GetLayer()))
            {
                InitializeElement(element);
            }
        }

        private void InitializeElement(UIElementBase elementBase)
        {
            try
            {
                AutoSetup.Setup(elementBase);
                elementBase.Initialization();
            }
            catch (Exception e)
            {
                Debug.LogException(new UIElementInitializationException(elementBase, e), elementBase);
            }
        }

        private bool IsValid(UIElementBase element, UIElementOption option)
        {
            if (option == UIElementOption.OnlyShowedWithoutPermanent)
            {
                return element.IsShow && !element.IsPermanent;
            }

            return option == UIElementOption.None || element.IsShow == (option == UIElementOption.OnlyShowed);
        }
    }
}