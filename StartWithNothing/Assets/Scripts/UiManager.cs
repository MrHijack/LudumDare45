using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class UiManager : MonoBehaviour
{
    public Inventory Inventory;
    public FirstPersonController Player;
    public CraftingInfo CraftingInfo;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Inventory.Busy)
        {
            if (CraftingInfo.Open)
            {
                CraftingInfo.ForceClose();
            }
            Inventory.Toggle();
            Player.ToggleMouseLock();
        }
    }

    public void ToggleCraftingInfo()
    {
        CraftingInfo.Toggle();
    }
}
