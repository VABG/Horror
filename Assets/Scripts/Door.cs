using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, InteractableBasic
{
    [SerializeField] Transform hinge;

    [SerializeField] string openString = "Open Door";
    [SerializeField] string closeString = "Close Door";
    [SerializeField] float openAngle = 90;
    [SerializeField] float closedAngle = 0;
    [SerializeField] float openCloseTime = 1.0f;
    [SerializeField] bool canInterrupt = false;
    bool locked = false;
    float lerp = 0;
    bool moving = false;
    bool open = false;
    public ColorAndText LookedAtInfo { get => new ColorAndText { text = open ? closeString : openString, color = Color.white }; set => throw new System.NotImplementedException(); }

    public void Trigger()
    {
        if (canInterrupt || !moving)
        {
            open = !open;
            moving = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (openCloseTime <= 0) openCloseTime = .1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (open) lerp += Time.deltaTime / openCloseTime;
            else lerp -= Time.deltaTime / openCloseTime;

            if (lerp < 0)
            {            
                moving = false;
                lerp = 0;
            }
            else if (lerp >= 1)
            {
                moving = false;
                lerp = 1;
            }

            Vector3 rot = hinge.transform.localRotation.eulerAngles;
            hinge.transform.localRotation = Quaternion.Euler(rot.x, Mathf.SmoothStep(closedAngle, openAngle, lerp), rot.z);
        }
    }
}
