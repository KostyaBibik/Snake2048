using System;
using JetBrains.Annotations;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class AutoSetupFieldAttribute : Attribute
{
    [CanBeNull] public string Name { get; }
    [CanBeNull] public MonoBehaviour Target { get; }

    public AutoSetupFieldAttribute()
    {
        
    }
    
    public AutoSetupFieldAttribute(string name)
    {
        Name = name;
    }
    
    public AutoSetupFieldAttribute(MonoBehaviour target)
    {
        Target = target;
    }
}