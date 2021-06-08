using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] Transform rayOrigin;
    [SerializeField] LayerMask layerMask;
    Quaternion targetRotation = Quaternion.identity;
    [SerializeField] float rotationSpeedMultiplier = 5.0f;
    Light light;
    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(new Ray(rayOrigin.position, rayOrigin.forward), out RaycastHit rHit, 5, layerMask))
        {
            targetRotation.SetLookRotation((rHit.point - transform.position).normalized);
        }
        else
        {
            targetRotation = rayOrigin.rotation;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeedMultiplier);
    }

    public void SetState(bool active)
    {
        light.enabled = active;
        audio.Play();
    }

    public void InvertState()
    {
        light.enabled = !light.enabled;
        audio.Play();
    }
}
