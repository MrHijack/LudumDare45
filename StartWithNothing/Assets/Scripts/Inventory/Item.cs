using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Item
{
    public int Id;
    public int Quantity;
    public int StackSize;

    public Item(int id, int quantity, int stackSize)
    {
        Id = id;
        Quantity = quantity;
        StackSize = stackSize;
    }

    public Item Clone()
    {
        return new Item(Id, Quantity, StackSize);
    }

    public static Item Empty => new Item(-1, -1, -1);

    public static bool operator ==(Item left, Item right)
    {
        return left.Id == right.Id && left.Quantity == right.Quantity && left.StackSize == right.StackSize;
    }

    public static bool operator !=(Item left, Item right)
    {
        return !(left == right);
    }
}
