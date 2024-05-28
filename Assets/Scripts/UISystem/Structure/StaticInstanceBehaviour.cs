using UnityEngine;

/// <summary>
/// Gives access to instance of T inherit from MonoBehaviour.
/// If instance of T object wasn't be founded, throws exception.
/// </summary>
public abstract class StaticBehaviour<T> : MonoBehaviour where T : StaticBehaviour<T>
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = SetInstanceOrThrow();
                _instance.Initialization();
            }

            return _instance;
        }
    }

    private static T _instance = null;

    private static T SetInstanceOrThrow()
    {
        T currentInstace = FindObjectOfType<T>();

        if (currentInstace == null)
        {
            throw new System.Exception($"Instance of {typeof(T)} object wasn't found.");
        }

        return currentInstace;
    }
    
    protected virtual void Initialization() { }
}

/// <summary>
/// Gives access to instance of T inherit from MonoBehaviour.
/// If instance of T object wasn't be founded, creates one.
/// </summary>
public abstract class StaticInstanceBehaviour<T> : MonoBehaviour where T : StaticInstanceBehaviour<T>
{
    public static T Instance
    {
        get
        {
            CrateInstanceIfNotCreated();

            return _instance;
        }
    }

    private static T _instance = null;

    public static void CrateInstanceIfNotCreated()
    {
        if (_instance == null)
        {
            _instance = GetOrCreateInstance();
            _instance.Initialization();
        }
    }

    private static T GetOrCreateInstance()
    {
        T currentInstace = FindObjectOfType<T>();

        if (currentInstace == null)
        {
            GameObject instanceContainer = new GameObject($"{typeof(T)} Instance");

            currentInstace = instanceContainer.AddComponent<T>();
        }

        return currentInstace;
    }
    
    protected virtual void Initialization() { }
}