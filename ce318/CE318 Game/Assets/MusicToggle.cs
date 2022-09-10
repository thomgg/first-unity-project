using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
    public Text music_toggle_text;
    bool music_on = true;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicOn"))
        {
            if (PlayerPrefs.GetInt("MusicOn") == 1) music_on = true;
            else music_on = false;
        }

        if (music_on) PlayerPrefs.SetInt("MusicOn", 1);
        else PlayerPrefs.SetInt("MusicOn", 0);

        ToggleMusic();
        ToggleMusic();
    }

    public void ToggleMusic()
    {
        music_on = !music_on;
        if (music_on)
        {
            music_toggle_text.text = "Music: On";
            PlayerPrefs.SetInt("MusicOn", 1);
        }
        else { 
            music_toggle_text.text = "Music: Off";
            PlayerPrefs.SetInt("MusicOn", 0);
        }
    }

}
