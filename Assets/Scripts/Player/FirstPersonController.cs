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
    [SerializeField] LayerMask rayCastMask;

    Rigidbody rb;
    PlayerUI ui;

    [SerializeField] GameObject flashlight;

    [SerializeField] float holdStrengthMultiplier = 50;
    [SerializeField] Transform lookAtTransform;
    [SerializeField] Transform holdTransform;
    [SerializeField] WeaponHand weaponHand;

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
            InputWeapon();
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
            if ((heldObject.transform.position-transform.position).magnitude > rayCastReach + .1f)
            {
                DropHeld();
                return;
            }
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
                // Flip back (Now has more stable behaviour!)
                dist = 1 - dist;
            }
            heldObject.AddForce(offset * holdStrengthMultiplier * dist * Time.fixedDeltaTime);

            if (dist >= 1) dist = .95f;
            heldObject.velocity *= dist;
            heldObject.angularVelocity *= dist;
        }
    }

    void InputWeapon()
    {
        if (Input.GetKeyDown(KeyCode.G)) weaponHand.DropWeapon();
        if (Input.GetMouseButtonDown(0)) weaponHand.Attack();
    }

    float LimitMouseXRotation(float angle)
    {
        if (angle > 89 && angle < 180) return 89;
        if (angle < 271 && angle > 180) return 271;
        return angle;
    }

    void InputMouseView()
    {
        // Rotate camera 
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector3 rotation = cam.transform.rotation.eulerAngles;
        cam.transform.rotation = Quaternion.Euler(LimitMouseXRotation(rotation.x - mouseInput.y), rotation.y + mouseInput.x, rotation.z);
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
                DropHeld();
            }
            return;
        }

        Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, rayCastReach, rayCastMask);
        if (hit.collider)
        {
            IInteractableBasic interact = hit.collider.GetComponent<IInteractableBasic>();
            if (interact != null)
            {
                if (interactBtnPressed)
                {
                    interact.Trigger();
                    PickupMode p = interact.GetPickupMode();
                    switch (p)
                    {
                        case PickupMode.Hold:
                            heldObject = hit.collider.attachedRigidbody;
                            heldInteractable = interact;
                            break;
                        case PickupMode.LookAt:
                            PickUpLookAt(hit, interact);
                            break;
                        case PickupMode.Weapon:
                            weaponHand.SetWeapon(hit.collider);
                            break;
                    }
                }
                ColorAndText c = interact.LookedAtInfo;
                ui.SetCenterText(c.text, c.color);
            }
        }
    }

    private void DropHeld()
    {
        if (heldInteractable != null) heldInteractable.Trigger();
        
        heldObject = null;
        heldInteractable = null;
        locked = false;

        if (heldTransform != null)
        {
            heldTransform.SetParent(null);
            heldTransform.gameObject.layer = 0;
            heldTransform = null;
        }
    }

    private void PickUpLookAt(RaycastHit hit, IInteractableBasic interact)
    {
        heldTransform = hit.collider.transform;
        heldInteractable = interact;
        hit.collider.transform.SetParent(lookAtTransform);
        hit.collider.transform.localPosition = Vector3.zero;
        hit.collider.transform.localRotation = Quaternion.identity;
        hit.collider.gameObject.layer = 6;
        locked = true;
        moveInput = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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
