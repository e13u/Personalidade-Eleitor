using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    Slider sliderComponent;
    void Start()
    {
        sliderComponent = GetComponent<Slider>();
    }
    public void StartSliderAnimation(int maxValue)
    {
        StartCoroutine(SliderAnimation(maxValue));
    }
    IEnumerator SliderAnimation(int maxValue)
    {
        sliderComponent.value++;
        yield return new WaitForSeconds(0.025f);

        if (sliderComponent.value < maxValue) 
            StartCoroutine(SliderAnimation(maxValue));
        else 
            yield return new WaitForEndOfFrame();
    }
}
