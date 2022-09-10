using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 last_pos;

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(player.transform.position);
        last_pos = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
               
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position + player.transform.position - last_pos, 1f);

        transform.RotateAround(player.transform.position, player.transform.up, Input.GetAxis("CameraHorizontal"));
        transform.RotateAround(player.transform.position, transform.right, Input.GetAxis("CameraVertical"));

        last_pos = player.transform.position;
    }
}
