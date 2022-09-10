using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector3 grav_centre;
    public int move_speed;
    public int jump_speed;

    private Rigidbody rb;
    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float horAxis = Input.GetAxis("Horizontal");
        float verAxis = Input.GetAxis("Vertical");

        rb.AddRelativeForce(horAxis * move_speed, 0, verAxis * move_speed);

        Vector3 forward = transform.forward;

        transform.up = Vector3.Lerp(transform.up, (transform.position - grav_centre).normalized, 0.8f);

        transform.rotation.SetLookRotation(forward);

        if (grounded)
        {
            rb.AddRelativeForce(new Vector3(0f, jump_speed * Input.GetAxis("Jump"), 0f));
        }
        rb.AddRelativeForce(Physics.gravity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Gravitation":
                grav_centre = gameObject.transform.position;
                break;
            case "Ground":
                grounded = true;
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Gravitation":
                grav_centre = transform.position;
                break;
            case "Ground":
                grounded = false;
                break;
        }
    }
}
