using System.Linq;
using UnityEngine;

public static class TransformExtensions
{
    public static T GetChild<T>(this Transform root, int index = 0) where T : Transform
    {
        if (index < 0 || index >= root.childCount)
        {
            return null;
        }

        return root.GetChild(index) as T;
    }

    public static T[] GetChilds<T>(this Transform root, int start = 0, int count = int.MaxValue) where T : Transform
    {
        if (root.childCount == 0)
        {
            return Enumerable.Empty<T>().ToArray();
        }

        start = Mathf.Clamp(start, 0, root.childCount - 1);
        count = Mathf.Clamp(count, 0, root.childCount - start);

        var childs = Enumerable.Range(start, root.childCount)
            .Select(i => root.GetChild(i) as T)
            .ToArray();

        return childs;
    }
}