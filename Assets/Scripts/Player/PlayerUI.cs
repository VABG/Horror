using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text textCenter;
    [SerializeField] UnityEngine.UI.Image fadeFromImage;
    [SerializeField] float textCenterFadeTime = .5f;
    [SerializeField] UnityEngine.UI.Image bloodEffect;
    [SerializeField] Color bloodEffectColor;
    [SerializeField] Color bloodEffectFadeoutColor;
    float fadeFromBlackTime = 0;
    bool fadeFromBlack = false;
    bool fadeFromBlackActivatesPlayer = true;
    float fadeFromBlackTimer = 0;
    float fadeStartTime = 0;

    float hurtTimer = 0;
    float hurtTime = 0;
    bool gotHurt = false;

    float timeSinceTextCenterSet = 0;

    float time = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        FadeTextCenter();
        FadeFromImage();
        UpdateBloodEffect();
    }

    public void GotHurt(float amount)
    {
        hurtTimer = amount;
        hurtTime = amount;
        gotHurt = true;
        bloodEffect.enabled = true;
        bloodEffect.color = bloodEffectColor;
    }
    void UpdateBloodEffect()
    {
        if (gotHurt)
        {
            hurtTimer -= Time.deltaTime;
            bloodEffect.color = Color.Lerp(bloodEffectFadeoutColor, bloodEffectColor, hurtTimer / hurtTime);
            if (hurtTimer <= 0)
            {
                bloodEffect.enabled = false;                
            }
        }
    }

    public void Die()
    {
        bloodEffect.enabled = true;
        bloodEffect.color = bloodEffectColor;
        gotHurt = false;
    }

    void FadeFromImage()
    {
        if (fadeFromBlack && time >= fadeStartTime)
        {
            fadeFromBlackTimer -= Time.deltaTime;
            if (fadeFromBlackTimer <= 0)
            {
                fadeFromBlackTimer = 0;
                fadeFromBlack = false;
                if (fadeFromBlackActivatesPlayer) GetComponent<FirstPersonController>().SetActive(true);
            }
            fadeFromImage.color = new Color(fadeFromImage.color.r, fadeFromImage.color.g, fadeFromImage.color.b, (float)fadeFromBlackTimer / (float)fadeFromBlackTime);
        }
    }

    public void StartFadeFromImage(float time, float timeToStart, bool unlockPlayer = true)
    {
        fadeStartTime = timeToStart;
        fadeFromBlackTime = fadeFromBlackTimer = time;
        fadeFromBlack = true;
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
