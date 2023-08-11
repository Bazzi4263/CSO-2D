using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teleporter : MonoBehaviour
{
    public GameObject obj_Exit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Human" || collision.tag == "Zombie") && collision.GetComponent<HumanAI>() == null)
        {
            collision.transform.position = new Vector3(obj_Exit.transform.position.x, obj_Exit.transform.position.y, collision.transform.position.z);
        }
    }
}
