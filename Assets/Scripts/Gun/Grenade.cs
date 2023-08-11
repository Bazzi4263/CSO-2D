using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Grenade : MonoBehaviourPun
{
    public PhotonView PV;
    public AudioSource audioSource;

    Gun gun;
    Vector3 mousePos;

    bool isFire = false;

    private void Start()
    {
        if (PV.IsMine)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            gun = this.transform.parent.gameObject.GetComponent<Gun>();

            PV.RPC("PositionRPC", RpcTarget.All, mousePos);
        }

        float x = mousePos.x;
        float y = mousePos.y;

        mousePos = new Vector3(x, y, 0);

        gameObject.transform.parent = null;

        x = transform.position.x;
        y = transform.position.y;

        Vector3 pos = new Vector3(x, y, 0);

        float distance = Vector3.Distance(pos, mousePos);
        transform.DOJump(mousePos, distance / 10, 1, distance / 20).SetEase(Ease.Linear);
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (Vector2.Distance(mousePos, transform.position) < 0.5)
            {
                if (!isFire)
                {
                    EffectScript.instance.GetComponent<PhotonView>().RPC("ExplosionEffect", RpcTarget.All, transform.position);

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);
                    foreach (Collider2D hit in colliders)
                    {
                        if (hit.tag == "Zombie")
                        {
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;

                            hit.GetComponent<ZombieScript>().PV.RPC("Hit", RpcTarget.All, OICW.grenadeDamageMin, OICW.grenadeDamageMax, OICW.grenadeCriticalDamage, OICW.grenadeStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                        else if (hit.tag == "ZombieAI")
                        {
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;

                            hit.GetComponent<ZombieAI>().PV.RPC("Hit", RpcTarget.All, OICW.grenadeDamageMin, OICW.grenadeDamageMax, OICW.grenadeCriticalDamage, OICW.grenadeStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                    }
                    PV.RPC("DisapeearBullet", RpcTarget.All);
                    PV.RPC("Destroy", RpcTarget.All, 4f);
                    isFire = true;
                }
            }
        }
    }

    [PunRPC]
    void PositionRPC(Vector3 _mousePos)
    {
        mousePos = _mousePos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PV.IsMine)
        {
            if (collision.tag != "Bullet" && collision.tag != "Human")
            {
                if (!isFire)
                {
                    EffectScript.instance.GetComponent<PhotonView>().RPC("ExplosionEffect", RpcTarget.All, transform.position);

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4f);

                    foreach (Collider2D hit in colliders)
                    {
                        if (hit.tag == "Zombie")
                        {
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;

                            hit.GetComponent<ZombieScript>().PV.RPC("Hit", RpcTarget.All, OICW.grenadeDamageMin, OICW.grenadeDamageMax, OICW.grenadeCriticalDamage, OICW.grenadeStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                        else if (hit.tag == "ZombieAI")
                        {
                            Debug.Log("HitZombieAI");
                            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                            Vector2 explosionPos = transform.position;
                            Vector3 dir = (explosionPos - rb.position).normalized;

                            hit.GetComponent<ZombieAI>().PV.RPC("Hit", RpcTarget.All, OICW.grenadeDamageMin, OICW.grenadeDamageMax, OICW.grenadeCriticalDamage, OICW.grenadeStifness, -dir.x, -dir.y, gun.KnockBack);
                        }
                    }

                    PV.RPC("DisapeearBullet", RpcTarget.All);
                    PV.RPC("Destroy", RpcTarget.All, 4f);

                    isFire = true;
                }
            }
        }
    }

    [PunRPC]
    IEnumerator KnockBack(float _knockBack, Vector3 dir, Vector3 _pos, int _id)
    {
        GameObject zombie = PhotonView.Find(_id).gameObject;
        Rigidbody2D rb = zombie.GetComponent<Rigidbody2D>();

        zombie.GetComponent<ZombieScript>().isHit = true;
        rb.velocity = new Vector2(-dir.x * _knockBack, -dir.y * _knockBack);
        yield return new WaitForSeconds(0.1f);
        rb.velocity = new Vector2(0, 0);
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
