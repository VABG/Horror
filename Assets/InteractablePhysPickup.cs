using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePhysPickup : MonoBehaviour, IInteractableBasic, IDamagable
{
    [SerializeField] string pickUpText = "Pick up";
    [SerializeField] string releaseText = "Let go";
    [SerializeField] public Transform grabTransform;
    public ColorAndText LookedAtInfo => new ColorAndText { text = pickedUp ? releaseText: pickUpText, color = Color.red };
    [SerializeField] PickupMode pickupMode;
    bool pickedUp = false;
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
        Debug.Log("Hey!");
    }
}
