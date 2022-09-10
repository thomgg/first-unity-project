using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    Transform planet;

    // Start is called before the first frame update
    void Start()
    {
        planet = GetComponentInParent<Transform>();
        if (transform.position != planet.position)
            transform.position = planet.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<ThirdPersonCharacterMod>().SetGravityCentre(planet.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<ThirdPersonCharacterMod>().SetGravityCentre();
        }
    }
}
