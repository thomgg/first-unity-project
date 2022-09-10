
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPickupControl : MonoBehaviour
{
    public float centre_y;
    public float yrange;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        centre_y = transform.position.y;
        transform.position.Set(transform.position.x, centre_y + yrange, transform.position.z);

        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null && PlayerPrefs.HasKey("Volume")) audio.volume = PlayerPrefs.GetFloat("Volume");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= centre_y + yrange)
            rb.AddForce(0, -0.1f, 0);
        else if (transform.position.y <= centre_y - yrange)
            rb.AddForce(0, 0.1f, 0);
        transform.Rotate(0, 90 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<AudioSource>().Play();
            gameObject.SetActive(false);
        }
    }
}
