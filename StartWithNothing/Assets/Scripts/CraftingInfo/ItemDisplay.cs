﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [SerializeField] private Image itemDisplay;
    [SerializeField] private Text quantityText;

    private Item _item;

    public Item Item
    {
        get => _item;
        set => SetItem(value);
    }

    public Color TextColor
    {
        get => quantityText.color;
        set => SetColor(value);
    }

    private void SetItem(Item item)
    {
        _item = item;
        quantityText.text = _item.Quantity.ToString();
        itemDisplay.sprite = ItemResolver.ResolveSprite(_item);
    }

    private void SetColor(Color color)
    {
        quantityText.color = color;
    }
}
