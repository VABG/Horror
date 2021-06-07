using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractablePaper : MonoBehaviour, IInteractableBasic
{
    ColorAndText IInteractableBasic.LookedAtInfo => new ColorAndText {color = Color.black, text = pickedUp ? "" : "Read"};
    bool pickedUp = false;
    [SerializeField] Text text;
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

    public void SetText(string text)
    {
        this.text.text = text;
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
