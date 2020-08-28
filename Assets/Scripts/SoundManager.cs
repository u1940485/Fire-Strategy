using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    public Slider volumeSlider;
    public new AudioSource audio;

    // Use this for initialization
    void Start () {

        if (PlayerPrefs.HasKey("audioVolume"))
        {
            if (volumeSlider != null) volumeSlider.value = PlayerPrefs.GetFloat("audioVolume");
            if (audio != null) audio.volume = PlayerPrefs.GetFloat("audioVolume");
        }
        else
        {
            if (volumeSlider != null) volumeSlider.value = 0.5f;
            if (audio != null) audio.volume = 0.5f;

        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void storeAudioVolume()
    {
        if (volumeSlider != null) PlayerPrefs.SetFloat("audioVolume", volumeSlider.value);
    }
    public void updateAudioVolume()
    {
        if (audio != null && volumeSlider != null)
        {
            audio.volume = volumeSlider.value;
        }
    }
}
