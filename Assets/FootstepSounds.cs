using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField] AudioClip[] footsteps;
    AudioSource source;
    float footstepTimer = .5f;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void UpdateFootstep(float speed, bool smallMovement = false)
    {
        footstepTimer -= Time.deltaTime * speed * 5;
        if (smallMovement && footstepTimer > .2f) footstepTimer = .15f;
        if (footstepTimer <= 0)
        {
            source.pitch = 1 + Random.value * .1f;
            int rnd = Random.Range(0, footsteps.Length);
            source.PlayOneShot(footsteps[rnd], .3f);
            footstepTimer = .5f;
        }       
    }
}
