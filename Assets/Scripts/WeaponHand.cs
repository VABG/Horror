using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHand : MonoBehaviour
{    
    Animator animator;
    GameObject weapon;
    Vector3 approxVel;
    Vector3 lastPos;
    Vector3 lastRot;
    Vector3 approxAngVel;
    AudioSource audio;
    [SerializeField] AudioClip swingAudio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public bool HasWeapon()
    {
        return weapon != null;
    }

    public void DropWeapon()
    {
        if (!HasWeapon()) return;
        Collider c = weapon.GetComponentInChildren<Collider>();

        //Unsubscribe to StopAttack event
        MeleeDamage dmg = c.GetComponentInParent<MeleeDamage>();
        dmg.onHitEvent -= StopAttack;

        // Hey!
        c.attachedRigidbody.isKinematic = false;
        c.attachedRigidbody.useGravity = true;
        c.attachedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        InteractablePhysPickup phys = c.GetComponent<InteractablePhysPickup>();
        phys.Trigger();
        Transform tPos = phys.grabTransform;
        tPos.SetParent(null);
        tPos.GetComponent<MeleeDamage>().SetIsTrigger(false);
        c.gameObject.layer = 0;
        weapon.layer = 0;
        weapon = null;        
        //Set velocity
        c.attachedRigidbody.velocity = approxVel;
        c.attachedRigidbody.angularVelocity = approxAngVel;
    }

    public void SetWeapon(Collider collider)
    {
        // If has weapon, then drop first
        if (HasWeapon()) DropWeapon();

        // Setup phsysics
        collider.attachedRigidbody.isKinematic = true;
        collider.attachedRigidbody.useGravity = false;

        // Get Components
        InteractablePhysPickup iPP = collider.GetComponent<InteractablePhysPickup>();
        Transform tPos = iPP.grabTransform;

        // Disable pickupable script
        iPP.enabled = false;

        // Set parent and move to origin
        tPos.SetParent(transform);
        tPos.position = transform.position;
        tPos.rotation = transform.rotation;

        //Set game object
        weapon = tPos.gameObject;

        // Set layer to player
        collider.gameObject.layer = 6;
        collider.attachedRigidbody.interpolation = RigidbodyInterpolation.None;
        weapon.layer = 6;

        //Subscribe to StopAttack event
        MeleeDamage dmg = collider.GetComponentInParent<MeleeDamage>();
        dmg.onHitEvent += StopAttack;
    }

    public void Attack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("melee_idle"))
        {
            animator.SetTrigger("Attack");
            audio.pitch = .6f;
            audio.PlayOneShot(swingAudio);
        }
    }

    public void StopAttack()
    {
        animator.SetTrigger("HitOther");
        DeactivateDmg();
    }

    public void ActivateDmg()
    {
        MeleeDamage m = GetComponentInChildren<MeleeDamage>();
        if (m != null) m.SetIsTrigger(true);
    }

    public void DeactivateDmg()
    {
        MeleeDamage m = GetComponentInChildren<MeleeDamage>();
        if (m != null) m.SetIsTrigger(false);
    }

    private void FixedUpdate()
    {
        approxVel = (transform.position - lastPos) / Time.fixedDeltaTime;
        lastPos = transform.position;

        approxAngVel = (transform.rotation.eulerAngles - lastRot) / Time.fixedDeltaTime;
        lastRot = transform.rotation.eulerAngles;
    }
}
