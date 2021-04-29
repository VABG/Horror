using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] float levelImageShowTime = 2.0f;
    [SerializeField] float fadeToGameTime = 3.0f;
    [SerializeField] bool doIntro = false;
    FirstPersonController player;
    PlayerUI playerUI;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<FirstPersonController>();
        playerUI = player.GetComponent<PlayerUI>();

        if (doIntro)
        {
            player.SetActive(false);
            playerUI.StartFadeFromImage(fadeToGameTime, levelImageShowTime);
        }
        else
        {
            playerUI.StartFadeFromImage(.01f, .01f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
