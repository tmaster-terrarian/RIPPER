using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Internal;

public class AdrenalineSystem : MonoBehaviour
{
    Slider adrenalineBar = null;
    GameObject adrenalineAlertText = null;
    Volume adrenalinePPVolume = null;
    bool isDraining = false;
    [SerializeField] float adrenaline = 0;
    [SerializeField] float adrenalineMax = 100;
    [SerializeField] float adrenalineDrainRate = 3;
    [SerializeField] float transitionInSpeed = 1;
    [SerializeField] float transitionOutSpeed = 1;

    [SerializeField] float debugPassiveGainRate = 0;

    void Start()
    {
        adrenalineBar = GameObject.FindGameObjectWithTag("AdrenalineBar").GetComponent<Slider>();
        adrenalineAlertText = GameObject.FindGameObjectWithTag("AdrenalineAlertText");
        adrenalinePPVolume = GameObject.FindGameObjectWithTag("AdrenalinePPVolume").GetComponent<Volume>();
        adrenalineAlertText.SetActive(false);
        adrenalinePPVolume.weight = 0;
        adrenalineBar.minValue = 0;
        adrenalineBar.maxValue = adrenalineMax;
    }

    void Update()
    {
        adrenalineBar.value = adrenaline;
        if(isDraining == false)
        {
            adrenaline += debugPassiveGainRate * 0.01f;
            adrenalinePPVolume.weight = Mathf.Lerp(adrenalinePPVolume.weight, 0, transitionOutSpeed * Time.deltaTime);
            adrenalinePPVolume.weight = Mathf.Clamp(adrenalinePPVolume.weight, 0.0001f, 0.9999f);
        }
        adrenaline = Mathf.Clamp(adrenaline, adrenalineBar.minValue, adrenalineBar.maxValue);

        if(adrenaline == adrenalineMax)
        {
            adrenalineAlertText.SetActive(true);

            if(Input.GetKeyDown(KeyCode.R))
            {
                adrenalinePPVolume.weight = 0;
                isDraining = true;
            }
        }
        else
        {
            adrenalineAlertText.SetActive(false);
        }

        Debug();
    }

    void FixedUpdate()
    {
        DrainAdrenaline();
    }

    void Debug()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Add(10);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Add(-10);
        }
    }

    void DrainAdrenaline()
    {
        if(isDraining == true)
        {
            adrenalinePPVolume.weight = Mathf.Lerp(adrenalinePPVolume.weight, 1, transitionInSpeed * Time.deltaTime);
            adrenalinePPVolume.weight = Mathf.Clamp(adrenalinePPVolume.weight, 0.0001f, 0.9999f);
            adrenaline -= adrenalineBar.maxValue * 0.0001f * adrenalineDrainRate;

            if(adrenaline <= 0)
            {
                adrenaline = 0;
                isDraining = false;
            }
        }
    }

    ///<summary>Add a value to the player's adrenaline (or subtract).</summary>
    public void Add(float value)
    {
        adrenaline += value;
    }
}
