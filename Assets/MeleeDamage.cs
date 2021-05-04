using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamagable
{
    abstract void Damage(float damage, Vector3 position, Vector3 force);
}


public class MeleeDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamagable dmg = collision.gameObject.GetComponentInParent<IDamagable>();
        if (dmg == null) collision.gameObject.GetComponentInChildren<IDamagable>();
        if (dmg == null) collision.gameObject.GetComponent<IDamagable>();
        if (dmg != null)
        {
            dmg.Damage(10, Vector3.zero, Vector3.zero);
        }
            
    }
}
