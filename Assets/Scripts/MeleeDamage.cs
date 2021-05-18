using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamagable
{
    abstract void Damage(float damage, Vector3 position, Vector3 force);
}

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] AudioClip audioHit;
    [SerializeField] AudioClip audioHitFail;

    public delegate void HitStatic();
    public event HitStatic onHitEvent;
    bool didHit = false;

    AudioSource audioSource;
    Collider[] colliders;
    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponentsInChildren<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetIsTrigger(bool isTrigger)
    {
        if (isTrigger) didHit = false;
        foreach (Collider c in colliders)
        {
            c.isTrigger = isTrigger;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (didHit) return;
        didHit = true;
        audioSource.pitch = 1 + Random.value * .2f - .1f;
        IDamagable dmg = other.gameObject.GetComponentInParent<IDamagable>();
        if (dmg == null) other.gameObject.GetComponentInChildren<IDamagable>();
        if (dmg == null) other.gameObject.GetComponent<IDamagable>();
        if (dmg != null)
        {
            dmg.Damage(10, Vector3.zero, -transform.up * 60);
            audioSource.PlayOneShot(audioHit);            
        }
        else
        {
            onHitEvent?.Invoke();
            audioSource.PlayOneShot(audioHitFail);
        }
    }
}
