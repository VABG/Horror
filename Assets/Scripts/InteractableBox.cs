using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SimpleInteractable
{
    abstract void Trigger();
    public ColorAndText LookedAtInfo { get; set; }
}

public struct ColorAndText {
    public string text;
    public Color color;
}

public class InteractableBox : MonoBehaviour, SimpleInteractable
{
    [SerializeField] string lookAtString;
    bool interactable = true;
    [SerializeField] float rotationTime = 1.0f;
    [SerializeField] float rotationDegrees = 90;

    float currentRotation = 0;
    float targetRotation = 0;
    float lerp = 0;
    ColorAndText SimpleInteractable.LookedAtInfo { get => new ColorAndText {color = Color.white, text = lookAtString }; set => Debug.Log("No behaviour defined"); }

    void SimpleInteractable.Trigger()
    {
        if (interactable)
        {
            lerp = 0;
            interactable = false;
            currentRotation = transform.rotation.eulerAngles.y;
            targetRotation = currentRotation + 90;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactable)
        {
            lerp += Time.deltaTime;            
            Vector3 rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(rotation.x, Mathf.SmoothStep(currentRotation, targetRotation, lerp), rotation.z));
            if (lerp >= 1)
            {
                transform.rotation = Quaternion.Euler(new Vector3(rotation.x, targetRotation, rotation.z));
                interactable = true;
            }
        }
    }
}
