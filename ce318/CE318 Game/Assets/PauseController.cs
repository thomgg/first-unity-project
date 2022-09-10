using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class PauseController : MonoBehaviour
{
    bool pause;
    public GameObject pause_menu;
    private Scene current_scene;
    private Dropdown drop;
    public Button goBut;
    // Start is called before the first frame update
    void Start()
    {
        pause = false;
        pause_menu.SetActive(false);
        drop = pause_menu.GetComponentInChildren<Dropdown>();
        current_scene = SceneManager.GetActiveScene();
        StartCoroutine(FadeIn());
    }

 

    // Update is called once per frame
    void Update()
    {
        
        if (pause)
        {
            try
            {
                goBut.interactable = !(SceneManager.GetSceneByBuildIndex(drop.value + 1) == current_scene);
            } catch (System.ArgumentException)
            {
                goBut.interactable = false;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                pause = false;
                Time.timeScale = 1;
                pause_menu.SetActive(false);
            }
        } else if (Input.GetKeyDown(KeyCode.P))
        {
            pause = true;
            Time.timeScale = 0;
            pause_menu.SetActive(true);
        }
    }

    public void toMainMenu()
    {
        StartCoroutine(ChangeLevel(0));
        FindObjectOfType<LevelState>().Save();
        FindObjectOfType<GameplayController>().Save();
        Time.timeScale = 1;
    }

    public void toOtherLevel()
    {
        PlayerPrefs.SetInt("LastLevel", drop.value + 1);
        StartCoroutine(ChangeLevel(drop.value + 1));
        FindObjectOfType<LevelState>().Save();
        Time.timeScale = 1;
    }

    IEnumerator FadeIn()
    {
        FaderScript fs = GetComponent<FaderScript>();
        fs.BeginFade(-1);
        yield return new WaitForSeconds(1);
    }

    IEnumerator ChangeLevel(int scene_id)
    {
        FaderScript fs = GetComponent<FaderScript>();
        fs.BeginFade(-1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene_id);
    }
}
