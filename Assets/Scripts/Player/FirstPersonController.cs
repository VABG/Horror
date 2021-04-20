using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerUI))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float moveAccelerationMultiplier = 5;
    [Range(.1f, 3.0f)]
    [SerializeField] float rayCastReach = 1.0f;

    Rigidbody rb;
    PlayerUI ui;

    [SerializeField] GameObject flashlight;

    Vector3 moveInput;
    Vector3 forward;

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

        rb.sleepThreshold = 0;
    }

    // Update is called once per frame
    void Update()
    {
        InputMouseView();
        InputKeyboardMovement();
        RayCastToScene();
        if (Input.GetKeyDown(KeyCode.F)) flashlight.SetActive(!flashlight.activeSelf);
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
        // Reset Input
        moveInput = Vector3.zero;
        // Get Input
        if (Input.GetKey(KeyCode.W)) moveInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveInput += Vector3.back;
        if (Input.GetKey(KeyCode.D)) moveInput += Vector3.right;
        if (Input.GetKey(KeyCode.A)) moveInput += Vector3.left;
        //  Normalize
        moveInput.Normalize();
    }

    void RayCastToScene()
    {
        Ray r = new Ray();
        Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, rayCastReach);
        if (hit.collider)
        {
            SimpleInteractable interact = hit.collider.GetComponent<SimpleInteractable>();
            if (interact != null)
            {
                if (Input.GetKeyDown(KeyCode.E)) interact.Trigger();
                ColorAndText c = interact.LookedAtInfo;
                ui.SetCenterText(c.text, c.color);
            }
        }
    }

    private void FixedUpdate()
    {
        // Update movement in fixed update for stability
        rb.AddForce(forward * moveInput.z * moveAccelerationMultiplier, ForceMode.Acceleration);
        rb.AddForce(cam.transform.right * moveInput.x * moveAccelerationMultiplier, ForceMode.Acceleration);
    }
}
