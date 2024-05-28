using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public static class GameObjectExtensions
{
    public static void ClearMonoBehaviours(this GameObject gameObject)
    {
        var components = gameObject.GetComponents<MonoBehaviour>();

        foreach (var component in components)
        {
            Object.Destroy(component);
        }
    }

    public static void SetLayer(this GameObject gameObject, int layerMask)
    {
        gameObject.layer = layerMask;
        
        foreach (Transform child in gameObject.transform)
        {
            SetLayer(child.gameObject, layerMask);
        }    
    }

    public static int GetLayer(this GameObject gameObject)
    {
        if (gameObject == null)
        {
            throw new ArgumentNullException(nameof(gameObject));
        }
        
        var layer = 0;
        var target = gameObject.transform;
        
        while (target.parent != null)
        {
            target = target.parent;
            layer++;
        }

        return layer;
    }
    
    public static T AddIfNotComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    /// <summary>
    /// Возвращает элемент с именеи формата $name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tag"></param>
    /// <param name="type">Тип искомого компонента</param>
    /// <param name="parent"></param>
    public static Component GetElement(this GameObject parent, string tag, Type type)
    {
        foreach (var component in parent.GetComponentsInChildren(type, true))
        {
            if (component)
            {
                string name = component.gameObject.name;

                name = name.Replace($"${tag}", string.Empty);
                name = name.Trim();

                if (string.IsNullOrEmpty(name))
                    return component;
            }
        }

        return default;
    }
    
    /// <summary>
    /// Возвращает элемент с именеи формата $name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tag"></param>
    /// <param name="parent"></param>
    public static T GetElement<T>(string tag, GameObject parent) where T : Component
    {
        foreach (T t_component in parent.GetComponentsInChildren<T>(true))
        {
            Component component = t_component as Component;

            if (component)
            {
                string name = component.gameObject.name;

                name = name.Replace($"${tag}", string.Empty);
                name = name.Trim();

                if (string.IsNullOrEmpty(name))
                    return t_component;
            }
        }

        return default;
    }

    /// <summary>
    /// Возвращает элементы с именеи формата $name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tag"></param>
    /// <param name="parent"></param>
    public static T[] GetElements<T>(this GameObject parent, string tag) where T : Component
    {
        List<T> result = new List<T>();

        foreach (T t_component in parent.GetComponentsInChildren<T>(true))
        {
            Component component = t_component as Component;

            if (component)
            {
                string name = component.gameObject.name;

                name = name.Replace($"${tag}", string.Empty);
                name = name.Trim();

                if (string.IsNullOrEmpty(name))
                    result.Add((T)component);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Возвращает элемент с именеи формата $name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tag"></param>
    /// <param name="parent"></param>
    public static T GetElement<T>(this GameObject parent, string tag) where T : Component
    {
        return GetElement<T>(tag, parent);
    }
}