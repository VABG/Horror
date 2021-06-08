using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter;
    bool triggered;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            triggered = true;
            onEnter.Invoke();
        }
    }
}
