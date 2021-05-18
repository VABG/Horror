using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.UI;
public enum KeyPadStatus
{
    Open,
    Closed,
    Idle,
    Off
}

public class KeyPad : MonoBehaviour
{
    [SerializeField] AudioClip audioButton;
    [SerializeField] AudioClip audioFail;
    [SerializeField] AudioClip audioUnlock;
    [SerializeField] string code;

    [SerializeField] float activeTime = 4.0f;
    [SerializeField] float errorTime = .5f;

    [SerializeField] KeyPadStatusLight statusLight;
    [SerializeField] UnityEngine.UI.Text text;
    [SerializeField] GameObject progressObject;

    [SerializeField] UnityEvent correctCodeEvent;
    [SerializeField] UnityEvent inactiveEvent;
    public KeyPadStatus status;
    string currentInput = "";

    float timeSinceChange = 0;
    bool activated = false;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetProgress();
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            timeSinceChange -= Time.deltaTime;
            statusLight.timer = timeSinceChange;

            if (timeSinceChange <= 0)
            {
                activated = false;
                currentInput = "";
                inactiveEvent.Invoke();
                SetProgress();
                statusLight.SetIdle();
            }
        }
    }

    public void ReceiveInput(int number)
    {
        if (activated) return;
        audioSource.PlayOneShot(audioButton, .5f);
        currentInput += number.ToString();
        SetProgress();
        if (currentInput.Length == code.Length)
        {
            if (currentInput == code) CorrectInput();
            else BadInput();
        }
    }

    void SetProgress(bool error = false)
    {
        if (error)
        {
            progressObject.transform.localScale = new Vector3(1, 1, 1);
            text.text = currentInput;
            return;
        }
        if (currentInput.Length == 0) progressObject.transform.localScale = new Vector3(0, 1, 1);

        progressObject.transform.localScale = new Vector3((float)currentInput.Length / (float)code.Length, 1, 1);
        text.text = currentInput;
    }

    void CorrectInput()
    {
        audioSource.PlayOneShot(audioUnlock, .3f);
        correctCodeEvent.Invoke();
        statusLight.SetCorrect();
        currentInput = "";
        activated = true;
        timeSinceChange = activeTime;
        SetProgress();
    }

    void BadInput()
    {
        audioSource.PlayOneShot(audioFail, .7f);
        // Make light red        
        statusLight.SetError();
        currentInput = "- - - -";
        SetProgress(true);
        currentInput = "";
        activated = true;
        timeSinceChange = errorTime;
    }
}
