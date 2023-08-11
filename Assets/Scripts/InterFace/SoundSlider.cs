using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    public Slider slider;

    void Update()
    {
        ChangeSound();
    }

    public void ChangeSound()
    {
        AudioListener.volume = slider.value;
    }
}
