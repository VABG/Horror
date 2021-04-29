using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadStatusLight : MonoBehaviour
{

    [SerializeField] Material wrongMaterial;
    [SerializeField] Material rightMaterial;
    [SerializeField] Material idleMaterial;
    [SerializeField] Material offMaterial;
    MeshRenderer mesh;

    public float timer = 0;
    bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    void SetMaterial(Material m)
    {
        mesh.material = m;
    }

    public void SetIdle()
    {
        if (active)
        {
            SetMaterial(idleMaterial);
        }
    }

    public void SetOff()
    {
        if (active)
        {
            SetMaterial(offMaterial);
            active = false;
        }
    }

    public void SetError()
    {
        if (active)
        {
            SetMaterial(wrongMaterial);
        }
    }

    public void SetCorrect()
    {
        if (active)
        {
            SetMaterial(rightMaterial);
        }
    }
}
