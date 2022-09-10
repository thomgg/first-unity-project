using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        AudioSource auds = GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("Volume"))
        {
            auds.volume = PlayerPrefs.GetFloat("Volume");
        }

        if (PlayerPrefs.HasKey("MusicOn"))
        {
            auds.mute = PlayerPrefs.GetInt("MusicOn") == 1 ? true : false;
        }
    }
}
