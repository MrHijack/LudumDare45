using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image slotBorder;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text quantityText;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;
    [SerializeField] private GameObject gameObject;

    private Item? _content;
    private int _quantity;
    private bool _selected;

    public Item? Content
    {
        get => _content;
        set => SetContent(value);
    }

    public int Quantity
    {
        get => _quantity;
        set => SetQuantity(value);
    }

    public bool Selected
    {
        get => _selected;
        set => SetSelected(value);
    }

    private void SetContent(Item? content)
    {
        _content = content;
        itemImage.enabled = _content != null;
        quantityText.enabled = _content != null;
        itemImage.sprite = ItemResolver.ResolveSprite(content);
        if (_content.HasValue)
        {
            Quantity = _content.Value.Quantity;
        }
    }

    private void SetQuantity(int quantity)
    {
        _quantity = quantity;
        if (_quantity < 1)
        {
            Content = Item.Empty;
        }
        quantityText.text = _quantity.ToString();
    }

    private void SetSelected(bool selected)
    {
        _selected = selected;
        slotBorder.color = _selected ? selectedColor : deselectedColor;
    }

    public void ToggleSelected()
    {
        Selected = !_selected;
    }
}
