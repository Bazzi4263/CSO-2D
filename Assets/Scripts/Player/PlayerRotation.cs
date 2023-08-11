using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField]
    private FieldOfView fieldOfView;

    private void Update()
    {
         Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         targetPosition.z = 0f;
         
         Vector3 aimDir = (targetPosition - transform.position).normalized;
         fieldOfView.SetAimDirection(aimDir);
    }
}
