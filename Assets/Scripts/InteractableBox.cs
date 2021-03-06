using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableBasic
{
    abstract void Trigger();
    public ColorAndText LookedAtInfo { get; }
    abstract PickupMode GetPickupMode();
}

public enum PickupMode
{
    None,
    Hold,
    LookAt,
    Weapon
}

public struct ColorAndText {
    public string text;
    public Color color;
}

public class InteractableBox : MonoBehaviour, IInteractableBasic, IDamagable
{
    [SerializeField] string lookAtString;
    [SerializeField] float rotationTime = 1.0f;
    [SerializeField] float rotationDegrees = 90;

    bool interactable = true;
    float rotationAngleStart = 0;
    float rotationAngleEnd = 0;

    float lerp = 0;

    public PickupMode pickupMode;

    ColorAndText IInteractableBasic.LookedAtInfo { get => new ColorAndText { color = Color.white, text = lookAtString }; }

    public PickupMode GetPickupMode()
    {
        return pickupMode;
    }

    void IInteractableBasic.Trigger()
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

    public void Damage(float damage, Vector3 position, Vector3 force)
    {
        if (interactable)
        {
            lerp = 0;
            interactable = false;
            rotationAngleStart = transform.rotation.eulerAngles.y;
            rotationAngleEnd = rotationAngleStart + rotationDegrees;
        }
    }
}
