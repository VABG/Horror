using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadBtn : MonoBehaviour, IInteractableBasic
{
    [SerializeField] KeyPad keyPadParent;
    public int number;
    public ColorAndText LookedAtInfo { get => new ColorAndText { text = number.ToString(), color = Color.green }; set => LookedAtInfo = value; }
    public void Trigger()
    {
        if (keyPadParent.status == KeyPadStatus.Off) return;
        keyPadParent.ReceiveInput(number);
        // Add animation?
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PickupMode GetPickupMode()
    {
        return PickupMode.Hold;
    }
}
