using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    FieldOfView fieldOfView;
    GameObject black;

    private void Start()
    {
        black = GameObject.FindWithTag("Black");
        fieldOfView = GameObject.FindWithTag("FOV").GetComponent<FieldOfView>();
        GameObject.FindWithTag("CMCamera").GetComponent<CameraFollowing>().SetFollower(this.gameObject);
    }

    private void Update()
    {
        SetOrigin();
        Move();
    }

    public void SetOrigin()
    {
        fieldOfView.SetOrigin(transform.position);
    }

    public void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 10, 0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-10, 0, 0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, -10, 0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(10, 0, 0) * Time.deltaTime;
        }
    }
}
