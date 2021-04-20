using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text textCenter;
    [SerializeField] float textCenterFadeTime = 2;

    float timeSinceTextCenterSet = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        FadeTextCenter();
    }

    private void FadeTextCenter()
    {
        if (timeSinceTextCenterSet >= 0)
        {
            timeSinceTextCenterSet -= Time.deltaTime;
            float fadeAmount = timeSinceTextCenterSet / textCenterFadeTime;
            textCenter.color = new Color(textCenter.color.r, textCenter.color.g, textCenter.color.b, fadeAmount);
        }
    }
    public void SetCenterText(string text, Color color)
    {
        timeSinceTextCenterSet = textCenterFadeTime;
        if (textCenter.text != text)
        {
            textCenter.text = text;
            textCenter.color = color;
        }
    }

    public void ClearCenterText()
    {
        if (textCenter.text != "")
        {
            textCenter.text = "";
        }
    }
}
