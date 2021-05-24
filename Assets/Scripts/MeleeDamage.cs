using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamagable
{
    abstract void Damage(float damage, Vector3 position, Vector3 force);
}

[RequireComponent(typeof(AudioSource))]
public class MeleeDamage : MonoBehaviour
{
    [SerializeField] AudioClip audioHit;
    [SerializeField] AudioClip audioHitFail;

    float damageMultiplier = 1;
    public delegate void HitStatic();
    public event HitStatic onHitEvent;

    AudioSource audioSource;
    Collider[] colliders;

    List<Collider> hitColliders;

    // Start is called before the first frame update
    void Start()
    {
        hitColliders = new List<Collider>();
        colliders = GetComponentsInChildren<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetDmgMult(float mult)
    {
        damageMultiplier = mult;
    }

    public void SetIsTrigger(bool isTrigger)
    {
        hitColliders.Clear();
        foreach (Collider c in colliders)
        {
            c.isTrigger = isTrigger;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitColliders.Contains(other) || hitColliders.Count > 2)
        {
            return;
        }

        float dmgMult = (3.0f - hitColliders.Count) / 3.0f;
        dmgMult *= damageMultiplier;

        hitColliders.Add(other);


        audioSource.pitch = 1 + Random.value * .2f - .1f;
        IDamagable dmg = other.gameObject.GetComponentInParent<IDamagable>();
        if (dmg == null) other.gameObject.GetComponentInChildren<IDamagable>();
        if (dmg == null) other.gameObject.GetComponent<IDamagable>();
        if (dmg != null)
        {
            dmg.Damage(10*dmgMult, Vector3.zero, -transform.up * 60*dmgMult);
            audioSource.PlayOneShot(audioHit, dmgMult);
        }
        else
        {
            onHitEvent?.Invoke();
            audioSource.PlayOneShot(audioHitFail, dmgMult);
            return;
        }

        if (hitColliders.Count == 3) onHitEvent?.Invoke();
    }
}
