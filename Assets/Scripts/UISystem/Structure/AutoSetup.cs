using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class AutoSetup
{
    private const int MaximalDepth = 8;
    
    public static void Setup(MonoBehaviour defaultTarget, bool unsafeMode = false)
    {
        if (defaultTarget == null)
        {
            throw new NullReferenceException();
        }

        var depth = 0;
        var type = defaultTarget.GetType();
        
        while (depth < MaximalDepth && type != typeof(MonoBehaviour))
        {
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.CustomAttributes
                .Any(x => x.AttributeType == typeof(AutoSetupFieldAttribute)));

            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<AutoSetupFieldAttribute>();
                var fieldType = field.FieldType;
                var target = attribute.Target != null ? attribute.Target : defaultTarget;
                var name = attribute.Name;

                if (string.IsNullOrEmpty(name))
                {
                    name = FormatFieldName(field.Name);
                }
                    
                var component = target.gameObject.GetElement(name, fieldType);

                if (!unsafeMode && component == null)
                {
                    throw new Exception($"Не удалось получить компонент {attribute.Name}:{fieldType}");
                }
            
                field.SetValue(defaultTarget, component);
                depth++;
            }

            type = type.BaseType;
        }
    }

    private static string FormatFieldName(string name)
    {
        if (name[0] == '_')
        {
            return name.Substring(1);
        }

        return name;
    }
}
