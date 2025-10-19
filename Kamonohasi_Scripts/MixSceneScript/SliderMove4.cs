using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SlideMove4 : MonoBehaviour
{
    [SerializeField] Slider currentSlider;
    private bool isClicked;
    private bool maxValue;

    void Start()
    {
        UnityEngine.Debug.Log("start");
        currentSlider.value = 0;
        maxValue = false;
        isClicked = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isClicked == false)
        {
            UnityEngine.Debug.Log("stop");
            isClicked = true;
        }
        else if (Input.GetMouseButtonDown(0) && isClicked == true)
        {
            UnityEngine.Debug.Log("start");
            isClicked = false;
        }
        if (isClicked)
        {
            if (currentSlider.value >= 4 && currentSlider.value <= 6)
            {
                UnityEngine.Debug.Log("¬Œ÷");
                SceneManager.LoadScene("ResultScene");
            }
            else
            {
                UnityEngine.Debug.Log("Ž¸”s");
                SceneManager.LoadScene("ResultButScene");
            }
            return;
        }

        if (currentSlider.value == 10)
        {
            maxValue = true;
        }
        if (currentSlider.value == 0)
        {
            maxValue = false;
        }

        if (maxValue == true)
        {
            currentSlider.value -= 0.04f;
        }

        if (maxValue == false)
        {
            currentSlider.value += 0.04f;
        }
    }
}