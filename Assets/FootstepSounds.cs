using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField] AudioClip[] footsteps;
    [SerializeField] float footstepTime = .5f;
    [SerializeField] float foostepSpeedMult = 5.0f;
    [SerializeField] float footstepVolume = .3f;

    [SerializeField] float pitchBase = 1.0f;
    [SerializeField] float pitchRandom = .1f;

    [SerializeField] bool hasAIListener;
    [SerializeField] LayerMask listenerMask;
    AudioSource source;
    float footstepTimer = .5f;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void UpdateFootstep(float speed, bool running = false)
    {
        footstepTimer -= Time.deltaTime * speed * foostepSpeedMult;
        //if (smallMovement && footstepTimer > .2f) footstepTimer = .15f;
        if (footstepTimer <= 0)
        {
            if (hasAIListener && running) AlertWithSound();
            source.pitch = pitchBase + Random.value * pitchRandom;
            int rnd = Random.Range(0, footsteps.Length);
            source.PlayOneShot(footsteps[rnd], running ? footstepVolume : footstepVolume/3);
            footstepTimer = footstepTime;
        }       
    }

    private void AlertWithSound()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 10, Vector3.up, listenerMask);
        foreach (RaycastHit h in hits)
        {
            IListener l = h.transform.GetComponent<IListener>();
            if (l != null) l.HearSound(1, transform.position);
        }
    }
}
