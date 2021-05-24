using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHand : MonoBehaviour
{
    [SerializeField] float windUpTimeMin = 1.0f;
    [SerializeField] float windUpTimeMax = 2.0f;
    [SerializeField] float windUpTimeMult = 1.0f;
    Vector3 localWeaponRotation;
    Animator animator;
    MeleeDamage weapon;
    Vector3 approxVel;
    Vector3 lastPos;
    Vector3 lastRot;
    Vector3 approxAngVel;
    AudioSource audio;
    [SerializeField] AudioClip swingAudio;
    float windUpTimer = 0;
    bool isPoweringUp = false;

    bool wantsToWindUp = false;
    bool wantsToAttack = false;

    // Start is called before the first frame update
    void Start()
    {        
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public void WantToWindUp()
    {
        wantsToWindUp = true;
        wantsToAttack = false;
    }

    public void WantToAttack()
    {
        wantsToAttack = true;
        wantsToWindUp = false;
    }

    private void Update()
    {
        if (isPoweringUp && windUpTimer < windUpTimeMax + windUpTimeMin) windUpTimer += Time.deltaTime * windUpTimeMult;
        if (HasWeapon())transform.localRotation = Quaternion.Euler(localWeaponRotation + new Vector3(Random.value * windUpTimer, Random.value * windUpTimer, Random.value * windUpTimer)*90);

        if (wantsToWindUp) StartAttack();
        if (wantsToAttack) Attack();
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
        weapon.gameObject.layer = 0;
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
        weapon = tPos.GetComponent<MeleeDamage>();

        // Set layer to player
        collider.gameObject.layer = 6;
        collider.attachedRigidbody.interpolation = RigidbodyInterpolation.None;
        weapon.gameObject.layer = 6;
        localWeaponRotation = weapon.transform.localRotation.eulerAngles;

        //Subscribe to StopAttack event
        MeleeDamage dmg = collider.GetComponentInParent<MeleeDamage>();
        dmg.onHitEvent += StopAttack;
    }

    bool StartAttack()
    {
        if (CanStartAttack())
        {
            animator.SetTrigger("Windup");
            isPoweringUp = true;
            wantsToWindUp = false;
            windUpTimer = windUpTimeMin;
            return true;
        }
        return false;
    }

    bool CanStartAttack()
    {
        return !isPoweringUp && animator.GetCurrentAnimatorStateInfo(0).IsName("melee_idle");
    }

    bool Attack()
    {
        if (!CanAttack()) return false;
        wantsToAttack = false;
        animator.SetTrigger("Attack");
        if (HasWeapon()) weapon.SetDmgMult(Mathf.Clamp(windUpTimer, windUpTimeMin, windUpTimeMax + windUpTimeMin));
        isPoweringUp = false;
        audio.pitch = .6f;
        audio.PlayOneShot(swingAudio);
        return true;
    }

    bool CanAttack()
    {
        return isPoweringUp && animator.GetCurrentAnimatorStateInfo(0).IsName("melee_windup");
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
