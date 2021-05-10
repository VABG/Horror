using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamagable
{
    abstract void Damage(float damage, Vector3 position, Vector3 force);
}

public class MeleeDamage : MonoBehaviour
{
    public delegate void HitStatic();
    public event HitStatic onHitEvent;

    Collider[] colliders;
    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
    }


    public void SetIsTrigger(bool isTrigger)
    {
        foreach(Collider c in colliders)
        {
            c.isTrigger = isTrigger;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        IDamagable dmg = other.gameObject.GetComponentInParent<IDamagable>();
        if (dmg == null) other.gameObject.GetComponentInChildren<IDamagable>();
        if (dmg == null) other.gameObject.GetComponent<IDamagable>();
        if (dmg != null)
        {
            dmg.Damage(10, Vector3.zero, -transform.up * 20);
        }
        else
        {
            HitNonDamagable();
        }
    }

    public void HitNonDamagable()
    {
        onHitEvent?.Invoke();
    }
}
