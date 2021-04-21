using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        if (currentInput.Length == code.Length)
        {
            if (currentInput == code) CorrectInput();
            else BadInput();
        }
    }

    public void CorrectInput()
    {
        correctCodeEvent.Invoke();
        statusLight.SetCorrect(activeTime);
        currentInput = "";
        correctActivated = true;
        timeSinceCorrect = activeTime;
    }

    public void BadInput()
    {
        currentInput = "";
        // Make light red        
        statusLight.SetError(.5f);

    }

}
