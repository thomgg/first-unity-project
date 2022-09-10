using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolChange : MonoBehaviour
{  public void ChangeVolume()
    {
        PlayerPrefs.SetFloat("Volume", GetComponentInParent<Slider>().value);
    }
}
