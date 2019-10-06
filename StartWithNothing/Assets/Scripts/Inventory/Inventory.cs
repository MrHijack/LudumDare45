using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const string OPEN_ID = "Open";

    [SerializeField] private Animator animator;
    [SerializeField] private Transform toolbarParent;
    [SerializeField] private Transform backpackParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] [Range(1, 4)] private int toolbarCount = 4;
    [SerializeField] [Range(1, 24)] private int backpackCount = 24;
    [SerializeField] float scrollDelay = 0.1f;

    private readonly List<Slot> toolbar = new List<Slot>();
    private readonly List<Slot> backpack = new List<Slot>();
    private int _currentToolbar;
    private float scrollTimer = 0;

    private int CurrentToolbar
    {
        get => _currentToolbar;
        set => SetToolbarItem(value);
    }

    public bool Open { get; set; } = false;
    public bool Busy;

    private void Start()
    {
        InitializeSlots();
        CurrentToolbar = 0;
    }

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scrollTimer > 0)
        {
            scrollTimer -= Time.deltaTime;
        }
        if (!Open && scroll > 0f && scrollTimer <= 0)
        {
            scrollTimer = scrollDelay;
            CurrentToolbar-=1;
        }
        if (!Open && scroll < 0f && scrollTimer <= 0)
        {
            scrollTimer = scrollDelay;
            CurrentToolbar+=1;
        }
    }

    private void InitializeSlots()
    {
        for (int index = 0; index < toolbarCount; index++)
        {
            var slotObj = Instantiate(slotPrefab, toolbarParent);
            var slot = slotObj.GetComponent<Slot>();
            toolbar.Add(slot);
        }
        for (int index = 0; index < backpackCount; index++)
        {
            var slotObj = Instantiate(slotPrefab, backpackParent);
            var slot = slotObj.GetComponent<Slot>();
            backpack.Add(slot);
        }
    }

    private void SetToolbarItem(int slot)
    {
        toolbar[_currentToolbar].Selected = false;
        var nextToolbar = slot % toolbarCount;
        _currentToolbar = nextToolbar >= 0 ? nextToolbar : toolbarCount + nextToolbar;
        toolbar[_currentToolbar].Selected = true;
    }

    public void Toggle()
    {
        if (!Busy)
        {
            if (Open)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    private void OpenInventory()
    {
        Open = true;
        animator.SetBool(OPEN_ID, true);
        toolbar[_currentToolbar].Selected = false;
    }

    private void CloseInventory()
    {
        Open = false;
        animator.SetBool(OPEN_ID, false);
        toolbar[_currentToolbar].Selected = true;
    }

    public int TryPickupItem(Item item)
    {
        var possibleSlots = backpack.Where(entry => (entry.Content?.Id ?? item.Id) == item.Id && entry.Quantity < item.StackSize).OrderByDescending(entry => entry.Content != null);
        var quantityToPickUp = item.Quantity;
        foreach (var slot in possibleSlots)
        {
            var slotCapacity = (slot.Content?.StackSize ?? item.StackSize) - slot.Quantity;
            var quantityToAssign = slotCapacity > item.Quantity ? item.Quantity : slotCapacity;
            quantityToPickUp -= quantityToAssign;

            if (slot.Content == null)
            {
                var pickedItem = item.Clone();
                pickedItem.Quantity = quantityToAssign;
                slot.Content = pickedItem;
            }
            else
            {
                slot.Quantity += quantityToAssign;
            }
            item.Quantity -= quantityToAssign;

            if (quantityToPickUp == 0)
            {
                break;
            }
        }
        return quantityToPickUp;
    }
}
