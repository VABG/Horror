using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePhysPickup : MonoBehaviour, IInteractableBasic, IDamagable
{
    [SerializeField] string pickUpText = "Pick up";
    [SerializeField] public Transform grabTransform;
    public ColorAndText LookedAtInfo => new ColorAndText { text = pickedUp ? "": pickUpText, color = Color.red };
    [SerializeField] PickupMode pickupMode;
    Rigidbody rb;

    bool pickedUp = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public PickupMode GetPickupMode()
    {
        return pickupMode;
    }

    public void Trigger()
    {
        pickedUp = !pickedUp;
    }

    public void Damage(float damage, Vector3 position, Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
    }
}
