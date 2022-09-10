using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 gdir = (gameObject.transform.position - transform.position).normalized;
            gameObject.GetComponent<PlayerController>().SetGravityDirection(gdir.x, gdir.y, gdir.z);
        }

    }
}
