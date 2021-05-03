using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerUI))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float moveAccelerationMultiplier = 5;
    [SerializeField] float sprintAccelerationMultiplier = 50;

    [Range(.1f, 3.0f)]
    [SerializeField] float rayCastReach = 1.0f;

    Rigidbody rb;
    PlayerUI ui;

    [SerializeField] GameObject flashlight;

    [SerializeField] float holdStrengthMultiplier = 50;
    [SerializeField] Transform lookAtTransform;
    [SerializeField] Transform holdTransform;

    Rigidbody heldObject;
    InteractableBasic heldInteractable;
    Transform heldTransform;

    Vector3 moveInput;
    Vector3 forward;
    bool sprinting = false;

    bool locked = false;
    bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ui = GetComponent<PlayerUI>();

        //Deactivate flashlight
        flashlight.SetActive(false);

        // Lock mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            UpdateCarriedObject();
            InputMouseView();

            if (!locked)
            {
                InputKeyboardMovement();
            }
            RayCastToScene();
            if (Input.GetKeyDown(KeyCode.F)) flashlight.SetActive(!flashlight.activeSelf);
        }
    }

    void UpdateCarriedObject()
    {
        if (heldObject != null)
        {
            // Move object towards position with physics
            Vector3 offset = holdTransform.position - heldObject.position;
            float dist = offset.magnitude;
            offset /= dist;
            if (dist > .5) dist = 1;
            else
            {
                dist *= 2;
                dist *= dist;
            }
            heldObject.AddForce(offset * holdStrengthMultiplier * dist);
            heldObject.velocity *= dist;
            heldObject.angularVelocity *= dist;
        }
    }

    void InputMouseView()
    {
        // Rotate camera 
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector3 rotation = cam.transform.rotation.eulerAngles;
        cam.transform.rotation = Quaternion.Euler(rotation.x - mouseInput.y, rotation.y + mouseInput.x, rotation.z);

        //Set forward direction after rotating
        forward = cam.transform.forward;
        forward = new Vector3(forward.x, 0, forward.z);
        forward.Normalize();
    }    

    void InputKeyboardMovement()
    {
        sprinting = false;
        // Reset Input
        moveInput = Vector3.zero;
        // Get Input
        if (Input.GetKey(KeyCode.W)) moveInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveInput += Vector3.back;
        if (Input.GetKey(KeyCode.D)) moveInput += Vector3.right;
        if (Input.GetKey(KeyCode.A)) moveInput += Vector3.left;
        if (Input.GetKey(KeyCode.LeftShift)) sprinting = true;
        //  Normalize
        moveInput.Normalize();
    }

    void RayCastToScene()
    {
        bool interactBtnPressed = Input.GetKeyDown(KeyCode.E);

        // Drop what's being held
        if (heldObject != null || heldInteractable != null)
        {
            if (interactBtnPressed)
            {
                heldObject = null;
                if (heldInteractable != null) heldInteractable.Trigger();
                heldInteractable = null;
                locked = false;

                if (heldTransform != null) heldTransform.SetParent(null);
                heldTransform = null;
            }
            return;
        }

        Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, rayCastReach);
        if (hit.collider)
        {
            InteractableBasic interact = hit.collider.GetComponent<InteractableBasic>();
            if (interact != null)
            {
                if (interactBtnPressed)
                {
                    interact.Trigger();
                    PickupMode p = interact.GetPickupMode();
                    if (p == PickupMode.Hold)
                    {
                        heldObject = hit.collider.attachedRigidbody;
                        heldInteractable = interact;
                    }
                    else if (p == PickupMode.LookAt)
                    {
                        heldTransform = hit.collider.transform;
                        heldInteractable = interact;
                        hit.collider.transform.SetParent(lookAtTransform);
                        hit.collider.transform.localPosition = Vector3.zero;
                        hit.collider.transform.localRotation = Quaternion.identity;

                        locked = true;
                        moveInput = Vector3.zero;
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
                ColorAndText c = interact.LookedAtInfo;
                ui.SetCenterText(c.text, c.color);
            }
        }
    }

    private void FixedUpdate()
    {
        // Update movement in fixed update for stability
        float multiplier = sprinting ? sprintAccelerationMultiplier : moveAccelerationMultiplier;
        rb.AddForce(forward * moveInput.z * multiplier, ForceMode.Acceleration);
        rb.AddForce(cam.transform.right * moveInput.x * multiplier, ForceMode.Acceleration);
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }    

}
