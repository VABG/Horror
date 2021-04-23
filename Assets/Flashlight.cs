using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] Transform rayOrigin;
    Quaternion targetRotation = Quaternion.identity;
    [SerializeField] float rotationSpeedMultiplier = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(new Ray(rayOrigin.position, rayOrigin.forward), out RaycastHit rHit, 5))
        {
            targetRotation.SetLookRotation((rHit.point - transform.position).normalized);
        }
        else
        {
            targetRotation = rayOrigin.rotation;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 4.0f);
    }
}
