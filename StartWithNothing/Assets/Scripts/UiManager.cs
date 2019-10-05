using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class UiManager : MonoBehaviour
{
    public Inventory Inventory;
    public FirstPersonController Player;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Inventory.Busy)
        {
            Inventory.Toggle();
            Player.ToggleMouseLock();
        }
    }
}
