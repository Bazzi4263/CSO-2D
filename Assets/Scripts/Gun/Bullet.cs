using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public float bulletSpeed;
    public float offset;

    float shotgunTime;

    Quaternion rotation;

    Gun gun;

    Vector3 dir;
    int zombieID;

    GameObject[] zombies;
    GameObject zombie = null;

    private void Start()
    {
        if (PV.IsMine)
        {
            gun = this.transform.parent.gameObject.GetComponent<Gun>();
            shotgunTime = gun.shotgunTime;
            Vector3 sp = transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            dir = (mousePos - sp).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - offset;

            float spread = 0;

            switch (gun.gameObject.tag)
            {
                case "Rifle":
                    if (!gun.recoilStart)
                        spread = Random.Range(-1.5f, 1.5f);
                    else
                        spread = Random.Range(-gun.spread, gun.spread);
                    break;
                case "DMR":
                    if (!gun.recoilStart && gun.gameObject.GetComponent<DMR>().isZoom)
                        spread = Random.Range(-0.8f, 0.8f);
                    else
                        spread = Random.Range(-gun.spread, gun.spread);
                    break;
                default:
                    spread = Random.Range(-gun.spread, gun.spread);
                    break;

            }

            if (gun.transform.GetComponentInParent<HumanScript>().isMove)
            {
                switch (gun.gameObject.tag)
                {
                    case "SMG":
                        if (spread < 0)
                            spread -= 0.7f;
                        else
                            spread += 0.7f;
                        break;
                    case "HandGun":
                        if (spread < 0)
                            spread -= 0.7f;
                        else
                            spread += 0.7f;
                        break;
                    case "ShotGun":
                        if (spread < 0)
                            spread -= 0.7f;
                        else
                            spread += 0.7f;
                        break;
                    case "M16A4":
                        if (spread < 0)
                            spread -= 3.2f;
                        else
                            spread += 3.2f;
                        break;
                    default:
                        if (gun.name == "Minigun")
                        {
                            if (spread < 0)
                                spread -= 0.9f;
                            else
                                spread += 0.9f;
                            break;
                        }
                        else
                        {
                            if (spread < 0)
                                spread -= 2.3f;
                            else
                                spread += 2.3f;
                            break;
                        }
                }
            }

            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle + spread));

            PV.RPC("RotationRPC", RpcTarget.All, bulletRotation);
        }

        transform.rotation = rotation;
        gameObject.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
        gameObject.transform.parent = null;

        StartCoroutine(BulletSize());

        zombies = GameObject.FindGameObjectsWithTag("Zombie");
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (gun.gameObject.CompareTag("ShotGun"))
            {
                if (shotgunTime > 0)
                {
                    shotgunTime -= Time.deltaTime;
                }
                else
                {
                    PV.RPC("DestroyRPC", RpcTarget.All);
                }
            }
        }
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
        if (PV.IsMine)
        {
            if (collision.tag == "Zombie" && !collision.gameObject.GetPhotonView().IsMine)
            {
                if (collision.gameObject == null)
                {
                    return;
                }

                collision.GetComponent<ZombieScript>().PV.RPC("Hit", RpcTarget.All, gun.gunDamageMin, gun.gunDamageMax, gun.criticalDamage, gun.Stiffness, dir.x, dir.y, gun.KnockBack);
                PV.RPC("DisapeearBullet", RpcTarget.All);
                PV.RPC("Destroy", RpcTarget.All, 4f);
            }
            else if (collision.gameObject.tag == "ZombieAI")
            {
                if (collision.gameObject == null)
                {
                    return;
                }

                collision.GetComponent<ZombieAI>().PV.RPC("Hit", RpcTarget.All, gun.gunDamageMin, gun.gunDamageMax, gun.criticalDamage, gun.Stiffness, dir.x, dir.y, gun.KnockBack);
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
