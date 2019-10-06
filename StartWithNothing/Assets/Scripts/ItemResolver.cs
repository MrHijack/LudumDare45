using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemResolver : MonoBehaviour
{
    public static ItemResolver Instance { get; private set; }

    public ItemCollection ItemCollection;

    void Start()
    {
        Instance = this;
    }

    public static Sprite ResolveSprite(Item? item)
    {
        return Instance.ResolveSpriteInternal(item);
    }

    private Sprite ResolveSpriteInternal(Item? item)
    {
        if (item != null)
        {
            return GetItemAsset(item.Value)?.Icon;
        }
        else
        {
            return null;
        }
    }

    public static GameObject ResolvePrefab(Item? item)
    {
        return Instance.ResolvePrefabInternal(item);
    }

    private GameObject ResolvePrefabInternal(Item? item)
    {
        if (item != null)
        {
            return GetItemAsset(item.Value)?.Prefab;
        }
        else
        {
            return null;
        }
    }

    private ItemAsset GetItemAsset(Item item)
    {
        return ItemCollection.Items.Where(entry => entry.Id == item.Id).FirstOrDefault();
    }

}
