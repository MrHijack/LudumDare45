using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const string OPEN_ID = "Open";

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject toolbar;
    [SerializeField] private GameObject inventory;

    public bool Open { get; set; } = false;
    public bool Busy;

    private void Start()
    {

    }

    private void Update()
    {

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
    }

    private void CloseInventory()
    {
        Open = false;
        animator.SetBool(OPEN_ID, false);
    }
}
