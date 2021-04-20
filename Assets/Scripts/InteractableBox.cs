using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InteractableBasic
{
    abstract void Trigger();
    public ColorAndText LookedAtInfo { get; set; }
}

public struct ColorAndText {
    public string text;
    public Color color;
}

public class InteractableBox : MonoBehaviour, InteractableBasic
{
    [SerializeField] string lookAtString;
    [SerializeField] float rotationTime = 1.0f;
    [SerializeField] float rotationDegrees = 90;

    bool interactable = true;
    float rotationAngleStart = 0;
    float rotationAngleEnd = 0;

    float lerp = 0;
    ColorAndText InteractableBasic.LookedAtInfo { get => new ColorAndText {color = Color.white, text = lookAtString }; set => Debug.Log("No behaviour defined"); }

    void InteractableBasic.Trigger()
    {
        if (interactable)
        {
            lerp = 0;
            interactable = false;
            rotationAngleStart = transform.rotation.eulerAngles.y;
            rotationAngleEnd = rotationAngleStart + rotationDegrees;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactable)
        {
            lerp += Time.deltaTime / rotationTime;
            Vector3 rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(rotation.x, Mathf.SmoothStep(rotationAngleStart, rotationAngleEnd, lerp), rotation.z));
            if (lerp >= 1)
            {
                transform.rotation = Quaternion.Euler(new Vector3(rotation.x, rotationAngleEnd, rotation.z));
                interactable = true;
            }
        }
    }
}
