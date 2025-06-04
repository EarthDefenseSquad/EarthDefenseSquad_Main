using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SoundSliderUI : MonoBehaviour
{

    public AudioMixer masterMixer;
    public Slider audioSlider;

    public void AudioControl()
    {
        float volume = audioSlider.value;

        if (volume == -40f) masterMixer.SetFloat("Master", -80);
        else masterMixer.SetFloat("Master", volume);
        
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

   
}

