using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDifficulty : MonoBehaviour
{
    public Text diff_text;

    //enum for referencing difficulty levels
    enum difficulty
    {
        Normal,
        Harder,
        Daredevil
    }
    difficulty diff_setting;

    //initialises difficulty, using PlayerPrefs if possible
    private void Start()
    {
        if (PlayerPrefs.HasKey("Difficulty")) diff_setting = (difficulty)PlayerPrefs.GetInt("Difficulty");
        else diff_setting = difficulty.Normal;
        diff_text.text = "Difficulty: " + diff_setting.ToString();
        PlayerPrefs.SetInt("Difficulty", (int)diff_setting);
    }

    //change difficulty setting
    public void changeDifficulty()
    {
        if (diff_setting++ == difficulty.Daredevil) diff_setting = difficulty.Normal;
        diff_text.text = "Difficulty: " + diff_setting.ToString();
        PlayerPrefs.SetInt("Difficulty", (int)diff_setting);
    }
}
