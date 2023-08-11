using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public GameObject posObject;
    public GameObject targetObject; //player
    public CinemachineVirtualCamera CV;
    public CinemachineConfiner CC;

    private void LateUpdate()
    {
        if (targetObject == null)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        posObject.transform.position = Vector3.Lerp(targetObject.transform.position, mousePos, 0.5f);

        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 pos = (mousePos + targetObject.transform.position) / 2;

        //pos.x = Mathf.Clamp(pos.x, targetObject.transform.position.x - xMax, targetObject.transform.position.x + xMax);
        //pos.y = Mathf.Clamp(pos.y, targetObject.transform.position.y - yMax, targetObject.transform.position.y + yMax);

        //posObject.transform.position = Vector3.Lerp(posObject.transform.position, pos, Time.deltaTime * 10);

        //CV.Follow = posObject.transform;
        //CV.LookAt = posObject.transform;
    }

    public void SetFollower(GameObject _gameObject)
    {
        CV.Follow = _gameObject.transform;
        //CV.LookAt = _gameObject.transform;
    }
}
