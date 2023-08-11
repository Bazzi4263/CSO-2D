using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletAI : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public float bulletSpeed;
    public float offset;
    public Vector3 tragetPos;

    Quaternion rotation;

    AIRifle aiRifle;

    Vector3 dir;
    int zombieID;

    GameObject[] zombies;
    GameObject zombie = null;

    Vector3 target;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            aiRifle = this.transform.parent.gameObject.GetComponent<AIRifle>();
            target = transform.GetComponentInParent<AIRifle>().target.transform.position;

            dir = (target - transform.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - offset;

            float spread = Random.Range(-Rifle.s_spread - 0.8f, Rifle.s_spread + 0.8f);
       
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle + spread));
            PV.RPC("RotationRPC", RpcTarget.All, bulletRotation);
        }

        transform.rotation = rotation;
        gameObject.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
        gameObject.transform.parent = null;

        StartCoroutine(BulletSize());

        zombies = GameObject.FindGameObjectsWithTag("Zombie");
    }

    [PunRPC]
    void RotationRPC(Quaternion Rotation)
    {
        rotation = Rotation;
    }

    IEnumerator BulletSize()
    {
        for (int i = 0; i < 50; i++)
        {
            transform.localScale += new Vector3(0, 0.05f, 0);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.tag == "Zombie")
            {
                if (collision.gameObject == null)
                {
                    return;
                }

                collision.GetComponent<ZombieScript>().PV.RPC("Hit", RpcTarget.All, Rifle.s_gunDamageMin, Rifle.s_gunDamageMax, Rifle.s_criticalDamage, 0.05f, dir.x, dir.y, 0.1f);
                PV.RPC("DisapeearBullet", RpcTarget.All);
                PV.RPC("Destroy", RpcTarget.All, 4f);
            }
            else if (collision.gameObject.tag == "ZombieAI")
            {
                if (collision.gameObject == null)
                {
                    return;
                }

                collision.GetComponent<ZombieAI>().PV.RPC("Hit", RpcTarget.All, Rifle.s_gunDamageMin, Rifle.s_gunDamageMax, Rifle.s_criticalDamage, 0.05f, dir.x, dir.y, 0.1f);
                PV.RPC("DisapeearBullet", RpcTarget.All);
                PV.RPC("Destroy", RpcTarget.All, 4f);
            }
            else if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Human")
            {
                PV.RPC("DestroyRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    [PunRPC]
    void Destroy(float _time) => Destroy(gameObject, _time);

    [PunRPC]
    void DisapeearBullet()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }

}
