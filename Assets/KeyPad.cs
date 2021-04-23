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
    [SerializeField] string code;

    [SerializeField] float activeTime = 4.0f;
    [SerializeField] float errorTime = .5f;

    [SerializeField] KeyPadStatusLight statusLight;
    [SerializeField] UnityEngine.UI.Text text;
    [SerializeField] GameObject progressObject;
    [SerializeField] List<GameObject> buttons;

    [SerializeField] UnityEvent correctCodeEvent;
    [SerializeField] UnityEvent inactiveEvent;
    public KeyPadStatus status;
    string currentInput = "";

    float timeSinceCorrect = 0;
    bool correctActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        SetProgress();
    }

    // Update is called once per frame
    void Update()
    {
        if (correctActivated)
        {
            timeSinceCorrect -= Time.deltaTime;
            if (timeSinceCorrect <= 0)
            {
                correctActivated = false;
                inactiveEvent.Invoke();
            }
        }
    }

    public void ReceiveInput(int number)
    {
        currentInput += number.ToString();
        SetProgress();
        if (currentInput.Length == code.Length)
        {
            if (currentInput == code) CorrectInput();
            else BadInput();
        }

    }

    void SetProgress()
    {
        if (currentInput.Length == 0) progressObject.transform.localScale = new Vector3(0, 1, 1);

        progressObject.transform.localScale = new Vector3((float)currentInput.Length / (float)code.Length, 1, 1);
        text.text = currentInput;
    }

    public void CorrectInput()
    {
        correctCodeEvent.Invoke();
        statusLight.SetCorrect(activeTime);
        currentInput = "";
        correctActivated = true;
        timeSinceCorrect = activeTime;
        SetProgress();
    }

    public void BadInput()
    {
        currentInput = "";
        // Make light red        
        statusLight.SetError(errorTime);
        SetProgress();
    }

}
