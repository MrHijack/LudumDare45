using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    private Camera playerCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Sprite crosshair;
    [SerializeField] private Sprite hand;
    [SerializeField] private Image centerImage;
    [SerializeField] private float pickupRange = 1.5f;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float raycastTimer = .05f;

    private bool marked = false;
    private float timer = 0;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        timer += Time.deltaTime;
        var pickUp = Input.GetKeyDown(KeyCode.E);
        RaycastHit hit = new RaycastHit();
        bool result = false;
        if (timer >= raycastTimer || pickUp)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            result = Physics.Raycast(ray, out hit, pickupRange, layerMask.value);
            if (result && !marked)
            {
                marked = true;
                centerImage.sprite = hand;
            }
            else if (!result && marked)
            {
                marked = false;
                centerImage.sprite = crosshair;
            }
        }
        if (pickUp)
        {
            if (result)
            {
                var pickableResource = hit.collider.gameObject.GetComponent<PickableResource>();
                var item = pickableResource.Resource;
                var remainingQuanity = inventory.TryPickupItem(item);
                if (remainingQuanity == 0)
                {
                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    item.Quantity = remainingQuanity;
                    pickableResource.Resource = item;
                }
            }
        }
    }
}
