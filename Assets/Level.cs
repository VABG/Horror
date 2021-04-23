using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] float levelImageShowTime = 2.0f;
    [SerializeField] float fadeToGameTime = 3.0f;    
    FirstPersonController player;
    PlayerUI playerUI;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<FirstPersonController>();
        player.SetActive(false);
        playerUI = player.GetComponent<PlayerUI>();
        playerUI.StartFadeFromImage(fadeToGameTime, levelImageShowTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
