using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePaper : MonoBehaviour, IInteractableBasic
{
    ColorAndText IInteractableBasic.LookedAtInfo => new ColorAndText {color = Color.black, text = pickedUp ? "" : "Read"};
    bool pickedUp = false;
    Quaternion rotation;
    Vector3 position;
    PickupMode IInteractableBasic.GetPickupMode()
    {
        return PickupMode.LookAt;
    }

    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.rotation;
        position = transform.position;
    }

    void IInteractableBasic.Trigger()
    {
        pickedUp = !pickedUp;
        if (!pickedUp) ResetPosition();
    }

    void ResetPosition()
    {
        transform.rotation = rotation;
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
