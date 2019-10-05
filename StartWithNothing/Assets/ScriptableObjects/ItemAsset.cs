using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName ="ScriptableObjets/Item")]
public class ItemAsset : ScriptableObject
{
    public string Name;
    public int Id;
    public int StackSize;
    public Sprite Icon;
    public GameObject Prefab;
}
