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
    [SerializeField] Transform hand;
    [SerializeField] GameObject weapon;


    // Held objects
    Rigidbody heldObject;
    IInteractableBasic heldInteractable;
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
            InputMouseView();
            InputDropWeapon();
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

            //Normalized to max of .5
            if (dist > .5) dist = 1;
            else
            {
                // Normalize .5 and under
                dist *= 2;
                // Flip value
                dist = 1 - dist;
                // Bend curve
                dist = Mathf.Pow(dist, 3);
                // Flip back (Now has better behaviour!)
                dist = 1 - dist;
            }
            heldObject.AddForce(offset * holdStrengthMultiplier * dist * Time.fixedDeltaTime);

            if (dist >= 1) dist = .95f;
            heldObject.velocity *= dist;
            heldObject.angularVelocity *= dist;
        }
    }

    void InputDropWeapon()
    {
        if (Input.GetKeyDown(KeyCode.G) && weapon != null)
        {
            Collider c = weapon.GetComponentInChildren<Collider>();
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.useGravity = true;
            c.attachedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Transform tPos = c.GetComponent<InteractablePhysPickup>().grabTransform;
            tPos.SetParent(null);

            c.gameObject.layer = 0;
            weapon.layer = 0;
            weapon = null;
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
            IInteractableBasic interact = hit.collider.GetComponent<IInteractableBasic>();
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
                    else if (p == PickupMode.Weapon)
                    {
                        hit.collider.attachedRigidbody.isKinematic = true;
                        hit.collider.attachedRigidbody.useGravity = false;
                        Transform tPos =  hit.collider.GetComponent<InteractablePhysPickup>().grabTransform;
                        tPos.SetParent(hand);
                        tPos.position = hand.position;
                        tPos.rotation = hand.rotation;
                        weapon = tPos.gameObject;
                        // Set layer to player
                        hit.collider.gameObject.layer = 6;
                        hit.collider.attachedRigidbody.interpolation = RigidbodyInterpolation.None;
                        weapon.layer = 6;
                    }
                }
                ColorAndText c = interact.LookedAtInfo;
                ui.SetCenterText(c.text, c.color);
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateCarriedObject();

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
