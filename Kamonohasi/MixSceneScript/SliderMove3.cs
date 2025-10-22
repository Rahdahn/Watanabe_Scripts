using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SliderMove3 : MonoBehaviour
{
    [SerializeField] Slider currentSlider;
    [SerializeField] Slider nextSlider;
    private bool isClicked;
    private bool maxValue;

    void Start()
    {
        currentSlider.value = 0;
        nextSlider.gameObject.SetActive(false); 
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
            if (currentSlider.value >= 8 && currentSlider.value <= 10)
            {
                currentSlider.gameObject.SetActive(false); 
                nextSlider.gameObject.SetActive(true); 
                nextSlider.value = 0; 
                UnityEngine.Debug.Log("成功");
            }
            else
            {
                UnityEngine.Debug.Log("失敗");
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