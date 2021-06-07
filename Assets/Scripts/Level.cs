using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] float levelImageShowTime = 2.0f;
    [SerializeField] float fadeToGameTime = 3.0f;
    [SerializeField] bool doIntro = false;
    [SerializeField] KeyPad[] setCodeTo;
    [SerializeField] InteractablePaper paperWithCode;
    string doorCode;

    FirstPersonController player;
    PlayerUI playerUI;
    // Start is called before the first frame update
    void Start()
    {
        SetRandomCode();

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

    void SetRandomCode()
    {
        int rnd = Random.Range(0, 9999);
        char[] code = new char[4];
        char[] codeGet = rnd.ToString().ToCharArray();
        if (codeGet.Length < 4)
        {
            int offset = 4 - codeGet.Length;
            for (int i = 0; i < 4; i++)
            {
                if (i >= offset)
                {
                    code[i] = codeGet[i - offset];
                }
                else
                {
                    code[i] = '0';
                }
            }
        }
        else code = codeGet;
        doorCode = new string(code);
        for (int i = 0; i < setCodeTo.Length; i++)
        {
            setCodeTo[i].SetCode(doorCode);
            paperWithCode.SetText("The door code is: " + doorCode);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
