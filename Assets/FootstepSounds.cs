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
    AudioSource source;
    float footstepTimer = .5f;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void UpdateFootstep(float speed, bool smallMovement = false)
    {
        footstepTimer -= Time.deltaTime * speed * foostepSpeedMult;
        //if (smallMovement && footstepTimer > .2f) footstepTimer = .15f;
        if (footstepTimer <= 0)
        {
            source.pitch = pitchBase + Random.value * pitchRandom;
            int rnd = Random.Range(0, footsteps.Length);
            source.PlayOneShot(footsteps[rnd], footstepVolume);
            footstepTimer = footstepTime;
        }       
    }
}
