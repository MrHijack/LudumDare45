using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemCollection", menuName = "ScriptableObjets/Item Collection")]
public class ItemCollection : ScriptableObject
{
    public ItemAsset[] Items;
}
