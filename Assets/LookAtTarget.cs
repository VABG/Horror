using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float maxLookRotation = 45;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LookAtObject();
    }

    void LookAtObject()
    {
        // Make lookObject manually rotated
        Vector3 dir = target.transform.position - transform.position;

        // Need to remove Y axis or else Signed Angle doesn't work correctly.
        dir = new Vector3(dir.x, 0, dir.z).normalized;
        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        // Smooth movement
        float rotation = angle * Time.deltaTime * 3.0f;

        // Check if further than angle towards target.
        if (Mathf.Abs(rotation) > Mathf.Abs(angle)) rotation = angle;

        transform.Rotate(transform.up, rotation);

        Vector3 resultRotation = transform.localEulerAngles;

        if (resultRotation.y > 180 && resultRotation.y < 360 - maxLookRotation)
        {
            resultRotation.y = 360 - maxLookRotation;
        }
        else if (resultRotation.y < 180 && resultRotation.y > maxLookRotation)
        {
            resultRotation.y = maxLookRotation;
        }
        transform.localRotation = Quaternion.Euler(resultRotation);
    }
}
