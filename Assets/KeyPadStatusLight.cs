using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadStatusLight : MonoBehaviour
{

    [SerializeField] Material wrongMaterial;
    [SerializeField] Material rightMaterial;
    [SerializeField] Material idleMaterial;
    [SerializeField] Material offMaterial;

    bool lightChanged = false;
    float lightTimer = 0;
    bool active = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    void SetMaterial(Material m)
    {
        GetComponent<MeshRenderer>().material = m;
    }

    // Update is called once per frame
    void Update()
    {
        if (lightChanged)
        {
            lightTimer -= Time.deltaTime;
            if (lightTimer <= 0)
            {
                lightChanged = false;
                SetIdle();
            }
        }
    }

    public void SetIdle()
    {
        if (active)
        {
            SetMaterial(idleMaterial);
            lightChanged = false;
        }
    }

    public void SetOff()
    {
        if (active)
        {
            SetMaterial(offMaterial);
            active = false;
            lightChanged = false;
        }
    }

    public void SetError(float time)
    {
        if (active)
        {
            lightChanged = true;
            SetMaterial(wrongMaterial);
            lightTimer = time;
        }
    }

    public void SetCorrect(float time)
    {
        if (active)
        {
            SetMaterial(rightMaterial);
            lightTimer = time;
            lightChanged = true;
        }
    }
}
