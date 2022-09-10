using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollsionEvents : MonoBehaviour
{
    public GameplayController gc;
    private void Start()
    {
        gc = GameObject.Find("GameplayController").GetComponent<GameplayController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "SmallData":
                gc.AddDataPoint();
                break;
            case "BigData":
                gc.AddBigDataPoint();
                break;
            case "HealthPack":
                gc.AddBattery();
                gc.ChangeHealth(1);
                break;
            case "DeathZone":
                gc.ChangeHealth(-1);
                if (gc.health <= 0)
                {
                    gc.health = 0;
                    gc.ChangeHealth(1);
                }
                break;
            case "Weakpoint":
                other.transform.parent.gameObject.SetActive(false);
                GetComponent<Rigidbody>().AddForce(new Vector3(0, 2, 0));
                break;
         
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") {
            gc.ChangeHealth(-1);
            if (gc.health <= 0)
            {
                gc.health = 0;
                gc.ChangeHealth(1);
                transform.position = new Vector3(0, 0, 0);
            } 
        }
    }
}
