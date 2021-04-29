using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePhysPickup : MonoBehaviour, InteractableBasic
{
    public ColorAndText LookedAtInfo => new ColorAndText { text = pickedUp ? "Let go" : "Pick up", color = Color.red };
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
