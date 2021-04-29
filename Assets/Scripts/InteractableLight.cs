using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLight : MonoBehaviour, InteractableBasic
{
    [SerializeField] bool on = true;

    // Interactivity Msg
    public ColorAndText LookedAtInfo { 
        get { return new ColorAndText { text = on ? onString : offString, color = on ? Color.black : Color.white }; } 
        set => throw new System.NotImplementedException(); }
    [SerializeField] string onString;
    [SerializeField] string offString;

    Material material;

    // Bulb colors
    [ColorUsage(true, true)]
    [SerializeField] Color emissiveColorOff;
    Color emissiveColor;

    // Light colors
    [ColorUsage(true, true)]
    [SerializeField] Color lightOffColor;
    Color lightOnColor;
    Light light;
    
    // Lerp
    float lerp = 1;
    [Range(0.001f, 10.0f)]
    [SerializeField] float lerpSpeedOn = .2f;

    [Range(0.001f, 10.0f)]
    [SerializeField] float lerpSpeedOff = .2f;

    bool changingColor = false;

    public void Trigger()
    {
        changingColor = true;        
        on = !on;
    }

    // Start is called before the first frame update
    void Start()
    {
        changingColor = true;

        if (!on)
        {
            lerp = 0;
        }
        else
        {
            lerp = 1;
        }
        if (lerpSpeedOn <= 0) lerpSpeedOn = .001f;
        if (lerpSpeedOff <= 0) lerpSpeedOff = .001f;

        light = GetComponent<Light>();
        lightOnColor = light.color;
        material = GetComponent<MeshRenderer>().material;
        emissiveColor = material.GetColor("_EmissionColor");
    }

    // Update is called once per frame
    void Update()
    {
        if (changingColor)
        {
            if (on)
            {
                lerp += Time.deltaTime/lerpSpeedOn;
                if (lerp >= 1)
                {
                    lerp = 1;
                    changingColor = false;
                }
            }
            else
            {
                lerp -= Time.deltaTime/lerpSpeedOff;
                if (lerp <= 0)
                {
                    lerp = 0;
                    changingColor = false;
                }
            }

            //Set colors
            float lerpMod = Mathf.SmoothStep(0, 1, lerp);
            //float lerpMod = lerp > 0 ? lerp * lerp : 0;
            light.color = Color.Lerp(lightOffColor, lightOnColor, lerpMod);
            material.SetColor("_EmissionColor", Color.Lerp(emissiveColorOff, emissiveColor, lerpMod));
        }
    }

    public PickupMode GetPickupMode()
    {
        return PickupMode.None;
    }
}
