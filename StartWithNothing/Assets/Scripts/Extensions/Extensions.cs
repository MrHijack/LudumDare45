using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static IEnumerable<GameObject> GetChildren(this GameObject parent)
    {
        var parentTransform = parent.GetComponent<Transform>();
        List<GameObject> children = new List<GameObject>();

        for (int index = 0; index < parentTransform.childCount; index++)
        {
            var child = parentTransform.GetChild(index).gameObject;
            children.Add(child);
        }
        return children;
    }

    public static void DestroyChildren(this GameObject parent)
    {
        var children = parent.GetChildren();
        children.ForEach(GameObject.Destroy);
        children = null;
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
        {
            action?.Invoke(item);
        }
    }
}
