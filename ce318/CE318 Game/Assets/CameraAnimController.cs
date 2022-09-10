using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimController : MonoBehaviour
{
    int options_menu = Animator.StringToHash("options");
    Animator cam_anim;

    private void Start()
    {
        cam_anim = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }

    public void Options()
    {
        cam_anim.SetBool(options_menu, true);
    }

    public void MainMenu()
    {
        cam_anim.SetBool(options_menu, false);
    }
}
