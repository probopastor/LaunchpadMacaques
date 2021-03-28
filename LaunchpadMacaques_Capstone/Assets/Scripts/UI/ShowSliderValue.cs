/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* ShowSliderValue.CS
* Displays slider value
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowSliderValue : MonoBehaviour
{
    Slider thisSlider;
    [SerializeField] TextMeshProUGUI sliderValueText = null;
    // Start is called before the first frame update
    void Start()
    {
        thisSlider = this.GetComponent<Slider>();
    }

    private void OnEnable()
    {
        thisSlider = this.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if(thisSlider.maxValue == 1)
        {
            sliderValueText.text = (int)(thisSlider.value * 100) + "%";
        }

        else
        {
            sliderValueText.text = thisSlider.value +"";
        }
    }
}
