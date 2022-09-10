using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    public float time; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        StartCoroutine(ChangeLevel());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator ChangeLevel()
    {
        FaderScript fs = GetComponent<FaderScript>();
        fs.BeginFade(-1);
        yield return new WaitForSeconds(time);
        if (!PlayerPrefs.HasKey("LastLevel")) PlayerPrefs.SetInt("LastLevel", 1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("LastLevel"));
    }
}
